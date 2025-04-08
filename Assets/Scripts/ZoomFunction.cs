using UnityEngine;
using UnityEngine.Events;
using static BuildshipCommunicator;
using static ComponentManager;

public class ZoomFunction : MonoBehaviour
{
    public UnityEvent ExecuteFunctionEvent;

    // Update is called once per frame
    public void Zoom(ComponentType component)
    {
        ComponentManager.Instance.ZoomIn(component);
    }
}
