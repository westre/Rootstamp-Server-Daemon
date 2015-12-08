using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RSD {
    class Utility {
        public static void DirectoryClear(string path) {
            DirectoryInfo directory = new DirectoryInfo(path);

            foreach (FileInfo file in directory.GetFiles()) {
                file.Delete();
            }

            foreach (DirectoryInfo dir in directory.GetDirectories()) {
                dir.Delete(true);
            }
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs) {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists) {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            }

            if (!Directory.Exists(destDirName)) {
                Directory.CreateDirectory(destDirName);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files) {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            if (copySubDirs) {
                foreach (DirectoryInfo subdir in dirs) {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public static string GetExternalIP() {
            using (WebClient client = new WebClient()) {
                try {
                    string ip = client.DownloadString("http://canihazip.com/s");
                    if (ip == "77.174.16.28")
                        return "176.31.253.42";

                    return ip;
                }
                catch (WebException) {
                    // this one is offline
                }

                try {
                    return client.DownloadString("http://wtfismyip.com/text");
                }
                catch (WebException) {
                    // offline...
                }

                try {
                    return client.DownloadString("http://ip.telize.com/");
                }
                catch (WebException) {
                    // offline too...
                }

                // if we got here, all the websites are down, which is unlikely
                return "Check internet connection?";
            }
        }

        public static string GetProcessInstanceName(int pid) {
            PerformanceCounterCategory cat = new PerformanceCounterCategory("Process");
            string[] instances = cat.GetInstanceNames();

            foreach (string instance in instances) {
                using (PerformanceCounter cnt = new PerformanceCounter("Process", "ID Process", instance, true)) {
                    int val = (int)cnt.RawValue;
                    if (val == pid) {
                        return instance;
                    }
                }
            }
            return "";
        }
    }
}
