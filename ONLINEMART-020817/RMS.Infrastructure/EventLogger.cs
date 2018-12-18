using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace HRMS.Infrastructure
{
   public class EventLogger
    {
        public static void WriteToErrorLog( Exception ex,string title)
        {
            string path = string.Empty;
          
            string uname = string.Empty;
            path = AppDomain.CurrentDomain.BaseDirectory + "/";
            if (!Directory.Exists(path + "\\Log\\"))
            {
                Directory.CreateDirectory(path + "\\Log\\");
            }

            string strDate = DateTime.Now.ToString("dd-MMM-yyyy");
            string strYear = DateTime.Now.ToString("yyyy");
            string strMonth = DateTime.Now.ToString("MMM");

            string fileYear = null, fileMonth = null, fileName = null;

            fileYear = path + "\\Log\\" + strYear;
            fileMonth = fileYear + "\\" + strMonth;

            if (!Directory.Exists(fileYear))
            {
                Directory.CreateDirectory(fileYear);
            }

            if (!Directory.Exists(fileMonth))
            {
                Directory.CreateDirectory(fileMonth);
            }

            fileName = fileMonth + "\\" + strDate + "_Log.xml";

            if (!File.Exists(fileName))
            {
                XmlTextWriter textWritter = new XmlTextWriter(fileName, null);
                textWritter.WriteStartDocument();
                textWritter.WriteStartElement("USERS");
                textWritter.WriteEndElement();
                textWritter.Close();
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);
            XmlElement subRoot = xmlDoc.CreateElement("User");

            CreateXmlElement(xmlDoc, subRoot, "Title", title);
            CreateXmlElement(xmlDoc, subRoot, "Message", ex.Message);
            CreateXmlElement(xmlDoc, subRoot, "InnerMessage", ex.InnerException != null ? ex.InnerException.Message : null);
            CreateXmlElement(xmlDoc, subRoot, "Source", ex.Source);
            CreateXmlElement(xmlDoc, subRoot, "StackTrace", ex.StackTrace);
            CreateXmlElement(xmlDoc, subRoot, "DateTime", DateTime.Now.ToString());
            CreateXmlElement(xmlDoc, subRoot, "LoginUserName", uname);

            xmlDoc.Save(fileName);
        }

        private static void CreateXmlElement(XmlDocument xmlDoc, XmlElement subRoot, string title, string message)
        {
            XmlElement appendedElement = xmlDoc.CreateElement(title);
            XmlText xmlText = xmlDoc.CreateTextNode(message);
            appendedElement.AppendChild(xmlText);
            subRoot.AppendChild(appendedElement);
            xmlDoc.DocumentElement.AppendChild(subRoot);
        }

        //public static void WriteToErrorLog(string path,Exception ex,string title,string uname)
        //{
        //    if (!Directory.Exists(path + "\\Log\\"))
        //    {
        //        Directory.CreateDirectory(path + "\\Log\\");
        //    }

        //    string strDate = DateTime.Now.ToString("dd-MMM-yyyy");
        //    string strYear = DateTime.Now.ToString("yyyy");
        //    string strMonth = DateTime.Now.ToString("MMM");

        //    string fileYear=null,fileMonth=null, fileName=null;

        //    fileYear = path + "\\Log\\" + strYear;
        //    fileMonth = fileYear+"\\" + strMonth;

        //    if (!Directory.Exists(fileYear))
        //    {
        //        Directory.CreateDirectory(fileYear);
        //    }

        //    if (!Directory.Exists(fileMonth))
        //    {
        //        Directory.CreateDirectory(fileMonth);
        //    }

        //    fileName = fileMonth+"\\" + strDate + "_Log.txt";

        //    FileStream errStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        //    StreamWriter errWriter = new StreamWriter(errStream);
        //    errWriter.Close();
        //    errStream.Close();
        //    FileStream errStream1 = new FileStream(fileName, FileMode.Append, FileAccess.Write);
        //    StreamWriter errWriter1 = new StreamWriter(errStream1);
        //    errWriter1.WriteLine("");
        //    errWriter1.WriteLine("Title: " + title);
        //    errWriter1.WriteLine("Message: " + ex.Message);
        //    errWriter1.WriteLine("Inner Message: " + ex.InnerException);
        //    errWriter1.WriteLine("Source: " + ex.Source);
        //    errWriter1.WriteLine("StackTrace: " + ex.StackTrace);
        //    errWriter1.WriteLine("Date/Time: " + DateTime.Now.ToString());
        //    errWriter1.WriteLine("Login User Name: " + uname);
        //    errWriter1.WriteLine("");
        //    errWriter1.WriteLine("===========================================================================================");
        //    errWriter1.Close();
        //    errStream1.Close();
        //}

        public static void WriteToServiceLog(string path, Dictionary<string, object> dicService, string title)
        {
            string method = null;
            DateTime startTime;
            DateTime endTime;
            long totalTime;

            string strStartTime = null;
            string strEndTime = null;

            if (!Directory.Exists(path + "\\ServiceLog\\"))
            {
                Directory.CreateDirectory(path + "\\ServiceLog\\");
            }

            string strDate = DateTime.Now.ToString("dd-MMM-yyyy");
            string strYear = DateTime.Now.ToString("yyyy");
            string strMonth = DateTime.Now.ToString("MMM");

            string fileYear = null, fileMonth = null, fileName = null;

            fileYear = path + "\\ServiceLog\\" + strYear;
            fileMonth = fileYear + "\\" + strMonth;

            if (!Directory.Exists(fileYear))
            {
                Directory.CreateDirectory(fileYear);
            }

            if (!Directory.Exists(fileMonth))
            {
                Directory.CreateDirectory(fileMonth);
            }

            fileName = fileMonth + "\\" + strDate + "_Log.xml";

            if (!File.Exists(fileName))
            {
                XmlTextWriter textWritter = new XmlTextWriter(fileName, null);
                textWritter.WriteStartDocument();
                textWritter.WriteStartElement("USERS");
                textWritter.WriteEndElement();
                textWritter.Close();
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);
            XmlElement subRoot = xmlDoc.CreateElement("User");

            method = dicService["MethodName"].ToString();
            startTime = Convert.ToDateTime(dicService["StartTime"]);
            endTime = Convert.ToDateTime(dicService["EndTime"]);
            totalTime = 0;

            if (startTime != null && endTime != null)
            {
                strStartTime = startTime.ToString("hh:mm:ss tt");
                strEndTime = endTime.ToString("hh:mm:ss tt");
                totalTime = endTime.TimeOfDay.Subtract(startTime.TimeOfDay).Seconds;
            }

            CreateXmlElement(xmlDoc, subRoot, "Title", title);
            CreateXmlElement(xmlDoc, subRoot, "Method", method);
            CreateXmlElement(xmlDoc, subRoot, "Date", strDate);
            CreateXmlElement(xmlDoc, subRoot, "StartTime", strStartTime);
            CreateXmlElement(xmlDoc, subRoot, "EndTime", strEndTime);
            CreateXmlElement(xmlDoc, subRoot, "TotalTime", totalTime.ToString() + " secs");
            xmlDoc.Save(fileName);
        }
    }
}
