using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using teichert.raft;

class NameServer
{
    static Random rand = new Random();
    static int min = 100;
    static int max = 100000;
    static Stopwatch sw = Stopwatch.StartNew();
    static UTF8Encoding encoding = new UTF8Encoding();
    static List<IPEndPoint> workersList = new List<IPEndPoint>();
    static void Main()
    {
        int RaftNodePort = 54321;

        var distributorTask = Task.Run(() =>
        {
            while (true)
            {
                Task.Run(() =>
                {

                    Console.WriteLine("Task starting at time " + sw.Elapsed);

                    //create a list of random names
                    var names = new List<string>() { "Adam", "Max", "Tanner" };
                    var rand = new Random();
                    var randName = names[rand.Next(names.Count)];

                    var permutate = rand.Next(10);

                    UdpClient udpClient = new UdpClient();

                    int data = rand.Next(min, max + 1);

                    string dataString = randName;

                    IPEndPoint listener = new IPEndPoint(0, 0);

                    //init raft message and send as json
                    var raftCommand = new RaftCommand {
                        CommandType = CommandType.Set,
                        Key = dataString,
                        Value = data,
                        Client = listener
                    };
                    var myRaftMessage = new RaftMessage {
                        MessageType = MessageType.Query,
                        RaftCommand = raftCommand
                    };
                    var toSend = JsonSerializer.SerializeToUtf8Bytes(myRaftMessage);

                    //init client and send request
                    UdpClient jobClient = new UdpClient();
                    jobClient.Send( toSend , toSend.Length, "127.0.0.1", RaftNodePort);

                    //wait for a message from the worker
                    byte[] workerResponse = jobClient.Receive(ref listener);
                    string workerResponseString = encoding.GetString(workerResponse);

                    Console.WriteLine("{0} received from RAFT", workerResponseString);
                });
            }
        });
    }
}