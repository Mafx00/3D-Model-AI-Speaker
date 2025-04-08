using UnityEngine;
using static ComponentManager;

[System.Serializable]
    public class ComponentInfo : MonoBehaviour
    {
        public string componentName;
        public string description;
        public ComponentType type;
        public GameObject componentObject;
        public AudioClip audioDemo;

    void Start()
    {
        componentName = gameObject.name;
        componentObject = gameObject;
    }
}
