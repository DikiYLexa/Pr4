using System.Net;
using System.Net.Sockets;
using System.Text;
using Common;
using Newtonsoft.Json;
IPAddress IpAdress;
int Port;
int Id = -1;
Console.WriteLine("Введите IP адрес сервера: ");
bool isParsedIP = IPAddress.TryParse(Console.ReadLine(), out IpAdress);
bool isParsedPort = int.TryParse(Console.ReadLine(), out Port);
if(isParsedIP && isParsedPort)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Ваш запрос принят. Подключаюсь к серверу.");
    while (true)
        ConnectServer();
}


bool CheckCommand(string message)
{
    string[] dataMessage = message.Split(" ");

    if (dataMessage.Length == 0)
        return false;
    string command = dataMessage[0];
    switch (command)
    {
        case "connect":
            if (dataMessage.Length != 3)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Использование: connect [login] [password]\nПример: connect User1 pASSword");
                return false;
            }
            else return true;
        case "cd":
            return true;
        case "get":
            if(dataMessage.Length == 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Использование: get [NameFile]\nПример: get Test.txt");
                return false;
            }
            else return true;
        case "set":
            if (dataMessage.Length == 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Использование: set [NameFile]\nПример: get Test.txt");
                return false;
            }
            else return true;
    };
    return false;
}

void ConnectServer()
{
    try
    {
        IPEndPoint endPoint = new IPEndPoint(IpAdress, Port);
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(endPoint);
        if (socket.Connected)
        {
            Console.ForegroundColor = ConsoleColor.White;
            string message = Console.ReadLine();
            if (CheckCommand(message))
            {
                ViewModelSend viewModelSend = new ViewModelSend(message, Id);
                if (message.Split(" ")[0] == "set")
                {
                    string[] dataMessage = message.Split(" ");
                    string nameFile = "";
                    for (int i = 1; i < dataMessage.Length; i++)
                        if (nameFile == "")
                            nameFile += dataMessage[i];
                        else nameFile += " " + dataMessage[i];
                    if (File.Exists(nameFile))
                    {
                        FileInfo fileInfo = new(nameFile);
                        FileInfoFTP newFileInfo = new(File.ReadAllBytes(nameFile), fileInfo.Name);
                        viewModelSend = new(JsonConvert.SerializeObject(newFileInfo), Id);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Указанный файл не существует");
                    }
                }
                byte[] messageByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(viewModelSend));
                int bytesSend = socket.Send(messageByte);
                byte[] bytes = new byte[10485760];
                int bytesRec = socket.Receive(bytes);
                string messageServer = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                ViewModelMessage viewModelMessage = JsonConvert.DeserializeObject<ViewModelMessage>(messageServer);

                if (viewModelMessage.Command == "Authorization")
                    Id = int.Parse(viewModelMessage.Data);
                else if (viewModelMessage.Command == "message")
                    Console.WriteLine(viewModelMessage.Data);
                else if (viewModelMessage.Command == "cd")
                {
                    List<string> foldersFiles = new List<string>();
                    foldersFiles = JsonConvert.DeserializeObject<List<string>>(viewModelMessage.Data);
                    foreach (string name in foldersFiles)
                        Console.WriteLine(name);
                }
                else if (viewModelMessage.Command == "file")
                {
                    string[] dataMessage = viewModelSend.Message.Split(" ");
                    string getFile = "";
                    for (int i = 1; i < dataMessage.Length; i++)
                        if (getFile == "")
                            getFile = dataMessage[i];
                        else getFile += " " + dataMessage[i];
                    byte[] byteFile = JsonConvert.DeserializeObject<byte[]>(viewModelMessage.Data);
                    File.WriteAllBytes(getFile, byteFile);
                }
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Подключение не удалось");
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(ex.Message);
    }
    
}