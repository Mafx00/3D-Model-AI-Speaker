using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class BuildshipFunctionManager : MonoBehaviour
{
    public static BuildshipFunctionManager Instance { get; private set; }


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ExecuteFunction(string methodName, JObject jsonArguments)
    {
        string functionName = methodName + "Function";

        Debug.Log("LOOKING FOR function: " + methodName + " AND component " + functionName);

        Type componentType = Type.GetType(functionName);

        if (componentType == null)
        {
            Debug.LogError($"Component type {functionName} not found.");
            return;
        }

        Component component = GetComponent(componentType);

        MethodInfo method = componentType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);

        if (method == null)
        {
            Debug.LogError($"Method {methodName} not found in component {functionName}.");
            return;
        }

        ParameterInfo[] parameters = method.GetParameters();
        object[] methodParameters = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            string paramName = parameters[i].Name;
            JToken jsonValue;

            if (jsonArguments.TryGetValue(paramName, out jsonValue))
            {
                methodParameters[i] = jsonValue.ToObject(parameters[i].ParameterType);
            }
            else
            {
                Debug.LogError($"Parameter {paramName} not found in JSON arguments.");
                return;
            }
        }

        // Invoke the method with parameters
        method.Invoke(component, methodParameters);
        ChatMessagesController.Instance.AddChatbotMessage("Performed action *" + methodName + "*");
        Debug.Log($"Invoked method: {methodName} from component: {functionName}");
    }
}