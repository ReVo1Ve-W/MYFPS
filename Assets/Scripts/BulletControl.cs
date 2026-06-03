using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    public float speed=55f;

    public GameObject hitEffect;
    public float lifeTime=2f;
    private Rigidbody rb;


    void Start()
    {
        rb=GetComponent<Rigidbody>();
        rb.AddForce(transform.forward*speed,ForceMode.Impulse);
        Destroy(gameObject,lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var effect= Instantiate(hitEffect,transform.position,Quaternion.LookRotation(collision.contacts[0].normal)); 
        Destroy(effect,1f);
        Destroy(gameObject);
    }
}
