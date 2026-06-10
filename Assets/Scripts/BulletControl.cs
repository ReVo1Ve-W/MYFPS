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
    //打到可破坏物
        if (collision.gameObject.tag=="Des")
        {
            Rigidbody rigidbody= collision.gameObject.GetComponent<Rigidbody>();
            if (rigidbody==null)
            {
                rigidbody= collision.gameObject.AddComponent<Rigidbody>();
            }
            rigidbody.AddForceAtPosition(transform.forward*5,collision.contacts[0].point,ForceMode.Impulse);
            //Destroy(collision.gameObject.GetComponent<Collider>(),0.5f);
            Destroy(collision.gameObject,2f);
        }
    //打到敌人
        if (collision.gameObject.tag=="Enemy")
        {
            EnemyControl enemy=collision.gameObject.GetComponent<EnemyControl>();
            if (enemy!=null)
            {
                enemy.Gethit(20f);
            }
        }

        var effect= Instantiate(hitEffect,transform.position,Quaternion.LookRotation(collision.contacts[0].normal)); 
        Destroy(effect,1f);
        Destroy(gameObject);
    }
}
