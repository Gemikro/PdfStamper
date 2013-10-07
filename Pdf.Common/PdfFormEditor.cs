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
        public static response_item_type[] FillForm(pdf_stamper_request request,string mapping_root_path,string template_root_path, string output_root_path, DataTable data, string fonts_root_path) {
            lock (_lock) {
                try {
                    List<Item> items_with_path = new List<Item>();
                    mappings mapping = new mappings();
                    if (File.Exists(mapping_root_path))
                        mapping = File.ReadAllText(mapping_root_path).DeserializeXml2<mappings>();

                    FileInfo mapping_path  = new FileInfo(mapping_root_path);
                    string fox_helper_path = Path.Combine(mapping_path.DirectoryName, "Fox.txt");
                    if (!File.Exists(fox_helper_path)) {
                        StringBuilder b = new StringBuilder();
                        b.Append(@"
DIMENSION laArray[30,2]
laArray[1,1] = 'Obrazac1'
laArray[2,1] = 'Obrazac2'
laArray[3,1] = 'Obrazac3'
laArray[4,1] = 'Obrazac4'
laArray[5,1] = 'Obrazac5'
laArray[6,1] = 'Obrazac6'
laArray[7,1] = 'Obrazac7'
laArray[8,1] = 'Obrazac8'
laArray[9,1] = 'Obrazac9'
laArray[10,1] ='Obrazac10'
laArray[11,1] = 'Obrazac11'
laArray[12,1] = 'Obrazac12'
laArray[13,1] = 'Obrazac13'
laArray[14,1] = 'Obrazac14'
laArray[15,1] = 'Obrazac15'
laArray[16,1] = 'Obrazac16'
laArray[17,1] = 'Obrazac17'
laArray[18,1] = 'Obrazac18'
laArray[19,1] = 'Obrazac19'
laArray[20,1] ='Obrazac20'
laArray[21,1] = 'Obrazac21'
laArray[22,1] = 'Obrazac22'
laArray[23,1] = 'Obrazac23'
laArray[24,1] = 'Obrazac24'
laArray[25,1] = 'Obrazac25'
laArray[26,1] = 'Obrazac26'
laArray[27,1] = 'Obrazac27'
laArray[28,1] = 'Obrazac28'
laArray[29,1] = 'Obrazac29'
laArray[30,1] ='Obrazac30'
");
                        int current_index = -1;
                        foreach (var item in mapping.mapping_item) {
                            current_index = Int32.Parse(item.pdf_template.ToString().Replace("Obrazac", ""));
                            string source_document_path = item.file_path.Replace("@root", template_root_path);
                            FileInfo info = new FileInfo(source_document_path);
                            string value = string.Format("laArray[{0},2] = '{1}'", current_index, info.Name.Replace(info.Extension,String.Empty));
                            b.AppendLine(value);                            
                        }
                        File.WriteAllText(fox_helper_path,b.ToString());                            
                    }
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
                        string id_pog = string.Empty;
                        if (data.Columns.Contains("id_pog"))
                            id_pog = row["id_pog"].ToString();

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
                                String.Format("{0}_{1}_{2}{3}",
                                id_pog.Replace("/","-").Trim(),
                                f.Name.Replace(f.Extension, ""),
                                DateTime.Now.ToFileTimeUtc().ToString(),
                                f.Extension)
                                );

                            items_with_path.Add(new Item() { Path = destination_document_path, PdfTemplate = template });

                            PdfReader reader = new PdfReader(source_document_path);
                            using (PdfStamper stamper = new PdfStamper(reader, new FileStream(destination_document_path, FileMode.Create))) {
                                AcroFields fields = stamper.AcroFields;


                                //Full path to the Unicode Arial file
                                //string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIALUNI.TTF");

                                //Create a base font object making sure to specify IDENTITY-H
                                //BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

                                //Create a specific font object
                                //Font f = new Font(bf, 12, Font.NORMAL);
                                iTextSharp.text.pdf.BaseFont baseFont  = iTextSharp.text.pdf.BaseFont.CreateFont(Path.Combine(fonts_root_path,"arial.ttf"),"Windows-1250", true);

                                fields.AddSubstitutionFont(baseFont);
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
                                                int int_value = 0;
                                                bool boolean_value = false;
                                                if(Int32.TryParse(value, out int_value))
                                                {
                                                    value = int_value == 0? "No" : "Yes";
                                                }
                                                else if (Boolean.TryParse(value, out boolean_value))
                                                {
                                                    value = boolean_value == false? "No" : "Yes";
                                                }
                                                else
                                                {
                                                    throw new NotImplementedException(string.Format("Invalid Value [{0}] was provided for Check box type field!", value));
                                                }
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
                            var bytes = File.ReadAllBytes(path);
                            string result = Convert.ToBase64String(bytes);
                            Logging.Singleton.WriteDebugFormat("Response lenght is {0} bytes", bytes.Length);
                            return new response_item_type[] { new response_item_type() { pdf_template = items_with_path.First().PdfTemplate, data = result } };
                            //return new response_item_type[] { new response_item_type() { pdf_template = items_with_path.First().PdfTemplate, data = Convert.ToBase64String(new byte[] {1,2,3,4,5,6,7,8,9}) } };
                        }
                        else {
                            if (request.merge_output == true) {
                                string merged_document_path = Path.Combine(output_root_path, String.Format("{0}_{1}{2}{3}", id_pog.Replace("/","-").Trim(), "merged", DateTime.Now.ToFileTimeUtc().ToString(), ".pdf"));

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

                                var bytes = File.ReadAllBytes(merged_document_path);
                                string result = Convert.ToBase64String(bytes);
                                Logging.Singleton.WriteDebugFormat("Response lenght is {0} bytes", bytes.Length);                                
                                return new response_item_type[] { new response_item_type() { pdf_template = template.MergedContent, data = result } };
                            }
                            else {
                                List<response_item_type> items = new List<response_item_type>();
                                foreach (var item in items_with_path) {
                                    var bytes = File.ReadAllBytes(item.Path);
                                    string result = Convert.ToBase64String(bytes);
                                    Logging.Singleton.WriteDebugFormat("Response lenght is {0} bytes", bytes.Length);                                
                                    var temp = new response_item_type() { pdf_template = item.PdfTemplate, data = result };
                                    //var temp = new response_item_type() { pdf_template = item.PdfTemplate, data = Convert.ToBase64String(new byte[] {1,2,3,4,5,6,7,8,9}) };
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
