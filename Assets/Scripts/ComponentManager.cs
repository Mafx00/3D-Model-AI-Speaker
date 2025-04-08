using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static BuildshipCommunicator;

public class ComponentManager : MonoBehaviour
{
    public Transform speakerRoot;
    public List<ComponentInfo> components;

    public static ComponentType highlightedComponent = ComponentType.Speaker;

    public bool zommedIn = false;

    public UnityEvent<Transform> onHighlightedComponent  = new UnityEvent<Transform>();

    public enum ComponentType { Speaker, Housing, Tweeter, Crossover, Woofer, Subwoofer }

    public static ComponentManager Instance { get; private set; }
    private GameObject lastHoveredObject;
    private Vector3 zoomedInPosition;
    Shader outlineShader;

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

        for (int i = 0; i < speakerRoot.childCount; i++)
        {
            if (speakerRoot.GetChild(i).GetComponent<ComponentInfo>() != null)
                components.Add(speakerRoot.GetChild(i).GetComponent<ComponentInfo>());            
        }

        zoomedInPosition = speakerRoot.transform.position;
        outlineShader = Resources.Load<Shader>("HighlightShader");
    }


void Update()
{
    HandleHoverEffect();

    if (Input.GetMouseButtonDown(0))
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        LayerMask blockLayer = LayerMask.NameToLayer("UIBlocker");

        if (Physics.Raycast(ray, out hit))
        {
            ComponentInfo clickedComponent = hit.transform.GetComponent<ComponentInfo>();
            if (clickedComponent != null)
            {
                if (highlightedComponent != clickedComponent.type)
                {
                    ZoomIn(clickedComponent);
                }
            }
        }
        else if(!IsMouseOverUI()) 
        {
            ResetZoom();
        }
    }
}
private bool IsMouseOverUI()
{
    PointerEventData pointerData = new PointerEventData(EventSystem.current)
    {
        position = Input.mousePosition
    };

    List<RaycastResult> results = new List<RaycastResult>();
    EventSystem.current.RaycastAll(pointerData, results);

    return results.Count > 0;
}
        public bool IsMouseNotOverUI()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        if (results.Count > 0)
        {
            if ((results[0].gameObject.layer & LayerMask.NameToLayer("UI")) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return true;
    }


    public void DisableAllComponents()
    {
        foreach (var component in components)
        {
            component.gameObject.SetActive(false);
        }
    }

    public void EnableAllComponents()
    {
        foreach (var component in components)
        {
            component.gameObject.SetActive(true);
        }
    }

    ComponentInfo FindComponentFromType(ComponentType type)
    {
        foreach (var component in components)
        {
            if(component.type == type)
            return component;
        }
        return null;
    }

    public void ZoomIn(ComponentInfo component)
    {
        Debug.Log("ZOOMING IN ON " + component.name);
        if(component.type == ComponentType.Speaker)
        return;

        DisableAllComponents();
        component.gameObject.SetActive(true);
        //component.gameObject.transform.position = zoomedInPosition;
        component.gameObject.transform.localPosition = new Vector3(0,0,0);
        highlightedComponent = component.type;

        onHighlightedComponent.Invoke(component.componentObject.transform);
    }

    public void ZoomIn(ComponentType type)
    {
        ComponentInfo component = FindComponentFromType(type);

        if(component == null)
        ResetZoom();
        else
        {
            DisableAllComponents();
            component.gameObject.SetActive(true);
            component.gameObject.transform.position = zoomedInPosition;
            //component.gameObject.transform.localPosition = new Vector3(0,0,0);
            highlightedComponent = component.type;

            onHighlightedComponent.Invoke(component.componentObject.transform);
        }
    }

    public void ResetZoom()
    {
        Debug.Log("RESET ZOOM");

        highlightedComponent = ComponentType.Speaker;
        EnableAllComponents();
        onHighlightedComponent.Invoke(speakerRoot.transform);
    }

void HandleHoverEffect()
{
    Vector3 mousePos = Input.mousePosition;
#if UNITY_WEBGL && !UNITY_EDITOR
    mousePos.y = Screen.height - mousePos.y; // WebGL fix (only if needed)
#endif

    Ray ray = Camera.main.ScreenPointToRay(mousePos);
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit))
    {
        ComponentInfo hoveredComponent = hit.transform.GetComponent<ComponentInfo>();

        if (hoveredComponent != null)
        {

            if (lastHoveredObject != hit.collider.gameObject)
            {
                if (lastHoveredObject != null)
                {
                    RemoveOutline(lastHoveredObject);
                }

                ApplyOutline(hit.collider.gameObject);
                lastHoveredObject = hit.collider.gameObject;
            }
        }
    }
    else
    {

        if (lastHoveredObject != null)
        {
            RemoveOutline(lastHoveredObject);
            lastHoveredObject = null;
        }
    }
}
    void ApplyOutline(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
        Material[] materials = renderer.materials;
        Material[] newMaterials = new Material[materials.Length + 1];

            for (int i = 0; i < materials.Length; i++)
            {
                newMaterials[i] = materials[i];
            }
            newMaterials[materials.Length] = new Material(Shader.Find("Unlit/HighlightShader"));
            renderer.materials = newMaterials;
        }        
    }
    

    void RemoveOutline(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material[] materials = renderer.materials;
            if (materials.Length > 1 && materials[materials.Length - 1].shader.name == "Unlit/HighlightShader")
            {
            Material[] newMaterials = new Material[materials.Length - 1];
                for (int i = 0; i < newMaterials.Length; i++)
                {
                 newMaterials[i] = materials[i];
                }   
                renderer.materials = newMaterials;
            }
        }
    }
}
