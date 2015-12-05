using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSD {
    struct FTPData {
        private string executeable;
        private string xmlFile;
        private int port;
        private string ftpUser;

        public FTPData(string executeable, string xmlFile, int port, string ftpUser) {
            this.executeable = executeable;
            this.xmlFile = xmlFile;
            this.port = port;
            this.ftpUser = ftpUser;
        }

        public string Executeable {
            get { return executeable; }
        }

        public string XmlFile {
            get { return xmlFile; }
        }

        public int Port {
            get { return port; }
        }

        public string User {
            get { return ftpUser; }
        }
    }
}
