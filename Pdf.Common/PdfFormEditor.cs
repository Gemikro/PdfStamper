using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using iTextSharp.text.pdf;
using Pdf.Interfaces;
using System.Linq;
//using Newtonsoft.Json;
using Utilities;
using iTextSharp.text;

namespace Pdf.Common
{
    public class PdfFormEditor
    {
        const string UNKNOWN = "Unknown";
        static object _lock = new object();

        
        public static void InitMappings(string template_root_path, string mapping_root_path, string output_root_path) {

            lock (_lock) {
                mappings mapping = new mappings();
                
                if (File.Exists(mapping_root_path))
                    mapping = File.ReadAllText(mapping_root_path).DeserializeXml2<mappings>();

                if(mapping.mapping_item==null)
                    mapping.mapping_item = new mapping_item_type[0];

                string[] files = Directory.GetFiles(template_root_path, "*.pdf");
                int i = 0;
                foreach (string file in files) {
                    i++;
                    FileInfo f = new FileInfo(file);
                    string input_path = String.Format(@"@root\{0}", f.Name);

                    mapping_item_type current = null;
                    var select = mapping.mapping_item.Where(p => p.file_path == input_path);
                    if (select.Count() == 1) {
                        current = select.First();
                    }
                    else {
                        current = new mapping_item_type();
                        current.mapping = new mapping_type[0];
                        current.file_path = input_path;
                        current.pdf_template = (template)Enum.Parse(typeof(template), string.Format("Obrazac{0}", i));

                        var list = mapping.mapping_item.ToList();
                        list.Add(current);
                        mapping.mapping_item = list.ToArray();
                    }

                    PdfReader reader = new PdfReader(file);
                    string destination_document_path = Path.Combine(output_root_path,
                        String.Format("{0}{1}{2}",
                        f.Name.Replace(f.Extension, ""),
                        DateTime.Now.ToFileTimeUtc().ToString(),
                        f.Extension)
                        );

                    using (PdfStamper stamper = new PdfStamper(reader, new FileStream(destination_document_path, FileMode.Create))) {
                        AcroFields fields = stamper.AcroFields;

                        foreach (var key in fields.Fields.Keys) {

                            var items = current.mapping.Where(p => p.data_field == key);
                            if (items.Count() == 1) {
                                continue;
                            }
                            else if (items.Count() == 0) {

                                var mappings = current.mapping.ToList();

                                data_field_type field_type = data_field_type.Text;
                                if (key.Contains("Check"))
                                    field_type = data_field_type.CheckBox;
                                else if (key.Contains("Radio"))
                                    field_type = data_field_type.RadioButton;

                                mappings.Add(new mapping_type() { column_name = UNKNOWN, data_field = key, field_type = field_type });

                                current.mapping = mappings.ToArray();                                
                            }
                            else {
                                throw new NotImplementedException();
                            }
                        }                        
                    }
                    File.WriteAllText(mapping_root_path, mapping.SerializeXml());
                }
            }
        }
        /// <summary>
        /// Merges pdf files from a byte list
        /// </summary>
        /// <param name="files">list of files to merge</param>
        /// <returns>memory stream containing combined pdf</returns>
        public static string MergePdfForms(List<string> file_names, string output) {
            

            if (file_names.Count > 1) {

                List<byte[]> files = new List<byte[]>();
                foreach (string file_name in file_names) {
                    var file = File.ReadAllBytes(file_name);
                    files.Add(file);
                }

                string[] names;
                PdfStamper stamper;
                MemoryStream msTemp = null;
                PdfReader pdfTemplate = null;
                PdfReader pdfFile;
                Document doc;
                PdfWriter pCopy;
                MemoryStream msOutput = new MemoryStream();

                pdfFile = new PdfReader(files[0]);

                doc = new Document();
                pCopy = new PdfSmartCopy(doc, msOutput);
                pCopy.PdfVersion = PdfWriter.VERSION_1_7;

                doc.Open();

                for (int k = 0; k < files.Count; k++) {
                    for (int i = 1; i < pdfFile.NumberOfPages + 1; i++) {
                        msTemp = new MemoryStream();
                        pdfTemplate = new PdfReader(files[k]);

                        stamper = new PdfStamper(pdfTemplate, msTemp);

                        names = new string[stamper.AcroFields.Fields.Keys.Count];
                        stamper.AcroFields.Fields.Keys.CopyTo(names, 0);
                        foreach (string name in names) {
                            stamper.AcroFields.RenameField(name, name + "_file" + k.ToString());
                        }

                        stamper.Close();
                        pdfFile = new PdfReader(msTemp.ToArray());
                        ((PdfSmartCopy)pCopy).AddPage(pCopy.GetImportedPage(pdfFile, i));
                        pCopy.FreeReader(pdfFile);
                    }
                }

                FileStream f = new FileStream(output, FileMode.Create);
                msOutput.WriteTo(f);
                msOutput.Flush();
                f.Flush();
                

                pdfFile.Close();
                pCopy.Close();
                doc.Close();
                msOutput.Close();
                f.Close();

                return output;
            }
            else if (file_names.Count == 1) {

                File.Copy(file_names.First(), output);
                return output;
            }

            return null;
        }
        public static response_item_type[] FillForm(pdf_stamper_request request,string mapping_root_path,string template_root_path, string output_root_path, DataTable data) {
            lock (_lock) {
                try {
                    List<Item> items_with_path = new List<Item>();
                    mappings mapping = new mappings();
                    if (File.Exists(mapping_root_path))
                        mapping = File.ReadAllText(mapping_root_path).DeserializeXml2<mappings>();

                    if (data.Rows.Count == 0) {
                        Logging.Singleton.WriteDebug("There is no data in the provided data table!");

                        foreach (var template in request.pdf_template_list) {
                            mapping_item_type selected = mapping.mapping_item.Where(p => p.pdf_template == template).First();

                            string source_document_path = selected.file_path.Replace("@root", template_root_path);
                            items_with_path.Add(new Item() { Path = source_document_path, PdfTemplate = template });
                        }
                        if (request.merge_output == true) {
                            string merged_document_path = Path.Combine(output_root_path, String.Format("{0}_{1}{2}", "merged", DateTime.Now.ToFileTimeUtc().ToString(), ".pdf"));

                            PdfMerge merge = new PdfMerge();
                            foreach (var item in items_with_path) {
                                merge.AddDocument(item.Path);
                            }
                            merge.EnablePagination = false;
                            merge.Merge(merged_document_path);
                            string result = Convert.ToBase64String(File.ReadAllBytes(merged_document_path));
                            return new response_item_type[] { new response_item_type() { pdf_template = template.MergedContent, data = result } };
                        }
                        else {
                            List<response_item_type> items = new List<response_item_type>();
                            foreach (var item in items_with_path) {
                                var temp = new response_item_type() { pdf_template = item.PdfTemplate, data = Convert.ToBase64String(File.ReadAllBytes(item.Path)) };
                                items.Add(temp);
                            }
                            return items.ToArray();
                        }
                    }
                    else {

                        DataRow row = data.Rows[0];

                        if (request.debug_mode) {
                            foreach (DataColumn column in data.Columns) {
                                Logging.Singleton.WriteDebugFormat("Data column [{0}] has a value [{1}]", column.ToString(), row[column].ToString());
                            }
                        }

                        foreach (var template in request.pdf_template_list) {
                            mapping_item_type selected = mapping.mapping_item.Where(p => p.pdf_template == template).First();

                            string source_document_path = selected.file_path.Replace("@root", template_root_path);
                            FileInfo f = new FileInfo(source_document_path);

                            string destination_document_path = Path.Combine(output_root_path,
                                String.Format("{0}_{1}{2}",
                                f.Name.Replace(f.Extension, ""),
                                DateTime.Now.ToFileTimeUtc().ToString(),
                                f.Extension)
                                );

                            items_with_path.Add(new Item() { Path = destination_document_path, PdfTemplate = template });

                            PdfReader reader = new PdfReader(source_document_path);
                            using (PdfStamper stamper = new PdfStamper(reader, new FileStream(destination_document_path, FileMode.Create))) {
                                AcroFields fields = stamper.AcroFields;

                                if (request.debug_mode) {
                                    foreach (var key in fields.Fields.Keys) {
                                        Logging.Singleton.WriteDebugFormat("Pdf field [{0}]. Data type [{1}]", key, fields.GetFieldType(key));
                                    }
                                }

                                foreach (var key in fields.Fields.Keys) {

                                    var items = selected.mapping.Where(p => p.data_field == key);
                                    if (items.Count() == 1) {
                                        var item = items.First();
                                        if (item.column_name == UNKNOWN)
                                            continue;
                                        if (data.Columns.Contains(item.column_name)) {
                                            string value = row[item.column_name].ToString();

                                            if (item.field_type == data_field_type.CheckBox) {
                                                value = value == "True" ? "Yes" : "No";
                                            }
                                            fields.SetField(key, value);
                                        }
                                        else {
                                            Logging.Singleton.WriteDebugFormat("Column {0} does not belong to table {1}! Check your mappings for template {2}", item.column_name, data.TableName, template);
                                        }
                                    }
                                    else if (items.Count() == 0) {

                                        var current = selected.mapping.ToList();

                                        data_field_type field_type = data_field_type.Text;
                                        if (key.Contains("Check"))
                                            field_type = data_field_type.CheckBox;
                                        else if (key.Contains("Radio"))
                                            field_type = data_field_type.RadioButton;

                                        current.Add(new mapping_type() { column_name = UNKNOWN, data_field = key, field_type = field_type });

                                        selected.mapping = current.ToArray();

                                        File.WriteAllText(mapping_root_path, mapping.SerializeXml());
                                    }
                                    else {
                                        throw new NotImplementedException();
                                    }
                                }
                                // flatten form fields and close document
                                if (request.read_only || (request.merge_output && request.pdf_template_list.Length > 1)) {
                                    Logging.Singleton.WriteDebugFormat("Form flattening requested... Read only {0}, Merge output {1}, Template list count {2}", request.read_only, request.merge_output, request.pdf_template_list.Length);
                                    stamper.FormFlattening = true;
                                }

                                stamper.Close();
                            }
                        }
                        if (items_with_path.Count() == 1) {
                            string path = items_with_path.First().Path;
                            string result = Convert.ToBase64String(File.ReadAllBytes(path));
                            return new response_item_type[] { new response_item_type() { pdf_template = items_with_path.First().PdfTemplate, data = result } };
                        }
                        else {
                            if (request.merge_output == true) {
                                string merged_document_path = Path.Combine(output_root_path, String.Format("{0}_{1}{2}", "merged", DateTime.Now.ToFileTimeUtc().ToString(), ".pdf"));

                                //List<string> file_names = new List<string>();
                                //foreach (var item in items_with_path) {
                                //    file_names.Add(item.Path);
                                //}
                                //var path = MergePdfForms(file_names, merged_document_path);

                                PdfMerge merge = new PdfMerge();
                                foreach (var item in items_with_path) {
                                    merge.AddDocument(item.Path);
                                }



                                merge.EnablePagination = false;
                                merge.Merge(merged_document_path);
                                //using (FileStream file = new FileStream(merged_document_path, FileMode.Create, System.IO.FileAccess.Write)) {
                                //    byte[] bytes = new byte[stream.Length];
                                //    stream.Read(bytes, 0, (int)stream.Length);
                                //    file.Write(bytes, 0, bytes.Length);
                                //    stream.Close();
                                //}

                                string result = Convert.ToBase64String(File.ReadAllBytes(merged_document_path));
                                return new response_item_type[] { new response_item_type() { pdf_template = template.MergedContent, data = result } };
                            }
                            else {
                                List<response_item_type> items = new List<response_item_type>();
                                foreach (var item in items_with_path) {
                                    var temp = new response_item_type() { pdf_template = item.PdfTemplate, data = Convert.ToBase64String(File.ReadAllBytes(item.Path)) };
                                    items.Add(temp);
                                }

                                return items.ToArray();
                            }
                        }
                    }                    
                }
                catch (Exception ex) {
                    string message = Logging.CreateExceptionMessage(ex);
                    Logging.Singleton.WriteDebug(message);
                    return null;
                }
            }
        }
    }
    public class Item
    {
        public string Path { get; set; }
        public template PdfTemplate { get; set; }
    }
}
