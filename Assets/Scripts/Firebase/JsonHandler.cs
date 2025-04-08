using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class JsonHandler
{
    public static void ProcessJson(string jsonString)
    {
        try
        {
            JObject json = JObject.Parse(jsonString);
            string type = (string)json["type"];

            if (type == "message")
            {
                string messageText = (string)json["value"]["message"];
                Debug.Log("Message Text: " + messageText);
            }
            else if (type == "function")
            {
                string functionName = (string)json["value"]["name"];
                JObject arguments = (JObject)json["value"]["arguments"];

                Debug.Log("Function Call: " + functionName);
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
}