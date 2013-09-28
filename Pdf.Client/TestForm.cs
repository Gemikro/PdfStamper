using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Pdf.Client.ProcessXmlHandler;
using Pdf.Common;
using Pdf.Interfaces;
using Utilities;

namespace Pdf.Client
{
    public partial class TestForm : Form
    {
        public TestForm() {
            InitializeComponent();
            string root = Path.Combine(Application.StartupPath,"Logs");
            if(!Directory.Exists(root))
                Directory.CreateDirectory(root);
            Logging.LogFileName = Path.Combine(root, "PdfStamperClientLog");
        }

        private void button1_Click(object sender, EventArgs e) {
            try {
                ProcessXmlHandler.ProcessXmlHandler client = new ProcessXmlHandler.ProcessXmlHandler();
                pdf_stamper_request request = new pdf_stamper_request();

                request.pdf_template_list = new template[] { Interfaces.template.Obrazac1, Interfaces.template.Obrazac2};

                //string json = JsonConvert.SerializeObject(GetTestData(), new Serialization.DataTableConverter());

                request.data = GetTestData().SerializeXml();
                //request.data = json;
                request.read_only = false;
                request.debug_mode = false;
                request.merge_output = true;

                string request_string = request.SerializeXml();
                //string request_string = JsonConvert.SerializeObject(request);

                string response_string = client.ProcessRequest(request_string);
                pdf_stamper_response response = response_string.DeserializeXml2<pdf_stamper_response>();
                ProcessResponse(response);                
            }
            catch (Exception ex) {
                string message = Logging.CreateExceptionMessage(ex);
                Logging.Singleton.WriteDebug(message);
                MessageBox.Show(this,message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button2_Click(object sender, EventArgs e) {
            try {
                ProcessXmlHandler.ProcessXmlHandler client = new ProcessXmlHandler.ProcessXmlHandler();
                pdf_stamper_request request = new pdf_stamper_request();
                request.pdf_template_list = new template[] {Interfaces.template.Obrazac1};

                string json = JsonConvert.SerializeObject(GetTestData(), new Serialization.DataTableConverter());

                //request.data = GetTestData().SerializeXml();
                request.data = json;
                request.read_only = false;
                request.debug_mode = false;
                request.merge_output = false;

                //string request_string = request.SerializeXml();
                string request_string = JsonConvert.SerializeObject(request);

                string response_string = client.ProcessRequestJson(request_string);
                //pdf_stamper_response response = response_string.DeserializeXml2<pdf_stamper_response>();
                pdf_stamper_response response = JsonConvert.DeserializeObject<pdf_stamper_response>(response_string);
                ProcessResponse(response);
            }
            catch (Exception ex) {
                string message = Logging.CreateExceptionMessage(ex);
                Logging.Singleton.WriteDebug(message);
                MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void ProcessResponse(pdf_stamper_response response) {
            if (response.response_items != null && response.response_items.Count() > 0) {
                foreach (var item in response.response_items) {
                    SaveFileDialog d = new SaveFileDialog();
                    d.FileName = Path.Combine(@"D:\", string.Format("{0}.pdf", item.pdf_template));
                    d.Filter = "Pdf files|*.pdf";
                    var res = d.ShowDialog();
                    if (res == System.Windows.Forms.DialogResult.OK) {
                        string path = d.FileName;
                        File.WriteAllBytes(path, Convert.FromBase64String(item.data));

                        Process.Start(path);
                    }
                }
            }
        }
        private DataTable GetTestData() { 
            using(SqlConnection c = new SqlConnection(Properties.Settings.Default.Db))
            {
                SqlDataAdapter a = new SqlDataAdapter("[dbo].[grp_scg_registar1_tmp] @already_exported, @id_cont_bit, @id_cont_val", c);
                a.SelectCommand.Parameters.AddWithValue("@already_exported", false);
                a.SelectCommand.Parameters.AddWithValue("@id_cont_bit", true);
                a.SelectCommand.Parameters.AddWithValue("@id_cont_val", 26);
                DataTable data = new DataTable("TestData");
                a.Fill(data);
                return data;
            }
        }
    }
}
