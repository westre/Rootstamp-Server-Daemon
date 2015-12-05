using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RSD {
    class SAMPServer : GameServer {
        private int packageId;

        public SAMPServer(int id, int port, string path, string executeable, FTPData ftpData, int packageId) : base(id, port, path, executeable, ftpData) {
            this.packageId = packageId;
        }

        public override bool Start(out List<string> messages) {
            messages = new List<string>();

            if (IsActive()) {
                messages.Add("This server is already active!");
                return false;
            }

            string directoryName = Path.GetFileName(Path.GetDirectoryName(GamePath));
            string[] configFile = File.ReadAllLines(Path.GetDirectoryName(GamePath) + "/server.cfg");

            bool isValid = true;

            foreach (string line in configFile) {
                if (line.Trim().StartsWith("port")) {
                    if (line.Trim().Contains(base.Port.ToString())) {
                        messages.Add("Port check success");
                    }
                    else {
                        messages.Add("Invalid port, it requires to be port " + Port);
                        isValid = false;
                    }
                }

                if (line.Trim().StartsWith("maxplayers")) {
                    int players = Convert.ToInt32(Regex.Replace(line, "[^0-9]+", string.Empty));
                    if ((players <= 30 && packageId == 1) || (players <= 50 && packageId == 2)) {
                        messages.Add("Maximum players check success");
                    }
                    else {
                        if (packageId == 1)
                            messages.Add("Invalid max players, make sure it's 30 or below! If you wish to have more, upgrade to the plus package.");
                        else if (packageId == 2)
                            messages.Add("Invalid max players, make sure it's 50 or below!");

                        isValid = false;
                    }
                }

                if (line.Trim().StartsWith("language")) {
                    if (packageId == 1) {
                        if (line.Trim().Contains("rootstamp.com")) {
                            messages.Add("Language check success");
                        }
                        else {
                            messages.Add("Invalid language, please make sure rootstamp.com is in the field!");
                            isValid = false;
                        }
                    }
                }
            }
            
            if(isValid) {
                if(StartProcess()) {
                    messages.Add("Server started");
                    return true;
                }
                else {
                    messages.Add("Server failed to start");
                }
            }

            return false;
        }

        public override bool Stop(out List<string> messages) {
            messages = new List<string>();

            if (StopProcess()) {
                messages.Add("Server stopped");
                return true;
            }
            else {
                messages.Add("Server failed to stop");
            }

            return false;
        }

        public int PackageId {
            get { return packageId; }
            set { packageId = value; }
        }
    }
}
