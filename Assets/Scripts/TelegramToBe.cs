using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using Newtonsoft.Json;
using System.IO;
using AsyncIO;
using NetMQ;
using NetMQ.Sockets;

public class TelegramToBe : MonoBehaviour
{
    private HelloRequester _helloRequester;
    public string message = null;
    private void Start()
    {
        _helloRequester = new HelloRequester();
    }

    private void Update()
    {
        if(message==null)
        {
        
        }
    }

}