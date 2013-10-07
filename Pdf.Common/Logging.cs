using System;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using System.Collections;
using System.Configuration;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Xml.Serialization;
using System.Data;
using System.IO;
using System.Data.SqlTypes;
using System.Security.Cryptography;
//using XmlValidator.Common.Database;
using System.Security.Principal;
using System.Linq;

namespace Utilities {

    /// <summary>
    /// This class handles creation and identification temporary directory.
    /// It also 
    /// </summary>
    public class TempDirManager
    {

        string temp_dir;
        bool clear_dir = false;

        /// <summary>
        /// Public read-only accessor for temporary directory.
        /// </summary>
        public string TempDir
        {
            get { return temp_dir; }
        }

        TempDirManager(string dir_name, bool clear_dir)
        {
            temp_dir = Path.GetTempPath() + dir_name;
            if (!temp_dir.EndsWith("\\")) temp_dir += "\\";
        }

        /// <summary> Static factory function. </summary>
        /// <param name="dir_name">Name of the directory - subdirectory of temporary directory.
        /// If this parameter is null, files will be created on root of user's temp directory.</param>
        /// <param name="clear_dir">This flag indicates that directory should be 
        /// cleared - all its files removed </param>
        /// <returns>Initialized temp dir manager object </returns>

        public static TempDirManager Create(string dir_name, bool clear_dir)
        {
            if (dir_name == null) dir_name = "";
            TempDirManager res = new TempDirManager(dir_name, clear_dir);
            res.PrepareTmpDir();
            return res;
        }

        /// <summary> Static factory function. No subdirectory is used.</summary>

        public static TempDirManager Create() { return Create(null, false); }

        /// <summary>
        /// This method initializes this object - prepares temporary directory....
        /// </summary>
        void PrepareTmpDir()
        {
            if (clear_dir)
            {
                if (Directory.Exists(temp_dir)) Directory.Delete(temp_dir, true);
                Directory.CreateDirectory(temp_dir);
            }
        }

        /// <summary>
        /// This method creates new unique file name and
        /// return the absolute path to it. No file is created.
        /// </summary>
        /// <returns>The absolute path to temporary file.</returns>
        public string GetTempFileName()
        {
            return this.TempDir + Guid.NewGuid().ToString() + ".tmp";
        }
    }
    ///////////////////////////////////////////////////////////////////////////////////
    /// <summary>This enumeration lists all severity levels for messages.
    /// A message will be written to log if global filter level is
    /// greater or equal to the message level.
    /// </summary>
    public enum LogMsgLevel {
        /// <summary> No messages will be filtered from log (all will be written). </summary>
        Debug = 0,
        /// <summary> Only important messages will be written to log </summary>
        Info = 1,
        /// <summary> Only really important messages will be written to log </summary>
        Important = 2,
        /// <summary> All messages will be filtered from log (none will be written) </summary>
        None = 3
    }

    /// <summary>
    /// Interface for LogMsgLevel - used by application
    /// </summary>
    public interface ILogMsgLevelProvider {
        LogMsgLevel Level { get; set; }
    }

    /// <summary>
    /// Memory-based logger.
    /// </summary>
    internal class LogMsgLevelProviderMemory : ILogMsgLevelProvider {

        LogMsgLevel level = LogMsgLevel.Debug;

        #region ILogMsgLevelProvider Members

        LogMsgLevel ILogMsgLevelProvider.Level {
            get {
                return level;
            }
            set {
                level = value;
            }
        }

        #endregion
    }

    ////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Public singleton class that implements writing of messages to a file and to eventlog.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////

    public sealed class Logging {

        public static string CreateExceptionMessage(Exception ex) {
            Exception innerException = ex.InnerException;
            string message = String.Format("An error occured during processing!\nException: {0}\nStack Trace: {1}\n{2}",
                ex.Message, ex.StackTrace, innerException != null ? String.Format("Inner Exception: {0}", innerException.Message) : String.Empty);
            return message;
        }
        #region members

        
        /// <summary> This member counts number of skipped log file size checks </summary>
        int file_size_check_counter = 0;

        /// <summary> This member stores log file limit (in MB) </summary>
        long file_size_limit = 20;

        /// <summary>
        ///Public accessor for internal member + calculates number
        ///of bytes for specified MB
        /// </summary>
        public long FileSizeLimit { get { return file_size_limit * 1024 * 1024; } }

        /// <summary>Template of the file name. It is read from the configuration file
        /// (section configuration/appSettings, value LogFile) </summary>

        string file_name_template;

        /// <summary>Name of the current file to which the messages are written to.
        /// It is always constructed as &lt;file_name_template&gt;yyyy_mm_dd.dbg</summary>

        string file_name_current;

        /// <summary>Time when file_name_current was computed. Used for detecting
        /// when file_name_current should be recomputed. We set it to yesterdy to force creation of new file. </summary>

        DateTime file_creation_time = DateTime.Today.AddDays(-1);

        /// <summary>If this value is true, then debug messges are not written to file.
        /// If appropriate registry key doesn't exist, this value is set to false,
        /// otherwise it is true.</summary>

