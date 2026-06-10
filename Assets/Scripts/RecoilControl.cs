using UnityEngine;

public class RecoilControl : MonoBehaviour
{
    public float X = -3f;
    public float speed = 10;

    private float targetRotation;
    private float currentRotation;

    void Update()
    {
        targetRotation = Mathf.Lerp(targetRotation, 0, speed * Time.deltaTime);
        currentRotation = Mathf.Lerp(currentRotation, targetRotation, speed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation, transform.localEulerAngles.y, 0);
    }

    public void Fire()
    {
        targetRotation += X;
    }
}
