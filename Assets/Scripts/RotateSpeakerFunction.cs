using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RotateSpeakerFunction : MonoBehaviour
{
    public UnityEvent ExecuteFunctionEvent;

    public GameObject speakerRoot;

    public float rotationSpeed = 90f;

    private Coroutine rotationCoroutine;
    

    public void RotateSpeaker(int degrees)
    {
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
        }

        rotationCoroutine = StartCoroutine(RotateOverTime(degrees));
    }

    private IEnumerator RotateOverTime(int targetDegrees)
    {
        float totalRotation = 0f;
        float rotationStep = targetDegrees > 0 ? rotationSpeed : -rotationSpeed;

        while (Mathf.Abs(totalRotation) < Mathf.Abs(targetDegrees))
        {
            float rotationAmount = rotationStep * Time.deltaTime;
            speakerRoot.transform.Rotate(0f, rotationAmount, 0.0f, Space.Self);
            totalRotation += rotationAmount;

            yield return null;
        }

        float finalRotation = targetDegrees - totalRotation;
        speakerRoot.transform.Rotate(0f, finalRotation, 0.0f, Space.Self);

        ExecuteFunctionEvent.Invoke();
    }
}
