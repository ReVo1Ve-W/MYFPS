using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilControl : MonoBehaviour
{
    public float X=-3f;
    public float speed=10;
    public float returnSpeed=6;

    private float targetRotation;
    private float currentRotation;


    // Update is called once per frame
    void Update()
    {
        //恢复
        targetRotation=Mathf.Lerp(targetRotation,0,speed*Time.deltaTime);
        //旋转
        currentRotation=Mathf.Lerp(currentRotation,targetRotation,speed*Time.deltaTime);
        //应用
        transform.localRotation=Quaternion.Euler(currentRotation,transform.localEulerAngles.y,0);
    }

    public void Fire()
    {
        targetRotation+=X;
    }



}
