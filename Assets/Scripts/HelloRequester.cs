using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Events;
using Newtonsoft.Json;
using System.IO;

/// <summary>
///     Example of requester who only sends Hello. Very nice guy.
///     You can copy this class and modify Run() to suits your needs.
///     To use this class, you just instantiate, call Start() when you want to start and Stop() when you want to stop.
/// </summary>
public class HelloRequester : RunAbleThread
{
    /// <summary>
    ///     Request Hello message to server and receive message back. Do it 10 times.
    ///     Stop requesting when Running=false.
    /// </summary>
    public string telegrammessage = null;
    public int telegramchatid = 0;
    private Telegram telegram;
    private TelegramToBe telegramnew;
    public string clientresponse = null;
    public bool first = true;
    private RequestSocket client;

    // Action telegramsend = sendtelly;
    // UnityThread.executeInUpdate(sendtelly);
    
    public void getTelegram() 
    {
        telegram = GameObject.FindWithTag("server").GetComponent<Telegram>();
    }

    protected override void Run()
    {
        ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use, not sure why yet
        client = new RequestSocket();
        client.Connect("tcp://localhost:5555");

            // for (int i = 0; i < 10 && Running; i++)
            // {
            //     Debug.Log("Sending Hello");
            //     client.SendFrame("Hello");
            // ReceiveFrameString() blocks the thread until you receive the string, but TryReceiveFrameString()
            // do not block the thread, you can try commenting one and see what the other does, try to reason why
            // unity freezes when you use ReceiveFrameString() and play and stop the scene without running the server
//               string message = client.ReceiveFrameString();
//               Debug.Log("Received: " + message);
            //}

        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use, not sure why yet
    }

    public bool sendToPython()
    {
        while(Running)
        {
            if(telegrammessage!=null)
            {
                Debug.Log("Sending Hello");
                client.SendFrame(telegrammessage);
                telegrammessage = null;
                if (first) first = false;
                return true;
            }
        }
        return false;
    }

    public bool receivePython()
    {
        string message = null;
        bool gotMessage = false;
        while (Running)
        {
            gotMessage = client.TryReceiveFrameString(out message); // this returns true if it's successful
            clientresponse = message;
            if (gotMessage) Debug.Log("Received " + clientresponse);
            if (gotMessage) return true;
        }
        return false;
        //telegram.sendtelly(message, telegramchatid);
        //telegram.SendTelegramMessage(message, telegramchatid);
    }

    public bool isRunning()
    {
        return Running;
    }

    // public void sendtelly(string message, int chatid)
    // {
    //     Invoke();
    // }

}