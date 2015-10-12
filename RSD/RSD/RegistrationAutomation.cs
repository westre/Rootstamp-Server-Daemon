using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace RSD {
    class RegistrationAutomation {
        private Server server;
        private Database database;

        public RegistrationAutomation(Server server) {
            this.server = server;
            this.database = server.Database;

            Timer registrationTimer = new Timer();
            registrationTimer.Elapsed += new ElapsedEventHandler(OnRegistrationTimer);
            registrationTimer.Interval = 5000;
            registrationTimer.Enabled = true;

            server.Form.Output("RA object created");
        }

        private void OnRegistrationTimer(object source, ElapsedEventArgs e) {
            CheckTerminations();
            CheckActivations();
            CheckPremiumExpiredServers();
        }

        private void CheckPremiumExpiredServers() {
            int serviceId = -1;
            string email = "";
            string userName = "";
            int msId = -1;
            int port = -1;

            database.GetExpiredPremiumServer(out serviceId, out email, out userName, out msId, out port);

            if (serviceId != -1) {
                WebRequest webRequest = WebRequest.Create("https://rootstamp.com/admin.php?rsautomation_override=1&set_to_free=1&id=" + serviceId + "&email=" + email + "&name=" + userName);
                webRequest.Timeout = 3000;

                try {
                    WebResponse webResponse = webRequest.GetResponse();
                    webResponse.Close();
                }
                catch (Exception ex) {

                }

                server.Form.Output("Set premium to free server: " + serviceId);
            }
        }

        private void CheckActivations() {
            int serviceId = -1;
            string email = "";
            string userName = "";

            database.GetServerToBeActivated(out serviceId, out email, out userName);

            if(serviceId != -1) {
                WebRequest webRequest = WebRequest.Create("https://rootstamp.com/admin.php?rsautomation_override=1&activate=1&id=" + serviceId + "&email=" + email + "&name=" + userName);
                webRequest.Timeout = 3000;

                try {
                    WebResponse webResponse = webRequest.GetResponse();
                    webResponse.Close();
                }
                catch (Exception ex) {

                }

                server.Form.Output("Activated server: " + serviceId);
            }
        }

        private void CheckTerminations() {
            int differenceHours;
            int serviceId = -1;
            string email = "";
            string userName = "";
            int msId = -1;
            int port = -1;

            database.GetTerminatedServer(out differenceHours, out serviceId, out email, out userName, out msId, out port);

            if(serviceId != -1) {
                WebRequest webRequest = WebRequest.Create("https://rootstamp.com/admin.php?rsautomation_override=1&terminate=1&id=" + serviceId + "&email=" + email + "&name=" + userName + "&msid=" + msId + "&port=" + port);
                webRequest.Timeout = 3000;

                try {
                    WebResponse webResponse = webRequest.GetResponse();
                    webResponse.Close();
                }
                catch (Exception ex) {

                }

                server.Form.Output("Terminated server: " + serviceId);
            }   
        }

        private long ConvertToUnixTime(DateTime datetime) {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(datetime - sTime).TotalSeconds;
        }
    }
}
