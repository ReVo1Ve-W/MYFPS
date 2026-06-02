using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    //速度
    public float moveSpeed = 5f;

    public float jumpForce=5f;
    //灵敏度
    public float xSensitivity = 10;

    public float ySensitivity = 10;

    private float xRotation = 0;
    private Rigidbody rb;
    private Animator anim;
    private Vector3 velocity;
    private bool jump=false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        Cursor.lockState=CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Mouse();
        HighSpeed();
        Move();
        Jump();
        //transform.position+Vector3.up*0.2f,-Vector3.up,out hit,
        //Debug.DrawRay(transform.position+Vector3.up*0.2f,-Vector3.up*0.4f,Color.red);
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
    void Move()
    {
        //获取输入
        float horizontal = Input.GetAxis("Horizontal");  
        float vertical = Input.GetAxis("Vertical");
        //计算移动方向
        Vector3 dir = transform.forward * vertical + transform.right * horizontal;
        dir.Normalize();
        //速度
        velocity = dir * moveSpeed;
        velocity.y = rb.velocity.y;
        //动画
        anim.SetFloat("Movement", dir.magnitude);
       
    }

    void HighSpeed()
    {
        if(Input.GetKey(KeyCode.LeftShift)&&IsGround())
        {
            moveSpeed = 7;
            anim.SetBool("Holstered",true);
        }
        else
        {
            moveSpeed = 5;
             anim.SetBool("Holstered",false);

        }
    }

    void Jump()
    {
        if (Input.GetKey(KeyCode.Space)&&IsGround())
        {
            jump=true;
        }
    }

    public bool IsGround()
    {
        RaycastHit hit;
        bool res = Physics.Raycast(transform.position+Vector3.up*0.2f,-Vector3.up,out hit,0.4f,LayerMask.GetMask("Ground"));
        return res;
    }




    private void FixedUpdate()
    {
        if (jump)
        {
            jump=false;
            velocity.y=jumpForce;
        }
        rb.velocity=velocity;
    }




}
