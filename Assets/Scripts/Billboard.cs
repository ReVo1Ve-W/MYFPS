using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main == null) return;

        Vector3 targetPosition = transform.position + Camera.main.transform.rotation * Vector3.forward;
        Vector3 targetOrientation = targetPosition - transform.position;
        targetOrientation.y = 0;

        if (targetOrientation.sqrMagnitude > 0.001f)
            transform.forward = targetOrientation;
    }
}