        bool dummy;

        #endregion

        public void RefreshDbSettings() {
            //db = UBSExtendedEntities.Create();
        }
        /// <summary>Recalculates file_name_current and updates file_creation_time.
        /// It is normally called at system start-up and when date changes.</summary>

        void CreateNewFileName() {
            file_creation_time = DateTime.Now;

            // check if target directory needs to be created
            string dir_path = Path.GetDirectoryName(file_name_template);
            if (dir_path.Trim() != "") {
                if (!Directory.Exists(dir_path))
                    Directory.CreateDirectory(dir_path);
            }

            string file_name = file_name_template + file_creation_time.ToString("yyyy_MM_dd") + "_";
            int i = 1;
            while (File.Exists(file_name + i + ".dbg") && (new FileInfo(file_name + i + ".dbg")).Length > FileSizeLimit) {
                FileInfo fi = new FileInfo(file_name + i + ".dbg");
                if (fi.Length < FileSizeLimit) break;
                i++;
            }

            file_name_current = file_name + i + ".dbg";

            // write message to file
            if (!File.Exists(file_name_current)) {
                // completely new file is generated - write header line
                StreamWriter sw = new StreamWriter(file_name_current, true,Encoding.Unicode);
                try { sw.WriteLine("Time\tThread ID\tOrigin\tGroup\tUser\tSession_id\tMessage"); }
                finally { sw.Close(); };
            }
        }

        /// <summary>This method tests if date changed since the time when
        /// file_name_current was last computed.</summary>

