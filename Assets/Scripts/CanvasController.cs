using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public static CanvasController Instance { get; private set; }

    public GameObject details;
    private TextMeshProUGUI detailsText;
    private AudioSource detailsAudioDemo;

    //public TMP_Text chatbotMessages;

    public RectTransform chat;

    private TMP_InputField chatInput;
    private bool animatingChat;
    public Button sendButton;
    public float animationSpeed = 5f; // Adjust for desired speed

    private float hideChatHeight = 20;
    private float showChatHeight;
    private float targetHeight;
    public UnityEvent<string> OnMessageSent = new UnityEvent<string>();

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

        sendButton.onClick.AddListener(SendMessage);
        detailsText = details.GetComponentInChildren<TextMeshProUGUI>();
        chatInput = chat.GetComponentInChildren<TMP_InputField>();
        detailsAudioDemo = chat.GetComponentInChildren<AudioSource>();
        showChatHeight = chat.sizeDelta.y;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendMessage();
        }

        if (animatingChat)
        {
            float currentHeight = chat.sizeDelta.y;
            if (Mathf.Abs(currentHeight - targetHeight) > 0.01f) 
            {
                float newHeight = Mathf.Lerp(currentHeight, targetHeight, Time.deltaTime * animationSpeed);
                chat.sizeDelta = new Vector2(chat.sizeDelta.x, newHeight);
            }
            else
            {
                chat.sizeDelta = new Vector2(chat.sizeDelta.x, targetHeight);
                animatingChat = false;
            }
        }
    }

    public void SendMessage()
    {
        string message = chatInput.text;
        if (!string.IsNullOrEmpty(message))
        {
            Debug.Log("SENT MESSAGE BUTTON " + chatInput.text);
            OnMessageSent.Invoke(chatInput.text);
            ChatMessagesController.Instance.AddUserMessage(chatInput.text);
            chatInput.text = "";
        }
    }

    public void DisplayMessage(string message)
    {
        //chatbotMessages.text = "";
        //chatbotMessages.text = message;
        Debug.Log("Displaying message: " + message);
        ChatMessagesController.Instance.AddChatbotMessage(message);
    }

    public void ShowDetails(string component, string detailsInfo)
    {
        details.SetActive(true);
        detailsText.text = $"<b>{component}</b>\n{detailsInfo}";
    }

    public void HideDetails()
    {
        details.SetActive(false);

    }

    public void hideChat(bool hide)
    {
        if(hide)
            targetHeight = hideChatHeight; 
        else
            targetHeight = showChatHeight;

        animatingChat = true;
    }


}