using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class FirebaseCommunicator : MonoBehaviour
{
    IEnumerator CheckForMessagesAndFunctions()
    {
        yield return StartCoroutine(FetchMessagesFirestore());
        yield return StartCoroutine(FetchFunctions());
    }

    IEnumerator FetchMessagesFirestore()
    {
        string firebaseUrl = " https://getdata-ivsiuhqz6q-uc.a.run.app";

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

    IEnumerator FetchFunctions()
    {
        yield return null;
    }

    void ProcessMessage(Message message)
    {
        Debug.Log("Received message: " + message.content);
    }

    [System.Serializable]
    public class MessageData
    {
        public string message;
        public string threadId;

        public string component;
    }

    [System.Serializable]
    public class Message
    {
        public string id;
        public string content;
        public Timestamp timestamp;
        public string type;
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
