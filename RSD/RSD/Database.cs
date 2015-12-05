using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSD {
    class Database {
        private Server server;
        private MySqlConnection connection;

        public Database(Server server) {
            this.server = server;

            connection = new MySqlConnection("Server=95.85.27.65;Database=rsd;Uid=westre;Pwd=holland17");            
        }

        public List<GameServer> GetAllGameServers() {
            Open();

            List<GameServer> gameServers = new List<GameServer>();

            MySqlCommand command = new MySqlCommand("SELECT server_instance.id, server_instance.root_folder, server_instance.port, server_instance.executeable, server_instance.server_instance_package_id, game.game, game.clean_game_directory, master_server.ftp_executeable, master_server.ftp_xml, master_server.ftp_port, server_instance.ftp_user FROM server_instance JOIN game ON server_instance.game_id = game_id JOIN master_server ON server_instance.master_server_id = master_server.id WHERE master_server.ip = @ip", connection);
            command.Parameters.AddWithValue("@ip", Utility.GetExternalIP()); /*Utility.GetExternalIP()*/

            MySqlDataReader reader = command.ExecuteReader();

            int _id = reader.GetOrdinal("id");
            int _path = reader.GetOrdinal("root_folder");
            int _port = reader.GetOrdinal("port");
            int _executeable = reader.GetOrdinal("executeable");
            int _game = reader.GetOrdinal("game");
            int _gameDir = reader.GetOrdinal("clean_game_directory");
            int _ftpExecuteable = reader.GetOrdinal("ftp_executeable");
            int _ftpXml = reader.GetOrdinal("ftp_xml");
            int _ftpPort = reader.GetOrdinal("ftp_port");
            int _ftpUser = reader.GetOrdinal("ftp_user");
            int _packageId = reader.GetOrdinal("server_instance_package_id");

            while (reader.Read()) {
                int id = reader.GetInt32(_id);
                int port = reader.GetInt32(_port);
                string path = reader.GetString(_path);
                string executeable = reader.GetString(_executeable);
                string game = reader.GetString(_game);
                string gameDirectory = reader.GetString(_gameDir);
                string ftpExecuteable = reader.GetString(_ftpExecuteable);
                string ftpXml = reader.GetString(_ftpXml);
                int ftpPort = reader.GetInt32(_ftpPort);
                string ftpUser = reader.GetString(_ftpUser);
                int packageId = reader.GetInt32(_packageId);

                if (game == "SA:MP") {
                    SAMPServer sampServer = new SAMPServer(id, port, path, executeable, new FTPData(ftpExecuteable, ftpXml, ftpPort, ftpUser), packageId);
                    sampServer.CleanGameDirectory = gameDirectory;

                    gameServers.Add(sampServer);
                }
                else
                    server.Form.Error("Invalid game found: " + game);
            }

            reader.Close();
            Close();

            return gameServers;
        }

        public void GetTerminatedServer(out int differenceHours, out int serviceId, out string email, out string userName, out int msId, out int port) {
            Open();

            // Get records of those who haven't checked in for atleast 24 hours
            DateTime seekTime = DateTime.Now;
            seekTime = seekTime.AddHours(-38);

            // Create the MySQL format date
            string MySQLDate = seekTime.ToString("yyyy-MM-dd HH:mm:ss");

            MySqlCommand command = new MySqlCommand("SELECT server_instance.*, account.email, account.username FROM server_instance JOIN master_server ON server_instance.master_server_id = master_server.id JOIN account ON server_instance.account_id = account.id WHERE server_instance.last_checkin < @seekTime AND server_instance.account_id != 1 AND server_instance_package_id = 1 AND master_server.ip = @ip LIMIT 1", connection);
            command.Parameters.AddWithValue("@seekTime", MySQLDate);
            command.Parameters.AddWithValue("@ip", Utility.GetExternalIP());

            MySqlDataReader reader = command.ExecuteReader();

            int _accountId = reader.GetOrdinal("account_id");
            int _lastCheckIn = reader.GetOrdinal("last_checkin");
            int _serviceId = reader.GetOrdinal("id");
            int _email = reader.GetOrdinal("email");
            int _username = reader.GetOrdinal("username");
            int _msId = reader.GetOrdinal("master_server_id");
            int _port = reader.GetOrdinal("port");

            differenceHours = -1;
            serviceId = -1;
            email = "";
            userName = "";
            msId = -1;
            port = -1;

            while (reader.Read()) {
                DateTime checkIn = reader.GetMySqlDateTime(_lastCheckIn).GetDateTime();
                DateTime currentTime = DateTime.Now;
                TimeSpan difference = currentTime - checkIn;

                differenceHours = Convert.ToInt32(difference.TotalHours);
                serviceId = reader.GetInt32(_serviceId);
                email = reader.GetString(_email);
                userName = reader.GetString(_username);
                msId = reader.GetInt32(_msId);
                port = reader.GetInt32(_port);
            }

            reader.Close();
            Close();
        }

        public void GetServerToBeActivated(out int serviceId, out string email, out string userName) {
            Open();

            MySqlCommand command = new MySqlCommand("SELECT server_instance.*, account.email, account.username FROM server_instance JOIN master_server ON server_instance.master_server_id = master_server.id JOIN account ON server_instance.account_id = account.id WHERE server_instance.active = 0 AND server_instance.account_id IS NOT NULL AND master_server.ip = @ip LIMIT 1", connection);
            command.Parameters.AddWithValue("@ip", Utility.GetExternalIP());

            MySqlDataReader reader = command.ExecuteReader();

            int _serviceId = reader.GetOrdinal("id");
            int _email = reader.GetOrdinal("email");
            int _username = reader.GetOrdinal("username");

            serviceId = -1;
            email = "";
            userName = "";

            while (reader.Read()) {
                serviceId = reader.GetInt32(_serviceId);
                email = reader.GetString(_email);
                userName = reader.GetString(_username);
            }

            reader.Close();
            Close();
        }

        public void GetExpiredPremiumServer(out int serviceId, out string email, out string userName, out int msId, out int port) {
            Open();

            DateTime seekTime = DateTime.Now;

            // Create the MySQL format date
            string MySQLDate = seekTime.ToString("yyyy-MM-dd HH:mm:ss");

            MySqlCommand command = new MySqlCommand("SELECT server_instance.*, account.email, account.username FROM server_instance JOIN master_server ON server_instance.master_server_id = master_server.id JOIN account ON server_instance.account_id = account.id WHERE @now > server_instance.package_until AND server_instance_package_id >= 2 AND master_server.ip = @ip LIMIT 1", connection);
            command.Parameters.AddWithValue("@now", MySQLDate);
            command.Parameters.AddWithValue("@ip", Utility.GetExternalIP());

            MySqlDataReader reader = command.ExecuteReader();

            int _serviceId = reader.GetOrdinal("id");
            int _email = reader.GetOrdinal("email");
            int _username = reader.GetOrdinal("username");
            int _msId = reader.GetOrdinal("master_server_id");
            int _port = reader.GetOrdinal("port");

            serviceId = -1;
            email = "";
            userName = "";
            msId = -1;
            port = -1;

            while (reader.Read()) {
                serviceId = reader.GetInt32(_serviceId);
                email = reader.GetString(_email);
                userName = reader.GetString(_username);
                msId = reader.GetInt32(_msId);
                port = reader.GetInt32(_port);
            }

            reader.Close();
            Close();
        }

        public MySqlConnection Connection {
            get { return connection; }
        }

        public void Open() {
            if(connection != null && connection.State == ConnectionState.Closed)
                connection.Open();
        }

        public void Close() {
            if(connection != null && connection.State == ConnectionState.Open)
                connection.Close();
        }
    }
}
