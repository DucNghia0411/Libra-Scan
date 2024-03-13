using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.ApiIntergration.Ultilities
{
    public class SettingUrl
    {
        public static Dictionary<string, string> GetPort()
        {
            string fileName = "..\\..\\..\\..\\..\\Ports\\ScanPort.txt";
            //string currentPath = Directory.GetCurrentDirectory() + fileName;
            Dictionary<string, string> MySettings = File
                .ReadLines(fileName)
                .ToDictionary(line => line.Substring(0, line.IndexOf('=')).Trim(),
                line => line.Substring(line.IndexOf('=') + 1).Trim());
            return MySettings;
        }
        public static string GetAddress()
        {
            //Dictionary<string, string> settings = GetPort();
            //return settings["http"] + "://" + settings["domain"] + ":" + settings["port"] + "/";
            return "http://localhost:9090";
        }
    }
}
