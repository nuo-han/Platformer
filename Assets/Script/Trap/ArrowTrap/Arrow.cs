using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Arrow : MonoBehaviour
{
    public float speed = 10.0f; // 箭矢的速度
    public float lifetime = 5.0f; // 箭矢的存活时间
    public float damage = 10.0f; // 箭矢的伤害值
    public Vector2 direction=new Vector2(1,0); // 箭矢的飞行方向
    private Rigidbody2D rb; // 箭矢的 Rigidbody2D 组件

    public ObjectPool<GameObject> arrowPool;//箭矢对象池
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction * speed; // 箭矢沿右方向飞行
        Destroy(gameObject, lifetime); // 设置箭矢的存活时间
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerA") || collision.CompareTag("PlayerB"))
        {
            // 玩家受到伤害
            collision.GetComponent<PlayerController>().PlayerHurt(damage);
        }

        arrowPool.Release(gameObject); // 箭矢碰撞后销毁
    }
}
