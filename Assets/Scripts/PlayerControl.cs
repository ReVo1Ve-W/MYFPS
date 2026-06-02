using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    //速度
    public float moveSpeed = 5f;
    //灵敏度
    public float xSensitivity = 10;
    public float ySensitivity = 10;

    private float xRotation = 0;
    private float yRotation = 0;
    private Rigidbody rb;
    private Animator anim;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Mouse();
    }
    //鼠标旋转
    void Mouse()
    {
        //获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y"); 

        //上下旋转
        xRotation -= mouseY*ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        anim.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        //左右旋转
        transform.Rotate(Vector3.up*xSensitivity*mouseX);
    }






}
