using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace RSD {
    abstract class GameServer {
        private int id;
        private string path;
        private int port;
        private string executeable;
        private Process process;

        private string cleanGameDirectory;
        private FTPData ftpData;

        public abstract bool Start(out List<string> messages);
        public abstract bool Stop(out List<string> messages);

        private PerformanceCounter ramCounter;
        private PerformanceCounter cpuCounter;

        private List<string> performanceWarnings;

        public GameServer(int id, int port, string path, string executeable, FTPData ftpData) {
            this.id = id;
            this.port = port;
            this.path = path;
            this.executeable = executeable;
            this.ftpData = ftpData;

            performanceWarnings = new List<string>();
        }

        public int Id {
            get { return id; }
            set { id = value; }
        }

        public int Port {
            get { return port; }
            set { port = value; }
        }

        public List<string> PerformanceWarnings {
            get { return performanceWarnings; }
            set { performanceWarnings = value; }
        }

        public string GamePath {
            get { return path; }
            set { path = value; }
        }

        public string Executeable {
            get { return executeable; }
            set { executeable = value; }
        }

        public PerformanceCounter RamCounter {
            get { return ramCounter; }
            set { ramCounter = value; }
        }

        public PerformanceCounter CpuCounter {
            get { return cpuCounter; }
            set { cpuCounter = value; }
        }

        public string CleanGameDirectory {
            get { return cleanGameDirectory; }
            set { cleanGameDirectory = value; }
        }

        public Process Process {
            get { return process; }
            set { process = value; }
        }

        public bool Reinstall(out List<string> messages) {
            messages = new List<string>();

            if (IsActive()) {
                messages.Add("This server is active.. shutting down server in 3 seconds");
                StopProcess();

                System.Timers.Timer reinstallTimer = new System.Timers.Timer(3000);
                reinstallTimer.Start();
                reinstallTimer.Elapsed += new ElapsedEventHandler(delegate (object s, ElapsedEventArgs ev)
                {
                    ExecuteReinstall();
                    reinstallTimer.Enabled = false;
                });
            }
            else {
                ExecuteReinstall();
                messages.Add("Reinstalling server.");
            }
            return true;
        }

        private bool ExecuteReinstall() {
            Utility.DirectoryClear(path);
            Utility.DirectoryCopy(cleanGameDirectory, path, true);
            return true;
        }

        protected bool StartProcess() {
            process = new Process();

            process.StartInfo.FileName = path + executeable;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(path);
            process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;

            process.EnableRaisingEvents = true;
            process.Exited += ExitHandler;

            // Executeable doesn't exist
            if (!File.Exists(process.StartInfo.FileName))
                return false;

            if(process.Start()) {
                ramCounter = new PerformanceCounter("Process", "Working Set", Utility.GetProcessInstanceName(Process.Id));
                cpuCounter = new PerformanceCounter("Process", "% Processor Time", Utility.GetProcessInstanceName(Process.Id));
                performanceWarnings = new List<string>();

                return true;
            }
            return false;
        }

        protected bool StopProcess() {
            if(process != null) {
                process.Kill();
                process = null;

                ramCounter.Dispose();
                cpuCounter.Dispose();
                ramCounter = null;
                cpuCounter = null;

                return true;
            }            
            return false;
        }

        private void ExitHandler(object sender, EventArgs e) {
            foreach(GameServer gameServer in Server.GameServers) {
                if(gameServer.Id == this.id) {
                    gameServer.Process = null;

                    ramCounter.Dispose();
                    cpuCounter.Dispose();
                    ramCounter = null;
                    cpuCounter = null;
                }
            }
        }

        public bool IsActive() {
            try {
                Process.GetProcessById(process.Id);
            }
            catch (Exception) {
                return false;
            }
            return true;
        }

        public FTPData FTPData {
            get { return ftpData; }
        }

        public bool GenerateFTP(out List<string> messages) {
            messages = new List<string>();

            string password = FileZilla.GeneratePassword(this);

            if (password != null) {
                messages.Add("New FTP password: " + password);
                return true;
            }
            else {
                messages.Add("Could not generate password");
                return false;
            }
        }
    }
}
