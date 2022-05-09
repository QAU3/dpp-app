using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;
using System;


/**<summary>
             A               B                   C
             |               |                   |
             |          prep Client        prep Server
             |               |                   |
             |               |                   |
             +-------------->+                   |
             |       1       |                   |
             |               +------------------>+
             |               |           2       |
             |               |                   |
             |               |                   |
             |               +<------------------+
             |               |           3       |
             +<--------------+                   |
             |       4       |                   |
             |               |                   |

A: external Script: calls the Method <c>FindServer</c>
B: this Script in the 'client' variant
C: this Script in the 'server' variant

starting: prepare client and server -> listen to the specific ports

1: external script calls the script <c>FindServer</c> and registers a callback method
2: client broadcasts discovery-signal over the network and server receives the signal
3: server answers to the client and client collects the ip
4: the callback of the external script is called 
</summary>
*/
public class ServerDiscovery : MonoBehaviour
{
    public enum DiscoveryType { REQUEST = 1, REQUEST_ALL = 2, RESPONSE = 4 };
    private class DiscoveryData
    {

        public DiscoveryType type;
        public int port;

        public string command;

        public DiscoveryData(DiscoveryType type, int port, string command)
        {
            this.type = type;
            this.port = port;
            this.command = command;
        }
        public DiscoveryData(byte[] bytes)
        {
            string data = System.Text.Encoding.ASCII.GetString(bytes);
            string[] split = data.Split(';');
            this.type = (DiscoveryType)byte.Parse(split[0]);
            this.port = int.Parse(split[1]);
            this.command = split[2];
        }

        public byte[] ToBytes()
        {
            return System.Text.Encoding.ASCII.GetBytes("" + (byte)type + ";" + port + ";" + command);
        }
    }

    [Header("General")]
    public bool server = false;
    [Tooltip("The port the Server listens to receive the broadcast message from the Client(s)")]
    public int serverPort = 6540;
    [Tooltip("The port the Client is listening to, in order to receive the answer of the Server")]
    public int clientPort = 6541;
    
    [Header("Server")]

    public List<string> supportedCommands = new List<string>{"findServer"};


    [Header("Client")]


    [Tooltip("If 'all Commands' is selected the client sends their request without a specific command and will use the first server that will respond")]
    public bool allCommands = false;
    public string clientCommand = "findServer";

    

    

    // internal Objects
    private UdpClient udpSender, udpReceiver;
    private Thread listenThread;
    private ConcurrentQueue<IPEndPoint> responses;
    private List<Action<IPEndPoint>> callbacks;

   

    // Start is called before the first frame update
    void Start()
    {
        if (this.server)
        {
            StartServer();
        }
        else
        {
            StartClient();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.server)
        {
            // bring the data from the receiving client thread to the main unity update thread
            while (!this.responses.IsEmpty)
            {
                IPEndPoint res;
                try
                {
                    this.responses.TryDequeue(out res);
                    if (res != null)
                    {
                        callbacks.ForEach((callback) =>
                        {
                            callback.Invoke(res);
                        });
                        callbacks.Clear();
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        if (this.listenThread.IsAlive)
            this.listenThread.Abort();

        this.udpSender.Close();
        this.udpReceiver.Close();
    }

    // ########################### client parts ###############################################################################################
    /**
    */
    public void FindServer(Action<IPEndPoint> callback)
    {
        callbacks.Add(callback);
        DiscoveryData request = new DiscoveryData(allCommands ? DiscoveryType.REQUEST_ALL : DiscoveryType.REQUEST, clientPort, clientCommand);
        byte[] data = request.ToBytes();
        this.udpSender.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, serverPort));
    }

    /**
     <summary> initialize the client by starting the receiving thread and also prepare the sender</summary>
     */
    private void StartClient()
    {
        this.responses = new ConcurrentQueue<IPEndPoint>();
        this.callbacks = new List<Action<IPEndPoint>>();

        this.listenThread = new Thread(new ThreadStart(ClientListen));
        this.listenThread.IsBackground = true;
        this.listenThread.Start();

        this.udpSender = new UdpClient();
        this.udpSender.EnableBroadcast = true;

    }
    /**
    <summary> client side listening in another trhead. All received data will be converted into <c>DiscoveryData</c>
    and the stored in a concurrentQueue to transport it to the main unity thread   </summary>
    */
    private void ClientListen()
    {
        // listen to the client port to be able to receive the descovery signal
        this.udpReceiver = new UdpClient(this.clientPort);
        while (this.udpReceiver != null)
        {
            try
            {
                //receive bytes and store them into queue for later processing in Update()
                IPEndPoint src = new IPEndPoint(IPAddress.Any, 0);
                byte[] bytes = this.udpReceiver.Receive(ref src);
                DiscoveryData data = new DiscoveryData(bytes);
                
                // only store responses
                if (data.type == DiscoveryType.RESPONSE)
                {
                    this.responses.Enqueue(src);
                    // this.udpReceiver.Close();
                    // this.udpReceiver = null;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
    }

    // ########################### server parts ###############################################################################################

    /** 
    <summary> Prepare server to listen to specific prt in another thread</summary>
    */
    private void StartServer()
    {
        // start thread for server receiving continously data
        this.listenThread = new Thread(new ThreadStart(ServerListen));
        this.listenThread.IsBackground = true;
        this.listenThread.Start();
    }

    /**
    <summary> The method for listening on the port in the server configuration and send the answer to the client upon request</summary>
    */
    private void ServerListen()
    {
        // listen to port and prepare to send response to discovery-signal
        this.udpReceiver = new UdpClient(this.serverPort);
        this.udpSender = new UdpClient();

        while (this.udpReceiver != null) // continue to receive data as long its existing
        {
            // endles loop running in other thread
            try
            {
                // receive discovery-signal
                IPEndPoint src = new IPEndPoint(IPAddress.Any, 0);
                byte[] bytes = this.udpReceiver.Receive(ref src);

                // convert bytes to DiscoveryData
                DiscoveryData data = new DiscoveryData(bytes);

                
                if (data.type == DiscoveryType.REQUEST_ALL || (data.type == DiscoveryType.REQUEST && supportedCommands.Contains(data.command)))
                {
                    // only if the request type is REQUEST_ALL or the command is contained in the list of supported Commands send an answer. If a RESPONSE is received, ignore it

                    // create response
                    DiscoveryData ret = new DiscoveryData(DiscoveryType.RESPONSE, serverPort, data.command);
                    Debug.Log("Request from: " + src.Address.ToString());
                    byte[] resp = ret.ToBytes();

                    // send response to asking client
                    this.udpSender.Send(resp, resp.Length, new IPEndPoint(src.Address, data.port));
                }

            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

    }
}
