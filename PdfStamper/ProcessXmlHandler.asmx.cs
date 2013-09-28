using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Pdf.Interfaces;
using Pdf.Common;
using System.IO;
using System.Data;
using Newtonsoft.Json;
using Utilities;

namespace PdfStamper
{
    /// <summary>
    /// Summary description for ProcessXmlHandler
    /// </summary>
    [WebService(Namespace = "http://www.gemikro.rs")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ProcessXmlHandler : System.Web.Services.WebService
    {

        [WebMethod]
        public string ProcessRequestJson(string request_string) {
            try {
                //pdf_stamper_request request = request_string.DeserializeXml2<pdf_stamper_request>();
                pdf_stamper_request request = JsonConvert.DeserializeObject<pdf_stamper_request>(request_string);
                pdf_stamper_response response = new pdf_stamper_response();

                //ovo je neophodno samo inicijalno, da se napravi mapping za sve fajlove
                //PdfFormEditor.InitMappings(Global.TemplateRootPath, Global.MappingRootPath, Global.OutputRootPath);


                DataTable data = JsonConvert.DeserializeObject<DataTable>(request.data, new Serialization.DataTableConverter());
                //DataTable data = request.data.DeserializeXml2<DataTable>();

                response.response_items = PdfFormEditor.FillForm(request, Global.MappingRootPath, Global.TemplateRootPath, Global.OutputRootPath, data);
                //string response_string = response.SerializeXml();
                string response_string = JsonConvert.SerializeObject(response);
                return response_string;
            }
            catch (Exception ex) { 
                //logging
                string message = Logging.CreateExceptionMessage(ex);
                Logging.Singleton.WriteDebug(message);
                return "Error";
            }
        }

        [WebMethod]
        public string ProcessRequest(string request_string) {
            try {
                Logging.Singleton.WriteDebug(request_string);
                pdf_stamper_request request = request_string.DeserializeXml2<pdf_stamper_request>();
                //pdf_stamper_request request = JsonConvert.DeserializeObject<pdf_stamper_request>(request_string);
                pdf_stamper_response response = new pdf_stamper_response();

                //ovo je neophodno samo inicijalno, da se napravi mapping za sve fajlove
                //PdfFormEditor.InitMappings(Global.TemplateRootPath, Global.MappingRootPath, Global.OutputRootPath);



                DataTable data = request.data.DeserializeXml2<DataTable>();

                response.response_items = PdfFormEditor.FillForm(request, Global.MappingRootPath, Global.TemplateRootPath, Global.OutputRootPath, data);
                string response_string = response.SerializeXml();
                Logging.Singleton.WriteDebug(response_string);
                //string response_string = JsonConvert.SerializeObject(response);
                return response_string;
            }
            catch (Exception ex) {
                //logging
                string message = Logging.CreateExceptionMessage(ex);
                Logging.Singleton.WriteDebug(message);
                return "Error";
            }
        }
    }
}
