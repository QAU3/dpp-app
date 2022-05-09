using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class RemoteMessageReceiver : UnityEvent<NetworkMessage>{ };
public class UDPREceive : MonoBehaviour
{
    public RemoteMessageReceiver onRemoteMessage = null;

    
    // receiving Thread
    Thread receiveThread;

    // udpclient object
    UdpClient client;

    // public
    // public string IP = "127.0.0.1"; default local
    public int port; // define > init

    // infos
    public string lastReceivedUDPPacket = "";
    public string allReceivedUDPPackets = ""; // clean up this from time to time!


    // start from shell
    private static void Main()
    {
        UDPREceive receiveObj = new UDPREceive();
        receiveObj.init();

        string text = "";
        do
        {
            text = Console.ReadLine();
        }
        while (!text.Equals("exit"));
    }
    // start from unity3d
    public void Start()
    {

        init();
    }

    // OnGUI
    //void OnGUI()
    //{
    //    Rect rectObj = new Rect(40, 10, 200, 400);
    //    GUIStyle style = new GUIStyle();
    //    style.alignment = TextAnchor.UpperLeft;
    //    GUI.Box(rectObj, "# UDPReceive\n127.0.0.1 " + port + " #\n"
    //                + "shell> nc -u 127.0.0.1 : " + port + " \n"
    //                + "\nLast Packet: \n" + lastReceivedUDPPacket
    //                + "\n\nAll Messages: \n" + allReceivedUDPPackets
    //            , style);
    //}

 
    private void Update()
    {

    }
    // init
    private void init()
    {
        // Endpunkt definieren, von dem die Nachrichten gesendet werden.
        print("UDPSend.init()");

        // define port
        port = 8051;

        // status


        // ----------------------------
        // Abhören
        // ----------------------------
        // Lokalen Endpunkt definieren (wo Nachrichten empfangen werden).
        // Einen neuen Thread für den Empfang eingehender Nachrichten erstellen.
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

    }
    private NetworkMessage GetPacketObject(byte[] packetBytes)
    {
        MemoryStream ms = new MemoryStream();
        ms.Write(packetBytes, 0, packetBytes.Length);
        ms.Position = 0;
        BinaryFormatter formatter = new BinaryFormatter();
        return formatter.Deserialize(ms) as NetworkMessage;
    }


    // receive thread
    private void ReceiveData()
    {

        client = new UdpClient(port);
        while (true)
        {

            try
            {
                // Bytes empfangen.
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);

                // Bytes mit der UTF8-Kodierung in das Textformat kodieren.
               // string text = Encoding.UTF8.GetString(data);
                NetworkMessage message = GetPacketObject(data);

                onRemoteMessage.Invoke(message);
                // Den abgerufenen Text anzeigen.
                print(">> " + message.ToString());

                // latest UDPpacket
               // lastReceivedUDPPacket = text;

                // ....
                //allReceivedUDPPackets = allReceivedUDPPackets + text;

            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    // getLatestUDPPacket
    // cleans up the rest
    public string getLatestUDPPacket()
    {
        allReceivedUDPPackets = "";
        return lastReceivedUDPPacket;
    }
}

