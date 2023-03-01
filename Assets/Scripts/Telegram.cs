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

[System.Serializable]
public class IntEvent : UnityEvent<int> { }

public class Telegram : MonoBehaviour
{
    public string chat_id = "6040682630"; // ID (you can know your id via @userinfobot)
    public string TOKEN = "6040682630:AAF8WI6ry4wa979hdqipOKOIgH4FhGxyVak"; // bot token (@BotFather)

    UdpSocket udpSocket;
    public string modelresponse = null;
    public string previousresponse = null;
    public int currentchat = 0;
    private bool conversationactive = false;
    private bool modelfree = true;
    private bool messagesentattempt = false;

    [SerializeField] public Queue<Message> messagesqueue = new Queue<Message>();
    [SerializeField] public Queue<string> modelrequeue = new Queue<string>();

    public string API_URL
    {
        get
        {
            return string.Format("https://api.telegram.org/bot{0}/", TOKEN);
        }
    }

    public int lastUpdateId = 0;
    public IntEvent sendPictureToChat;
    public UnityEvent messageReceived;

    private void Start()
    {
        udpSocket = FindObjectOfType<UdpSocket>();
        //SendTelegramMessage("helo telegram", chat_id);
        GetUpdates();
    }

    private void Update()
    {
        //if queue is not empty, dequeue
        //if (modelresponse!="none" && modelresponse != previousresponse)
        if(messagesqueue.Count != 0 && modelfree == true)
        {
            Debug.Log("SENDING OVER SERVER");
            string json = JsonUtility.ToJson(messagesqueue.Peek());
            //udpSocket.SendData(json); //USE UdpSocket.PROCESS TEXT function, merge telegram.cs and UdpSocket, two are not needed.
            udpSocket.SendData(messagesqueue.Peek().text);
            modelfree = false;
        } 
        else if(messagesqueue.Count != 0 && (modelresponse != null && modelresponse != "") && conversationactive && !messagesentattempt) 
        {
            Debug.Log("SENDING TO TELEGRAM");
            Debug.Log(messagesqueue.Peek().text);
            Debug.Log(modelresponse);
            SendTelegramMessage(modelresponse, messagesqueue.Peek().chat.id);
        }
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown("space"))
    //     {
    //         int i = 0;
    //         foreach(Message id in messagesqueue)
    //         {
    //             Debug.Log("Entry number: " + i + "| " + id.from.first_name + " | " + id.text + " | " + id.chat.id + " | " + id.date + " |");
    //             i++;
    //         }
    //     }
    //     else if (Input.GetKeyDown(KeyCode.M))
    //     {
    //         messagesqueue.Dequeue();
    //     }
    // }

    public class TelegramResultOfSentMessage
    {
        public bool ok;
        public Message result;
    }
    public class TelegramResult
    {
        public bool ok;
        public TelegramUpdate[] result;
    }
    public class TelegramUpdate
    {
        public int update_id;
        public Message message;
    }

    [Serializable]
    public class Message
    {
        public int message_id;
        public int date;
        public string text;
        public string modelresponse = "";
        // public Photo[] photo;
        public From from;
        public Chat chat;
    }
    // public class Photo
    // {
    //     public string file_id;
    //     public string file_unique_id;
    //     public int file_size;
    //     public int width;
    //     public int height;
    //     public string file_path;
    // }

    [Serializable]
    public class Chat
    {
        public string id;
        public string title;
        public string type;
        public bool all_members_are_administrators;
    }

    [Serializable]
    public class From
    {
        public long id;
        public bool is_bot;
        public string first_name;
        public string last_name;
        public string language_code;
    }
    // public class PhotoUpdate
    // {
    //     public bool ok;
    //     public Photo result;
    // }

    // public void GetMe()
    // {
    //     WWWForm form = new WWWForm();
    //     UnityWebRequest www = UnityWebRequest.Post(API_URL + "getMe", form);
    //     StartCoroutine(SendRequest(www));
    // }

    public void GetUpdates()
    {
        //Debug.Log("LISTENING FOR TELEGRAM UPDATES");
        WWWForm form = new WWWForm();
        UnityWebRequest www = UnityWebRequest.Post($"{API_URL}getUpdates?offset={lastUpdateId}&timeout=10", form);
        StartCoroutine(SendRequest(www, UpdateReceivedLol));
    }



    // public void GetPhotoUrl(string photoId)
    // {
    //     WWWForm form = new WWWForm();
    //     UnityWebRequest www = UnityWebRequest.Post($"{API_URL}getFile?file_id={photoId}", form);
    //     StartCoroutine(SendRequest(www, GetPhoto));
    // }

    // public void GetPhoto(string json)
    // {

    //     Debug.Log(json);
    //     PhotoUpdate data = JsonConvert.DeserializeObject<PhotoUpdate>(json);
    //     string url = $"https://api.telegram.org/file/bot{TOKEN}/{data.result.file_path}";
    //     StartCoroutine(GetText(url));
    // }

