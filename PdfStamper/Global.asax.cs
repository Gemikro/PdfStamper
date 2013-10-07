using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Utilities;

namespace PdfStamper
{
    public class Global : System.Web.HttpApplication
    {
        public static string MappingRootPath { get; set; }
        public static string TemplateRootPath { get; set; }
        public static string OutputRootPath { get; set; }
        public static string FontsRootPath { get; set; }

        protected void Application_Start(object sender, EventArgs e) {

            MappingRootPath = Path.Combine(Server.MapPath("~/Mappings"),"mapping.xml");
            TemplateRootPath = Server.MapPath("~/Templates");
            OutputRootPath = Server.MapPath("~/Output");
            FontsRootPath = Server.MapPath("~/Fonts");
            Logging.LogFileName = Path.Combine(Server.MapPath("~/Logs"), "PdfStamperLog");

            //string[] files = Directory.GetFiles(TemplateRootPath, "*.pdf");
            //string script_template_path = Path.Combine(TemplateRootPath, "Script_template.vbs");

            //foreach (string file in files) {
            //    string script = File.ReadAllText(script_template_path);
            //    FileInfo f = new FileInfo(file);
            //    //string output_root = Path.Combine(TemplateRootPath, "Template with tags");
            //    //string output_root = TemplateRootPath;
            //    //string output_path = Path.Combine(output_root, String.Format("{0}", f.Name));

            //    string input_path = String.Format("{0}", f.Name);
            //    string output_path = String.Format("Template_{0}", f.Name);
            //    //string output_path = Path.Combine(output_root, String.Format("Template_{0}",f.Name));
            //    string script_path = Path.Combine(TemplateRootPath, String.Format("{0}_Script.vbs", f.Name));

            //    script = script.Replace("@input", input_path);
            //    script = script.Replace("@output", output_path);

            //    File.WriteAllText(script_path, script);

            //    //Process.Start(script_path);
            //}

            Logging.Singleton.WriteDebug("Application started!");
            
        }

        protected void Session_Start(object sender, EventArgs e) {

        }

        protected void Application_BeginRequest(object sender, EventArgs e) {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e) {

        }

        protected void Application_Error(object sender, EventArgs e) {
            Logging.Singleton.WriteDebug("Application started!");

            System.Web.HttpContext context = HttpContext.Current;
            System.Exception ex = Context.Server.GetLastError();

            Logging.Singleton.WriteDebug(Logging.CreateExceptionMessage(ex));

            context.Server.ClearError();            
        }

        protected void Session_End(object sender, EventArgs e) {

        }

        protected void Application_End(object sender, EventArgs e) {

        }
    }
}