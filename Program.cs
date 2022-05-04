using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

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

                    var raftCommand = new RaftCommand() {
                        CommandType = CommandType.Set,
                        Key = dataString,
                        Value = data,
                        IPEndPoint = new IPEndPoint(IPAddress.Loopback, RaftNodePort)
                    }

                    if (permutate % 2 == 0)
                    {
                        dataString = "nameServer-" + randName + ":" + data.ToString();
                    }

                    var sendData = encoding.GetBytes(dataString);
                    Console.WriteLine("Sending data {0} to server {1}", dataString, RaftNodePort);
                    udpClient.Send(sendData, dataString.ToString().Length, "127.0.0.1", RaftNodePort);

                    var from = new IPEndPoint(0, 0);
                    byte[] recvBuffer = udpClient.Receive(ref from);
                    string message = encoding.GetString(recvBuffer);
                    Console.WriteLine("{0} received from {1}", message, from);
                    Console.WriteLine("Task stopping at time " + sw.Elapsed);

                    UdpClient jobClient = new UdpClient();

                    // Console.WriteLine($"Free workers: {workersList.Count}");

                    Console.WriteLine("Sending {0} to worker {1}", requestString, workersList.IndexOf(workersList[workerNumber]));

                    jobClient.Send(requestData, requestData.Length, workersList[workerNumber]);

                    IPEndPoint responder = new IPEndPoint(0, 0);

                    //wait for a message from the worker
                    byte[] workerResponse = jobClient.Receive(ref responder);
                    string workerResponseString = encoding.GetString(workerResponse);

                    Console.WriteLine("{0} received from worker {1}", workerResponseString, workerNumber);

                    Console.WriteLine("Sending {0} to distributor {1}", workerResponseString, distRequester);
                    byte[] responseData = encoding.GetBytes(workerResponseString);

                    nsClient.Send(responseData, responseData.Length, distRequester);

                });

            }

        });


        Task.WaitAll(distributorTask, dbTask);

    }



}