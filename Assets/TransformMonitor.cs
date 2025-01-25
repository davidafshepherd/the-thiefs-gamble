using UnityEngine;

public class TransformMonitor : MonoBehaviour
{
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private Vector3 lastScale;

    void Start()
    {
        Debug.Log("Initialised Transform Monitor");
        lastPosition = transform.position;
        lastRotation = transform.rotation;
        lastScale = transform.localScale;
    }

    void Update()
    {
        if (transform.position != lastPosition)
        {
            Debug.Log($"Position changed from {lastPosition} to {transform.position}\n{StackTraceLog()}");
            lastPosition = transform.position;
        }

        if (transform.rotation != lastRotation)
        {
            Debug.Log($"Rotation changed from {lastRotation.eulerAngles} to {transform.rotation.eulerAngles}\n{StackTraceLog()}");
            lastRotation = transform.rotation;
        }

        if (transform.localScale != lastScale)
        {
            Debug.Log($"Scale changed from {lastScale} to {transform.localScale}\n{StackTraceLog()}");
            lastScale = transform.localScale;
        }
    }

    private string StackTraceLog()
    {
        return System.Environment.StackTrace;
    }
}
