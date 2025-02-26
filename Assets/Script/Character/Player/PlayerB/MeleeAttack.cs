using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class MeleeAttack : MonoBehaviour
{
    [Header("近战攻击")]
    public bool isMeleeAttack;
    public float meleeAttackDamage;//近战攻击伤害
    public Vector2 attackSize = new Vector2(1f, 1f);//攻击范围的尺寸
    public float offsetX = 1f;//X轴的偏移量
    public float offsetY = 1f;//Y轴的偏移量
    public LayerMask enemyLayer;//表示敌人图层
    public LayerMask destructibleLayer;//表示可破坏物品图层
    private Vector2 AttackAreaPos;

    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        AttackAreaPos = transform.position;
    }

    void MeleeAttackAnimEvent()
    {
        //中心偏移量
        AttackAreaPos = transform.position;

        //是否翻转
        offsetX = sr.flipX ? -Mathf.Abs(offsetX) : Mathf.Abs(offsetX);

        AttackAreaPos.x += offsetX;
        AttackAreaPos.y += offsetY;

        Collider2D[] enemyHitColliders = Physics2D.OverlapBoxAll(AttackAreaPos, attackSize, 0f, enemyLayer);    

        foreach (Collider2D hitCollider in enemyHitColliders)
        {
            hitCollider.GetComponent<EnemyController>().TakeDamage(meleeAttackDamage );
        }
    }

    //绘图用于测试
    private void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(AttackAreaPos, attackSize);
    }
}
