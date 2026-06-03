using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public float HP=100f;
    public GameObject bombEffect;

    void Start()
    {
        
    }

    public void Gethit(float damage)
    {
        HP-=damage;
        if (HP<=0)
        {
           Instantiate(bombEffect,transform.position,Quaternion.identity);
           Destroy(gameObject);
        }
    }
}
