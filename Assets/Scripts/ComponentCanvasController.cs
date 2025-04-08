using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComponentCanvasController : MonoBehaviour
{
    public GameObject componentUI;
    public AudioSource audioDemo;
    TextMeshProUGUI detailText; 

    public float animationSpeed = 5f; 

    private RectTransform imageRectTransform;
    private bool isPopping = false;
    private float targetScale;

    void Start()
    {
        ComponentManager.Instance.onHighlightedComponent.AddListener(UpdateComponentInfo);
        detailText = componentUI.GetComponentInChildren<TextMeshProUGUI>();
        imageRectTransform = componentUI.GetComponent<RectTransform>();
        imageRectTransform.localScale = new Vector3(0,0,0);   
    }

    void Update()
    {
        if (isPopping)
        {
            imageRectTransform.localScale = Vector3.Lerp(imageRectTransform.localScale, Vector3.one * targetScale, Time.deltaTime * animationSpeed);

            if (Vector3.Distance(imageRectTransform.localScale, Vector3.one * targetScale) < 0.01f)
            {
                isPopping = false;
            }
        }
    }

    public void PopUp()
    {
        targetScale = 1;
        isPopping = true;
    }

    public void PopDown()
    {
        targetScale = 0;
        isPopping = true;
    }

    void UpdateComponentInfo(Transform component)
    {
        ComponentInfo componentInfo = component.GetComponent<ComponentInfo>();
        if(componentInfo == null)
        {
            PopDown();
            audioDemo.Stop();
        }
        else
        {
            //componentUI.SetActive(true);
            PopUp();
            detailText.text = componentInfo.description;
            audioDemo.clip = componentInfo.audioDemo;
        }
    }

}
