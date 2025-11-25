using System.Net;
using System.Net.Sockets;
using Common;
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
        }
    }
}