using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
namespace teichert.raft;

class NameServer
{
    static Random rand = new Random();
    static int min = 100;
    static int max = 100000;
    static Stopwatch sw = Stopwatch.StartNew();
    // static UTF8Encoding encoding = new UTF8Encoding();
    static List<IPEndPoint> workersList = new List<IPEndPoint>();

    //public class LogEntry
    //{
    //    public int Term { get; set; }
    //    public RaftCommand? Command { get; set; }

    //    public static LogEntry FromJson(string json)
    //    {
    //        return JsonSerializer.Deserialize<LogEntry>(json)!;
    //    }
    //    public string Json()
    //    {
    //        return JsonSerializer.Serialize(this);
    //    }
    //}
    //public class AppendEntriesRPCInfo
    //{
    //    public int LeaderTerm { get; set; } // c.f. term
    //    public int LeaderIndex { get; set; } // c.f. leaderId
    //    public int EntriesAlreadySent { get; set; } // c.f. prevLogIndex
    //    public int LastTermSent { get; set; }  // c.f. prevLogTerm
    //    public LogEntry[] EntriesToAppend { get; set; } = new LogEntry[] { }; // c.f. entries[]
    //    public int SenderSafeEntries { get; set; } // c.f. leaderCommit
    //}

    //public class RPCResponseInfo
    //{
    //    public int CurrentTerm { get; set; } // c.f. term
    //    public bool Response { get; set; } // c.f. success / voteGranted
    //}

    //public class RequestVoteRPCInfo
    //{
    //    public int CandidateTerm { get; set; } // c.f. term
    //    public int CandidateIndex { get; set; } // c.f. candidateId
    //    public int CandidateLogLength { get; set; } // c.f. lastLogIndex
    //    public int CandidateLogTerm { get; set; } // c.f. lastLogterm
    //}


    //public class RaftMessage
    //{
    //    public MessageType MessageType { get; set; }
    //    public AppendEntriesRPCInfo? AppendEntriesRPCInfo { get; set; }
    //    public RequestVoteRPCInfo? RequestVoteRPCInfo { get; set; }
    //    public RPCResponseInfo? RPCResponseInfo { get; set; }
    //    public RaftCommand? RaftCommand { get; set; }

    //    public static RaftMessage FromJson(string json)
    //    {
    //        return JsonSerializer.Deserialize<RaftMessage>(json)!;
    //    }
    //    public string Json()
    //    {
    //        return JsonSerializer.Serialize(this);
    //    }
    //}

    //public class RaftCommand
    //{
    //    public CommandType CommandType { set; get; }
    //    public string? Key { set; get; } = null;
    //    public int Value { set; get; }
    //    public IPEndPoint? Client { get; set; } = null;
    //}


    //public enum MessageType
    //{
    //    VoteRequest,
    //    Response,
    //    Query,
    //    AppendEntries
    //}

    //public enum CommandType
    //{
    //    Init,
    //    Set,
    //    Get
    //}
    public static string CallServer(RaftCommand raftCommand, IPEndPoint server)
    {
        UdpClient udpClient = new UdpClient();
        //IPEndPoint clientListener = new IPEndPoint(0, 0);

        //init raft message and send as json

        var myRaftMessage = new RaftMessage
        {
            MessageType = MessageType.Query,
            RaftCommand = raftCommand
        };
        Console.WriteLine("Sending " + myRaftMessage + " to " + server);

        RaftMessage reply = RaftNode.SendAndReceive(server, myRaftMessage);
        Console.WriteLine(reply);
        return reply.RaftResponse!;
    }

    static void Main()
    {
        int RaftNodePort = 54321;
        IPEndPoint server = IPEndPoint.Parse("127.0.0.1:" + RaftNodePort);


        var clientTask = Task.Run(() =>
        {
            while (true)
            {

                Console.WriteLine("Task starting at time " + sw.Elapsed);

                //create a list of random names
                var names = new List<string>() { "Adam", "Max", "Tanner" };
                var rand = new Random();
                var randName = names[rand.Next(names.Count)];
                int data = rand.Next(min, max + 1);
                var permutate = rand.Next(10);
                string dataString = randName;
                
                var raftCommand = new RaftCommand
                {
                    CommandType = CommandType.Set,
                    Key = dataString,
                    Value = data,
                    //Client = clientListener
                };
                Console.WriteLine($"set {dataString} {CallServer(raftCommand, server)}");
                


                //var serialized = "hello";

                ////try catch
                //try
                //{
                //    //send message
                //    byte[] toSend = JsonSerializer.SerializeToUtf8Bytes(myRaftMessage);
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e.Message);
                //}
                //finally
                //{
                //    udpClient.Close();
                //}
                //// byte[] toSend = JsonSerializer.SerializeToUtf8Bytes(myRaftMessage);


                ////init client and send request
                //UdpClient jobClient = new UdpClient();
                //// jobClient.Send(toSend, toSend.Length, "127.0.0.1", RaftNodePort);

                ////wait for a message from the worker
                //byte[] workerResponse = jobClient.Receive(ref clientListener);
                // string workerResponseString = encoding.GetString(workerResponse);

                // //console log response
                // Console.WriteLine("Received " + workerResponseString + " from " + clientListener);

                // Console.WriteLine("{0} received from RAFT", workerResponseString);
                var request = Console.ReadLine();
                 var raftGet = new RaftCommand
                {
                    CommandType = CommandType.Get,
                    Key = request
                    //Client = clientListener
                };
                
                Console.WriteLine($"got {request} {CallServer(raftGet, server)}");

            }
        });

        //     var workerTask = Task.Run(() =>
        //    {
        //        UdpClient workerReceiver = new UdpClient(45655);

        //        while (true)
        //        {

        //            IPEndPoint newWorker = new IPEndPoint(0, 0);
        //            byte[] requestData = workerReceiver.Receive(ref newWorker);
        //         //    string requestString = encoding.GetString(requestData);


        //         //    if (requestString.Contains("free"))
        //         //    {
        //         //        Console.WriteLine("Received {0} from worker {1}", requestString, newWorker);
        //         //        Console.WriteLine("Adding worker {0} to queue", newWorker);
        //         //        workersList.Add(newWorker);
        //         //    }
        //        }
        //    });

        Task.WaitAll(clientTask);
    }
}