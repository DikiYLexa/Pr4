using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
   
    public class Program
    {
        public static List<User> Users = new List<User>();
        public static IPAddress IpAdress;
        public static int Port;
        public static bool AutorizationUser(string login, string password)
        {
            User user = null;
            user = Users.Find(x => x.login == login && x.password == password);
            return user != null;

        }
        public static List<string> GetDirectory(string src)
        {
            List<string> FoldersFile = new List<string>();
            if(Directory.Exists(src))
            {
                string[] dirs = Directory.GetDirectories(src);
                foreach(string dir in dirs)
                {
                    string NameDirectory = dir.Replace(src, "");
                    FoldersFile.Add(NameDirectory + "/");
                }
                string[] files = Directory.GetFiles(src);
                foreach(string file in files)
                {
                    string NameFile = file.Replace(src, "");
                    FoldersFile.Add(NameFile);
                }
            }
            return FoldersFile;
        }
        public static void ServerStart()
        {
            IPEndPoint endPoint = new IPEndPoint(IpAdress, Port);
            Socket sListner = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
                );
            sListner.Bind(endPoint);
            sListner.Listen(10);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Сервер запущен");
        }
    }
}
