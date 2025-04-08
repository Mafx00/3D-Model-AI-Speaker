using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine.Events;
using static ComponentManager;

public class BuildshipCommunicator : MonoBehaviour
{
    public string buildshipUrl = "https://6t028j.buildship.run/ai-chatbot-362201272110"; 
    public string _threadId = "";

    public UnityEvent onRequestSentEvent;
    public UnityEvent onMessageRecievedEvent;
    private Coroutine promptToAICoroutine;
    public float minInactivityTime = 30f;
    public float maxInactivityTime = 60f;

    void Start()
    {
        CanvasController.Instance.OnMessageSent.AddListener(SendMessageToBuildship);
        StartInactivityCoroutine();
    }

    public void SendMessageToBuildship(string userText)
    {
        StartCoroutine(SendPostRequest(_threadId, userText, ComponentManager.highlightedComponent));
        ResetInactivityCoroutine();
    }

    IEnumerator SendPostRequest(string threadId, string message, ComponentType component)
    {
        string json = "{\"threadId\": \"" + threadId + "\", \"message\": \"" + message + "\", \"component\": \"" + component.ToString() + "\"}";

        Debug.Log("SENDING MESSAGE TO BUILDSHIP: " + json);

        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest webRequest = new UnityWebRequest(buildshipUrl, "POST"))
        {
            onRequestSentEvent.Invoke();

            webRequest.uploadHandler = new UploadHandlerRaw(jsonBytes);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("BUILDSHIP REQUEST SUCCESS: " + webRequest.downloadHandler.text);
                Debug.Log("Response Headers: " + GetResponseHeaders(webRequest));

                onMessageRecievedEvent.Invoke();
                ProcessBuildshipResponse(webRequest.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Buildship request failed: " + webRequest.error);
                Debug.LogError("Response Headers: " + GetResponseHeaders(webRequest));
            }
        }
    }

    private string GetResponseHeaders(UnityWebRequest webRequest)
    {
        if (webRequest.GetResponseHeaders() != null)
        {
            string headers = "";
            foreach (KeyValuePair<string, string> header in webRequest.GetResponseHeaders())
            {
                headers += header.Key + ": " + header.Value + "\n";
            }
            return headers;
        }
        return "Response Headers: null";
    }

    public void ProcessBuildshipResponse(string jsonString)
    {
        try
        {
            JObject json = JObject.Parse(jsonString);

            if (_threadId == "")
                _threadId = json["threadId"].ToString();

            string type = (string)json["type"];

            if (type == "message")
            {
                string messageText = (string)json["value"]["message"];
                CanvasController.Instance.DisplayMessage(messageText);
            }
            else if (type == "function")
            {
                string functionName = (string)json["value"]["name"];
                JObject arguments = (JObject)json["value"]["arguments"];

                BuildshipFunctionManager.Instance.ExecuteFunction(functionName, arguments);

                Debug.Log("FUNCTION CALL: " + functionName);
                Debug.Log("Arguments: " + arguments.ToString());
            }
            else
            {
                Debug.LogError("Unknown JSON type: " + type);
            }
        }
        catch (JsonException e)
        {
            Debug.LogError("Error parsing JSON: " + e.Message);
        }
        catch (System.NullReferenceException e)
        {
            Debug.LogError("Null reference Exception: " + e.Message);
        }
    }

    private void StartInactivityCoroutine()
    {
        promptToAICoroutine = StartCoroutine(SendEmptyMessageAfterInactivity());
    }

    private void ResetInactivityCoroutine()
    {
        if (promptToAICoroutine != null)
        {
            StopCoroutine(promptToAICoroutine);
        }
        StartInactivityCoroutine();
    }

    private IEnumerator SendEmptyMessageAfterInactivity()
    {
        while (true)
        {
            float waitTime = Random.Range(minInactivityTime, maxInactivityTime);
            yield return new WaitForSeconds(waitTime);

            SendMessageToBuildship(" ");
            Debug.Log("Sending empty message ");
        }
    }
}