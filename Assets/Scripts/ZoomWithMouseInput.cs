using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ZoomWithMouseInput : MonoBehaviour
{
    public Transform targetObject; 
    public float maxDistance = 60f; 
    public float minDistance = 20f; 
    private float normalDistance = 40f;
    public float zoomSpeed = 10f;
    public LayerMask uiBlockerLayer; 
    private Vector3 initialOffset; 

    void Start()
    {
        initialOffset = targetObject.position - Camera.main.transform.position;

        normalDistance = Vector3.Distance(Camera.main.transform.position, targetObject.position);

        ComponentManager.Instance.onHighlightedComponent.AddListener(newHighlight => targetObject = newHighlight);
    }

    void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            if (scrollInput != 0 && IsMouseOverObject())
            {
                Vector3 direction = initialOffset.normalized;
                float distance = Vector3.Distance(Camera.main.transform.position, targetObject.position);
                distance -= scrollInput * zoomSpeed;
                distance = Mathf.Clamp(distance, minDistance, maxDistance);

                targetObject.position = Camera.main.transform.position + direction * distance;
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

    public void ResetZoom()
    {
        Vector3 direction = initialOffset.normalized;
        targetObject.position = Camera.main.transform.position + direction * normalDistance;
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