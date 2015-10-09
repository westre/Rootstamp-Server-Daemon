﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RSD {
    class Server {
        private MainForm form;
        private SocketListener socketListener;
        private Database database;

        private static List<GameServer> gameServers;

        public Server(MainForm form) {
            this.form = form;

            database = new Database(this);

            gameServers = database.GetAllGameServers();
            LinkProcesses(gameServers);

            form.InitializeGameServerPanel(gameServers);

            socketListener = new SocketListener(this, 33333);
            socketListener.SocketMessageReceived += OnMessageReceive;
            form.Output("SocketListener initialized");
        }

        public void OnMessageReceive(SocketListener socketListener, TcpClient tcpClient, string message) {
            form.Output("OnMessageReceive: " + message);

            dynamic jsonObject = JsonConvert.DeserializeObject(message);
            string appendMessage = ProcessRequest(jsonObject);

            socketListener.Finalize(tcpClient, message + "<br/>" + appendMessage);
        }

        private string ProcessRequest(dynamic jsonObject) {
            string message = "";

            if(jsonObject.server_status != null) {
                int serverId = jsonObject.server_status.id;
                GameServer gameServer = Find(serverId);

                if (gameServer != null) {
                    if (gameServer.IsActive()) {
                        form.ServerOutput(gameServer, "Server is online");
                        message += "Server is online";
                    }
                    else {
                        form.ServerOutput(gameServer, "Server is offline");
                        message += "Server is offline";
                    }
                }
                else {
                    form.Error("Server_status: Could not find id " + jsonObject.server_start.id);
                    message += "Server_status: Could not find id " + jsonObject.server_start.id;
                }
            }
            else if(jsonObject.server_start != null) {
                int serverId = jsonObject.server_start.id;
                GameServer gameServer = Find(serverId);

                if (gameServer != null) {
                    List<string> messages;
                    if(gameServer.Start(out messages)) {
                        form.ServerOutput(gameServer, "Started server: " + string.Join(",", messages.ToArray()));
                        message += "Started server: " + string.Join(",", messages.ToArray());
                    }
                    else {
                        form.ServerOutput(gameServer, "Could not start server: " + string.Join(",", messages.ToArray()));
                        message += "Could not start server: " + string.Join(",", messages.ToArray());
                    }
                }
                else {
                    form.Error("Server_start: Could not find id " + jsonObject.server_start.id);
                    message += "Server_start: Could not find id " + jsonObject.server_start.id;
                }
            }
            else if (jsonObject.server_stop != null) {
                int serverId = jsonObject.server_stop.id;
                GameServer gameServer = Find(serverId);

                if (gameServer != null) {
                    List<string> messages;
                    if (gameServer.Stop(out messages)) {
                        form.ServerOutput(gameServer, "Stopped server: " + string.Join(",", messages.ToArray()));
                        message += "Stopped server: " + string.Join(",", messages.ToArray());
                    }
                    else {
                        form.ServerOutput(gameServer, "Could not stop server: " + string.Join(",", messages.ToArray()));
                        message += "Could not stop server: " + string.Join(",", messages.ToArray());
                    }
                }
                else {
                    form.Error("Server_stop: Could not find id " + jsonObject.server_start.id);
                    message += "Server_stop: Could not find id " + jsonObject.server_start.id;
                }
            }
            else if (jsonObject.server_reinstall != null) {
                int serverId = jsonObject.server_reinstall.id;
                GameServer gameServer = Find(serverId);

                if (gameServer != null) {
                    List<string> messages;
                    if (gameServer.Reinstall(out messages)) {
                        form.ServerOutput(gameServer, "Reinstalled server: " + string.Join(",", messages.ToArray()));
                        message += "Reinstalled server: " + string.Join(",", messages.ToArray());
                    }
                    else {
                        form.ServerOutput(gameServer, "Could not reinstall server: " + string.Join(",", messages.ToArray()));
                        message += "Could not reinstall server: " + string.Join(",", messages.ToArray());
                    }
                }
                else {
                    form.Error("Server_reinstall: Could not find id " + jsonObject.server_start.id);
                    message += "Server_reinstall: Could not find id " + jsonObject.server_start.id;
                }
            }
            else if (jsonObject.ftp_generate != null) {
                int serverId = jsonObject.ftp_generate.id;
                GameServer gameServer = Find(serverId);

                if (gameServer != null) {
                    List<string> messages;
                    if (gameServer.GenerateFTP(out messages)) {
                        form.ServerOutput(gameServer, "Generated FTP: " + string.Join(",", messages.ToArray()));
                        message += "Generated FTP: " + string.Join(",", messages.ToArray());
                    }
                    else {
                        form.ServerOutput(gameServer, "Could not generate FTP: " + string.Join(",", messages.ToArray()));
                        message += "Could not generate FTP: " + string.Join(",", messages.ToArray());
                    }
                }
                else {
                    form.Error("Ftp_generate: Could not find id " + jsonObject.server_start.id);
                    message += "Ftp_generate: Could not find id " + jsonObject.server_start.id;
                }
            }

            return message;
        }

        public static List<GameServer> GameServers {
            get { return gameServers; }
            set { gameServers = value; }
        }

        private GameServer Find(int id) {
            foreach(GameServer gameServer in gameServers) {
                if (gameServer.Id == id)
                    return gameServer;
            }
            return null;
        }

        private void LinkProcesses(List<GameServer> gameServers) {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes) {
                try {
                    string fullProcessPath = process.MainModule.FileName;

                    foreach(GameServer gameServer in gameServers) {
                        if(gameServer.GamePath + gameServer.Executeable == fullProcessPath) {
                            gameServer.Process = process;
                            form.Output("Found a link ID: " + gameServer.Id);
                        }
                    }
                }
                catch (Exception ex) { }
            }
        }

        public MainForm Form {
            get { return form; }
            set { form = value; }
        }
    }
}