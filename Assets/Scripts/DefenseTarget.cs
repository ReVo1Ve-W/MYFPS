using UnityEngine;

public class DefenseTarget : MonoBehaviour
{
    public float maxHP = 500f;
    public float HP { get; private set; }

    void Start()
    {
        HP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        HP -= damage;
        HP = Mathf.Max(0, HP);

        if (HP <= 0)
            Debug.Log("防御目标被摧毁！游戏结束！");
    }
}
