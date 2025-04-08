using TMPro;
using UnityEngine;

public class ChatMessagesController : MonoBehaviour
{
    public GameObject userMessage;
    public GameObject chatbotMessage;
    public Transform messagesParent;

    public static ChatMessagesController Instance { get; private set; }

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

    public void AddUserMessage(string message)
    {
        GameObject instantiatedObject = Instantiate(userMessage, messagesParent);

        TextMeshProUGUI textMeshPro = instantiatedObject.GetComponentInChildren<TextMeshProUGUI>();

        if (textMeshPro != null)
        {
            textMeshPro.text += message;
        }
    }

    public void AddChatbotMessage(string message)
    {
        GameObject instantiatedObject = Instantiate(chatbotMessage, messagesParent);

        TextMeshProUGUI textMeshPro = instantiatedObject.GetComponentInChildren<TextMeshProUGUI>();

        if (textMeshPro != null)
        {
            textMeshPro.text += message;
        }
    }

}
