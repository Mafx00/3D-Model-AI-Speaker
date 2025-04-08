using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class FirebaseListener : MonoBehaviour
{
    IEnumerator Start()
    {
        StartCoroutine(WaitForNewMessage());
        yield return null;
    }

    IEnumerator WaitForNewMessage()
    {
        string buildshipUrl = "https://us-central1-ai3d-b602b.cloudfunctions.net/getData";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(buildshipUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string json = webRequest.downloadHandler.text;
                //check the response from buildship.
                if (json.Contains("New message ready"))
                {
                    StartCoroutine(FetchMessages());
                }
            }
            else
            {
                Debug.LogError("Error waiting for new message: " + webRequest.error);
            }
        }

        yield return null;
        
    }

    void ProcessMessage(Message message)
    {
        if (message.type == "function_available") 
        {
            Debug.Log("Function available: " + message.content);
            // Handle function availability message
            HandleFunctionAvailable(message.content);
        }
        else if (message.type == "regular_message") 
        {
            Debug.Log("Regular message: " + message.content);
            HandleRegularMessage(message.content);
        }
        else
        {
            Debug.Log("Unknown message: " + message.content);
        }
    }

    void HandleFunctionAvailable(string content)
    {

    }

    void HandleRegularMessage(string content)
    {

    }

    IEnumerator FetchMessages()
    {
        string firebaseUrl = "YOUR_GET_MESSAGES_FUNCTION_URL";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(firebaseUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string json = webRequest.downloadHandler.text;
                Message[] messages = JsonUtility.FromJson<MessageArrayWrapper>("{\"messages\":" + json + "}").messages;

                if (messages != null && messages.Length > 0)
                {
                    foreach (Message message in messages)
                    {
                        ProcessMessage(message);
                    }
                }
            }
            else
            {
                Debug.LogError("Error fetching messages: " + webRequest.error);
            }
        }
    }

    [System.Serializable]
    public class Message
    {
        public string id;
        public string content;
        public Timestamp timestamp;
        public String type;
    }

    [System.Serializable]
    public class Timestamp
    {
        public int seconds;
        public int nanoseconds;
    }

    [System.Serializable]
    public class MessageArrayWrapper
    {
        public Message[] messages;
    }
}