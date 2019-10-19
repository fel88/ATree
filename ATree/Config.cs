using System.IO;
using System.Text;
using System.Xml.Linq;

namespace ATree
{
    public static class Config
    {

        #region fields
        public static bool QuickSaveOnClosing;
        public static bool QuickLoadOnStartup;
        #endregion

        public static void Save()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<root>");
            sb.AppendLine($"<setting name=\"{"quickSaveOnCLosing"}\" value=\"{QuickSaveOnClosing}\"/>");
            sb.AppendLine($"<setting name=\"{"quickLoadOnStartup"}\" value=\"{QuickLoadOnStartup}\"/>");
            sb.AppendLine("</root>");
            File.WriteAllText("config.xml", sb.ToString());
        }

        public static void Load()
        {
            if (!File.Exists("config.xml")) return;
            var doc = XDocument.Load("config.xml");
            foreach (var item in doc.Descendants("setting"))
            {
                var nm = item.Attribute("name").Value;
                var vl = item.Attribute("value").Value;
                switch (nm)
                {
                    case "quickSaveOnCLosing":
                        QuickSaveOnClosing = bool.Parse(vl);
                        break;
                    case "quickLoadOnStartup":
                        QuickLoadOnStartup = bool.Parse(vl);
                        break;
                }
            }
        }
    }
}

