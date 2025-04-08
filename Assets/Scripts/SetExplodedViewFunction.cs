using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class SetExplodedViewFunction : MonoBehaviour
{
    public UnityEvent ExecuteFunctionEvent;

    public float totalSpreadSize = 10.0f; 
    public float spreadSpeed = 5.0f; 
    public Transform speakerRoot;

    bool explodeModeActive = false;
    private Vector3[] targetPositions;
    private Vector3[] initialPositions;
    private Quaternion[] initialRotations;
    private Transform[] childrenTransforms; 

    private Coroutine explodeCoroutine;

    void Start()
    {
        childrenTransforms = new Transform[speakerRoot.childCount];
        targetPositions = new Vector3[speakerRoot.childCount];
        initialPositions = new Vector3[speakerRoot.childCount];
        initialRotations = new Quaternion[speakerRoot.childCount];

        for (int i = 0; i < speakerRoot.childCount; i++)
        {
            childrenTransforms[i] = speakerRoot.GetChild(i);
            initialPositions[i] = childrenTransforms[i].localPosition;
            initialRotations[i] = childrenTransforms[i].localRotation;
        }

        ComponentManager.Instance.onHighlightedComponent.AddListener(OnHighlightedComponentChanged);
    }

    private void OnHighlightedComponentChanged(Transform highlightComponent)
    {
        ResetView(highlightComponent);
        if (explodeCoroutine != null)
        StopCoroutine(explodeCoroutine);
    }

    void ResetView(Transform highlightComponent)
    {
        for (int i = 0; i < childrenTransforms.Length; i++)
        {
            if(highlightComponent != childrenTransforms[i])
            childrenTransforms[i].localPosition = initialPositions[i];
            childrenTransforms[i].localRotation = initialRotations[i];
        }
    }

    public void SetExplodedView(bool setActive)
    {
        explodeModeActive = setActive;

        if (explodeModeActive)
        {
            float segmentLength = totalSpreadSize / (childrenTransforms.Length - 1);

            for (int i = 0; i < childrenTransforms.Length; i++)
            {
                float zPosition = -totalSpreadSize / 2 + segmentLength * i;

                targetPositions[i] = new Vector3(0, 0, zPosition);
            }

            if (explodeCoroutine != null)
            {
                StopCoroutine(explodeCoroutine);
            }
            explodeCoroutine = StartCoroutine(ExplodeViewCoroutine());
        }
        else
        {
            if (explodeCoroutine != null)
            {
                StopCoroutine(explodeCoroutine);
            }
            explodeCoroutine = StartCoroutine(CollapseViewCoroutine());
        }

        ExecuteFunctionEvent.Invoke();
    }

    private IEnumerator ExplodeViewCoroutine()
    {
        while (explodeModeActive)
        {
            bool allClose = true;
            for (int i = 0; i < childrenTransforms.Length; i++)
            {
                childrenTransforms[i].localPosition = Vector3.MoveTowards(childrenTransforms[i].localPosition, targetPositions[i], spreadSpeed * Time.deltaTime);
                if (Vector3.Distance(childrenTransforms[i].localPosition, targetPositions[i]) > 0.05f)
                {
                    allClose = false;
                }
            }
            if (allClose)
            {
                explodeModeActive = false;
            }
            yield return null;
        }
    }

    private IEnumerator CollapseViewCoroutine()
    {
        while (!explodeModeActive)
        {
            bool allClose = true;
            for (int i = 0; i < childrenTransforms.Length; i++)
            {
                childrenTransforms[i].localPosition = Vector3.MoveTowards(childrenTransforms[i].localPosition, initialPositions[i], spreadSpeed * Time.deltaTime);
                if (Vector3.Distance(childrenTransforms[i].localPosition, initialPositions[i]) > 0.05f)
                {
                    allClose = false;
                }
            }
            if (allClose)
            {
                explodeModeActive = true;
            }
            yield return null;
        }
    }
}