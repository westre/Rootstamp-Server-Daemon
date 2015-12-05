using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RSD {

    partial class MainForm : Form {

        private Server server;

        delegate void SetTextCallback(string text);

        public MainForm() {
            InitializeComponent();
            Output("Initialized Component");

            messageLog.HorizontalScrollbar = true;
            gameServerPanel.AutoScroll = true;

            server = new Server(this);
        }

        public void Output(string message) {
            if (this.messageLog.InvokeRequired) {
                SetTextCallback d = new SetTextCallback(Output);
                this.Invoke(d, new object[] { message });
            }
            else {
                messageLog.Items.Add(message);
            }
        }

        public void InitializeGameServerPanel(List<GameServer> gameServers) {
            int top = 0;

            gameServerPanel.Controls.Clear();
            foreach (GameServer gameServer in gameServers) {
                Button serverButton = new Button();
                serverButton.Left = 10;
                serverButton.Top = top;
                serverButton.Text = "#" + gameServer.Id;
                serverButton.Tag = gameServer;
                serverButton.Size = new Size(gameServerPanel.Width - 40, serverButton.Height);
                serverButton.Click += ServerButton_Click;

                gameServerPanel.Controls.Add(serverButton);

                top += serverButton.Height + 2;
            }         
        }

        private void ServerButton_Click(object sender, EventArgs e) {
            Button button = (Button)sender;
            GameServer gameServer = (GameServer)button.Tag;

            if(gameServer.IsActive()) {
                ServerOutput(gameServer, "This server is active!");
            }
        }

        public void Error(string message) {
            if (this.messageLog.InvokeRequired) {
                SetTextCallback d = new SetTextCallback(Output);
                this.Invoke(d, new object[] { message });
            }
            else {
                messageLog.Items.Add("ERROR:: " + message);
                messageLog.SelectedIndex = messageLog.Items.Count - 1;
            }
        }

        public void ServerOutput(GameServer server, string message) {
            if (this.messageLog.InvokeRequired) {
                SetTextCallback d = new SetTextCallback(Output);
                this.Invoke(d, new object[] { message });
            }
            else {
                messageLog.Items.Add(server.Id + ":: " + message);
                messageLog.SelectedIndex = messageLog.Items.Count - 1;
            }
        }
    }
}