    IEnumerator GetText(string url)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded asset bundle
                var texture = DownloadHandlerTexture.GetContent(uwr);
                //skyboxManager.ChangeTexture(texture);
                Debug.Log(texture);
            }
        }
    }

    void UpdateReceivedLol(string json)
    {
        UpdateReceived(json);
        GetUpdates();
    }


    void UpdateReceived(string json)
    {

        // GetUpdates();
        //Debug.Log(json);
        TelegramResult data = JsonConvert.DeserializeObject<TelegramResult>(json);
        //Debug.Log(data.result.Length);
        if (data == null) return;
        if (data.result == null) return;
        if (data.result.Length <= 0) return;
        
        bool found = false;

        foreach (var update in data.result)
        {

            messageReceived?.Invoke();

            if (update.message.text != null)
            {
                Debug.Log($"{update.message.from.first_name} {update.message.from.last_name}: {update.message.text}");
                string text = update.message.text.ToLower();

                // foreach (Message message in messagesqueue)
                // {
                //     if(message.chat.id == update.message.chat.id)
                //     {
                //         message.text += "/n " + update.message.text; 
                //         found=true;
                //     }
                // }

                //if(!found){
                messagesqueue.Enqueue(update.message);
                //}

            }

            // if (update.message.photo != null)
            // {
            //     string photoId = null;
            //     if (update.message.photo.Length > 0 )
            //     {
            //         photoId = update.message.photo[update.message.photo.Length - 1].file_id;
            //         GetPhotoUrl(photoId);
            //     }
            // }


            if (update.update_id > lastUpdateId)
            {
                lastUpdateId = update.update_id;
            }
        }
        conversationactive = true;
        lastUpdateId++;

    }

    public class ChatData
    {
        public Chat chat;
        public List<Message> messages;
    }

    void SaveChatDataAsJson(Message message)
    {
        if (File.Exists(Application.dataPath + $"/CHATS/chatdata-{message.chat.id}.json"))
        {
            string oldChat = File.ReadAllText(Application.dataPath + $"/CHATS/chatdata-{message.chat.id}.json");
            ChatData oldData = JsonConvert.DeserializeObject<ChatData>(oldChat);
            oldData.messages.Add(message);
            string oldjson = JsonConvert.SerializeObject(oldData);
            File.WriteAllText(Application.dataPath + $"/CHATS/chatdata-{oldData.chat.id}.json", oldjson);
        }
        else
        {
            ChatData chatData = new ChatData();
            chatData.messages = new List<Message>();
            chatData.messages.Add(message);
            chatData.chat = message.chat;

            string json = JsonConvert.SerializeObject(chatData);
            File.WriteAllText(Application.dataPath + $"/CHATS/chatdata-{chatData.chat.id}.json", json);
        }
    }

    public void SendFile(byte[] bytes, string filename, int chat_id, string caption = "")
    {
        WWWForm form = new WWWForm();
        form.AddField("chat_id", chat_id);
        form.AddField("caption", caption);
        form.AddBinaryData("document", bytes, filename, "filename");
        UnityWebRequest www = UnityWebRequest.Post(API_URL + "sendDocument?", form);
        StartCoroutine(SendRequest(www, null));
    }

    // public void SendPhoto(Texture2D photo, string filename, int chat_id, string caption = "")
    // {
    //     byte[] bytes = photo.EncodeToPNG();

    //     WWWForm form = new WWWForm();
    //     form.AddField("chat_id", chat_id);
    //     form.AddField("caption", caption);
    //     form.AddBinaryData("photo", bytes, filename, "filename");
    //     UnityWebRequest www = UnityWebRequest.Post(API_URL + "sendPhoto?", form);
    //     StartCoroutine(SendRequest(www, null));
    // }

    public void SendTelegramMessage(string text, string chat_id)
    {
        //Debug.Log(text);
        //Debug.Log(chat_id);
        WWWForm form = new WWWForm();
        form.AddField("chat_id", chat_id);
        form.AddField("text", text);
        UnityWebRequest www = UnityWebRequest.Post(API_URL + "sendMessage?", form);
        messagesentattempt = true;
        StartCoroutine(SendRequest(www, SendComplete));
    }

    void SendComplete(string message)
    {
        modelresponse = null;
        messagesqueue.Dequeue();
        modelfree = true;
        Debug.Log(message);
        messagesentattempt = false;
    }

    IEnumerator SendRequest(UnityWebRequest www, System.Action<string> callback)
    {
        using (UnityWebRequest request = www)
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
                callback(null);
            }
            else
            {
                var w = www;
                if (callback != null)
                {
                    if (www.downloadHandler.text != null)
                    {

                        callback(www.downloadHandler.text);
                    }
                }
            }
        }

    }

    public void CallbackFunction(string response)
    {
        Debug.Log(response);
    }

    // public void SendTelegramHelper(string message, int chatId)
    // {
    //     SendTelegramMessage(message, chatId);
    // }


}