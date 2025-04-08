using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateWithMouseInput : MonoBehaviour
{
    public bool isEnabled;
    public float rotationSpeed = 100.0f; 
    private bool isDragging = false;
    private Vector3 lastMousePosition;

    public Transform targetObject;
    public LayerMask uiBlockerLayer; 
    void Start()
    {
        ComponentManager.Instance.onHighlightedComponent.AddListener(newHighlight => targetObject = newHighlight);
    }

    void Update()
    { 
        if(!isEnabled)
            return;
    
        if (Input.GetMouseButtonDown(0) && !IsMouseOverUILayer())
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 deltaMousePosition = Input.mousePosition - lastMousePosition;

            float rotationX = deltaMousePosition.y * rotationSpeed * Time.deltaTime;
            float rotationY = -deltaMousePosition.x * rotationSpeed * Time.deltaTime;

            targetObject.Rotate(Vector3.up, rotationY, Space.World);
            targetObject.Rotate(Vector3.left, rotationX, Space.Self);

            lastMousePosition = Input.mousePosition;
        }
    }

    private bool IsMouseOverObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == targetObject)
            {
                return true;
            }
        }
        return false;
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
private bool IsMouseOverUILayer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (((1 << hit.collider.gameObject.layer) & uiBlockerLayer) != 0)
            {
                return true;
            }
        }
        return false;
    }
}