        bool DoWeNeedNewLogFile() {
            // if current file doesn't exist, we need new file
            if (!File.Exists(file_name_current)) return true;

            // if date has changed, switch
            if (DateTime.Now.Day != file_creation_time.Day) return true;

            // check if file-size limit is reached
            file_size_check_counter++;
            if (file_size_check_counter > 20) {
                file_size_check_counter = 0;
                FileInfo fi = new FileInfo(file_name_current);
                if (fi.Length > FileSizeLimit)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Default constructor.<br/>
        /// If file name is not provided in the configuration, this object
        /// goes into dummy mode for debug messages.<br/>
        /// It also sets up event log object for output of error messages into event log.
        /// </summary>

        Logging() {
            file_name_template = LogFileName;
            dummy = (file_name_template == null);
            if (!dummy) {
                CreateNewFileName();
            }
        }

        /// <summary>Writes provided information into log file. Message is slightly
        /// transformed before it is written to the disk - new line and tab characters
        /// are replaced with ' '. This way we produce
        /// valid tab-separated files, which can be viewed in e.g. Excel. Thread-safe.</summary>
        /// <param name="msg">Text of the message</param>
        /// <param name="level">Level of message</param>

        void WriteToFile(string msg, LogMsgLevel level) {
            if (dummy) return;

            if (level == LogMsgLevel.None)
                throw new Exception("Illegal log message level - All.");
            LogMsgLevel m_level = CurLevelProvider.Level;
            if (level < m_level)
                return;

            lock (this) {
                if (DoWeNeedNewLogFile()) CreateNewFileName();

                // replace tabs and newlines with spaces
                msg = msg.Replace(Convert.ToChar(10), ' ');
                msg = msg.Replace(Convert.ToChar(13), ' ');
                msg = msg.Replace('\t', ' ');

                // find user and session descriptors if present
                string user = WindowsIdentity.GetCurrent().Name;
                string session = "";
                if (msg[0] == '[') {
                    int i = msg.IndexOf(']');
                    user = msg.Substring(0, i + 1);
                    msg = msg.Substring(i + 1);
                    if (msg[0] == '[') {
                        int j = msg.IndexOf(']');
                        session = msg.Substring(0, j + 1);
                        msg = msg.Substring(j + 1);
                    }
                }

                // create message string
                string s = String.Format(
                    "{0}:{1}\t{2}\t{3}\t{4}\t{5}",
                    DateTime.Now.ToString(),
                    DateTime.Now.Millisecond,
                    System.Threading.Thread.CurrentThread.ManagedThreadId,
                    user,
                    session,
                    msg);

                //write message to DB
                try
                {
                    //using (UBSEntities db = UBSExtendedEntities.Create()) {
                    //    AuditLog log = new AuditLog();
                    //    log.Application = "PURS_UBS Handler";
                    //    log.LogTime = DateTime.Now;
                    //    log.Message = msg;
                    //    log.Severity = (int)level;
                    //    log.Username = user;

                    //    db.AddToAuditLog(log);
                    //    db.SaveChanges();
                    //}
                }
                catch (Exception ex)
                {
                    Exception innerException = ex.InnerException;
                    string message = String.Format("An error occured during processing!\nException: {0}\nStack Trace: {1}\nInner Exception: {2}",
                        ex.Message, ex.StackTrace, innerException != null ? innerException.Message : String.Empty);

                    //Logging.Singleton.WriteDebug(message);
                }
                // write message to file
                StreamWriter sw = new StreamWriter(file_name_current, true,Encoding.Unicode); // append to file
                try {
                    sw.WriteLine(s);
                }
                finally {
                    sw.Close();
                };
            }
        }

        #region Client methods for writing messages to log


        private StringBuilder ListStack(out string sType)
        {
            sType = string.Empty;
            return new StringBuilder();
            //ON RLRSWEBAAP1 this is not allowed!!!
            /*
            StringBuilder sb = new StringBuilder();
            sType = "";

            StackTrace st = new StackTrace(true);
            foreach (StackFrame f in st.GetFrames())
            {
                MethodBase m = f.GetMethod();
                if (f.GetFileName() != null)
                {
                    sb.AppendLine(string.Format("{0}:{1} {2}.{3}",
                      f.GetFileName(), f.GetFileLineNumber(),
                      m.DeclaringType.FullName, m.Name));

                    if (!string.IsNullOrEmpty(m.DeclaringType.Name))
                        sType = m.DeclaringType.Name;
                }
            }

            return sb;
            */
        }

        /// <summary>Writes debug message into log file - if one is specified.</summary>
        /// <param name="msg">Text of the message.</param>
        /// <param name="origin">Name of the module/program part/method/class where this
        /// message was reported.</param>
        /// <param name="level">Level of message</param>

        void WriteMsg(string msg, LogMsgLevel level) {
            //Trace.WriteLine(msg);            
            WriteToFile(msg, level);
        }

        /// <summary>Writes debug message into log file with level None.</summary>
        /// <param name="msg">Text of the message.</param>
        public void WriteDebug(string msg) {            
            string stack = "";
            ListStack(out stack);

            WriteMsg(string.Format("{0}{1}", msg, IsNullOrEmpty(stack) ? String.Empty : String.Format("-{0}", stack)), LogMsgLevel.Debug);          
            
        }

        /// <summary>Writes debug message into log file with level None.</summary>
        /// <param name="msg">Text of the message.</param>
        /// <param name="args">Arguments to be inserted into string.</param>
        public void WriteDebugFormat(string msg, params object[] args) {
            WriteDebug(string.Format(msg, args));
        }

        /// <summary>Writes debug message into log file with level None.</summary>
        /// <param name="msg">Text of the message.</param>
        public void WriteInfo(string msg) {
            string stack = "";
            ListStack(out stack);

            WriteMsg(string.Format("{0}{1}", msg, IsNullOrEmpty(stack) ? String.Empty : String.Format("-{0}", stack)), LogMsgLevel.Info);          
        }

        /// <summary>Writes debug message into log file with level None.</summary>
        /// <param name="msg">Text of the message.</param>
        /// <param name="args">Arguments to be inserted into string.</param>
        public void WriteInfoFormat(string msg, params object[] args) {
            WriteInfo(string.Format(msg, args));
        }

        /// <summary>Writes debug message into log file with level None.</summary>
        /// <param name="msg">Text of the message.</param>
        public void WriteImportant(string msg) {
            string stack = "";
            ListStack(out stack);

            WriteMsg(string.Format("{0}{1}", msg, IsNullOrEmpty(stack)?  String.Empty:String.Format("-{0}", stack)), LogMsgLevel.Important);          
        }

        private static bool IsNullOrEmpty(string stack) {
            return stack == String.Empty || stack == null;
        }

        /// <summary>Writes debug message into log file with level None.</summary>
        /// <param name="msg">Text of the message.</param>
        /// <param name="args">Arguments to be inserted into string.</param>
        public void WriteImportantFormat(string msg, params object[] args) {
            WriteImportant(string.Format(msg, args));
        }

        #endregion

        #region singleton

        /// <summary>Singleton design pattern.</summary>
        public static Logging Singleton = CreateDefaultLogger();

        /// <summary> Initializer for singleton </summary>
        /// <returns> initalized singleton </returns>
        static Logging CreateDefaultLogger() {

            Logging l = new Logging();
            PurgeLogDirectory();
            Singleton = l;            
            return l;
        }

        private static void PurgeLogDirectory() {
            if (log_file_name != null) {
                FileInfo f = new FileInfo(log_file_name);
                var files = f.Directory.GetFiles("*.dbg", SearchOption.TopDirectoryOnly);

                var toDelete = from x in files
                               where x.CreationTime <= DateTime.Today.AddDays(-DateTime.Today.Day)
                               select x;

                foreach (var file in toDelete) {

                    File.Delete(file.FullName);
                }
            }
        }
        /// <summary>
        /// Current logging file
        /// </summary>
        private static string log_file_name = null;

        /// <summary> Current logging file - when set it resets logging singleton </summary>
        public static string LogFileName {
            get { return log_file_name; }
            set {
                log_file_name = value;
                CreateDefaultLogger();
            }
        }

        /// <summary> Limit of log file size </summary>
        public static int LogFileLimit = -1;

        /// <summary> Current level of logging filter - can be set in runtime</summary>
        public static ILogMsgLevelProvider CurLevelProvider = new LogMsgLevelProviderMemory();

        #endregion
    }
}
