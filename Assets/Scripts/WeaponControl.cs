using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponControl : MonoBehaviour
{
    
    //发射位置
    public GameObject FirePoint;
    //子弹
    public GameObject BulletPre;
    //火焰效果
    public GameObject FirePre;
    //攻击间隔
    public float bulletInterval=0.3f;
    private float timer=0;
    private PlayerControl pc;
    private RecoilControl rc;
    void Start()
    {
        rc=GetComponent<RecoilControl>();
        pc=GetComponent<PlayerControl>();
    }

    void Update()
    {
        timer+=Time.deltaTime;
        if (Input.GetMouseButton(0)&&timer>=bulletInterval&&!pc.highSpeed)
        {
        timer=0;
        //后坐力
        rc.Fire();
        //发射子弹
        Instantiate(BulletPre,FirePoint.transform.position,FirePoint.transform.rotation);
        //火焰效果
        var effect= Instantiate(FirePre,FirePoint.transform.position,FirePoint.transform.rotation);            Destroy(effect,0.1f);
        }
    }
}
