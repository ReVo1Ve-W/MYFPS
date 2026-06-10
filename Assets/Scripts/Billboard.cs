using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cam;

    void Start()
    {
        if (Camera.main != null)
            cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        Vector3 forward = cam.rotation * Vector3.forward;
        forward.y = 0;

        if (forward.sqrMagnitude > 0.001f)
            transform.forward = forward;
    }
}
