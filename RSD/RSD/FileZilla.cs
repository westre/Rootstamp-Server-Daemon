using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RSD {
    class FileZilla {
        public static string GeneratePassword(GameServer gameServer) {
            string fileZillaExecuteable = gameServer.FTPData.Executeable;
            string xmlDocument = gameServer.FTPData.XmlFile;
            string user = gameServer.FTPData.User;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlDocument);

            XmlNodeList userNodes = xmlDoc.SelectNodes("/FileZillaServer/Users/User");

            foreach (XmlNode userNode in userNodes) {
                if (userNode.Attributes["Name"].Value.Equals(user)) {
                    Console.WriteLine("I found the correct user: " + user);

                    XmlNodeList optionNodes = userNode.ChildNodes;
                    foreach (XmlNode optionNode in optionNodes) {
                        if (optionNode.Attributes["Name"] != null && optionNode.Attributes["Name"].Value.Equals("Pass")) {
                            Console.WriteLine("BEFORE: " + optionNode.Attributes["Name"].Value + " == " + optionNode.InnerText);

                            string newPassword = Guid.NewGuid().ToString().Substring(0, 8);
                            Console.WriteLine("Generating MD5 for: " + newPassword);
                            string md5 = GenerateMD5(newPassword);

                            optionNode.InnerText = md5;

                            Console.WriteLine("AFTER: " + optionNode.Attributes["Name"].Value + " == " + optionNode.InnerText);

                            xmlDoc.Save(xmlDocument);
                            Process.Start(fileZillaExecuteable, "/reload-config");

                            return newPassword;
                        }
                    }
                    break;
                }
            }

            return null;
        }

        private static string GenerateMD5(string str) {
            MD5 md5 = new MD5CryptoServiceProvider();

            md5.ComputeHash(Encoding.ASCII.GetBytes(str));

            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++) {
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }
    }
}
