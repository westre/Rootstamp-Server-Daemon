using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RSD {
    class PerformanceMonitor {
        private Thread performanceThread;

        public delegate void OnPerformanceUpdate(GameServer server, double ram, double cpu);
        public event OnPerformanceUpdate OnPerformanceTick;

        public PerformanceMonitor() {
            performanceThread = new Thread(new ThreadStart(PerformanceListener));
            performanceThread.IsBackground = true;
            performanceThread.Start();
        }

        private void PerformanceListener() {
            while (true) {
                try {
                    Thread.Sleep(1000);
                    foreach(GameServer gameServer in Server.gameServers) {
                        if (gameServer.IsActive() && gameServer.RamCounter != null && gameServer.CpuCounter != null) {
                            double ram = gameServer.RamCounter.NextValue();
                            double cpu = gameServer.CpuCounter.NextValue();
                            //Console.WriteLine("RAM: " + (ram / 1024 / 1024) + " MB; CPU: " + (cpu) + " %");

                            if (OnPerformanceTick != null) {
                                OnPerformanceTick(gameServer, (ram / 1024 / 1024), cpu);
                            }
                        }
                    }                   
                }
                catch (Exception) { }
            }
        }
    }
}
