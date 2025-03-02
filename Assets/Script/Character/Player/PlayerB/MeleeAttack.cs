using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class MeleeAttack : MonoBehaviour
{
    [Header("��ս����")]
    public bool isMeleeAttack;
    public float meleeAttackDamage;//��ս�����˺�
    public Vector2 attackSize = new Vector2(1f, 1f);//������Χ�ĳߴ�
    public float offsetX = 1f;//X���ƫ����
    public float offsetY = 1f;//Y���ƫ����
    public LayerMask enemyLayer;//��ʾ����ͼ��
    public LayerMask destructibleLayer;//��ʾ���ƻ���Ʒͼ��
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
        //����ƫ����
        AttackAreaPos = transform.position;

        //�Ƿ�ת
        offsetX = sr.flipX ? -Mathf.Abs(offsetX) : Mathf.Abs(offsetX);

        AttackAreaPos.x += offsetX;
        AttackAreaPos.y += offsetY;

        Collider2D[] enemyHitColliders = Physics2D.OverlapBoxAll(AttackAreaPos, attackSize, 0f, enemyLayer);    

        foreach (Collider2D hitCollider in enemyHitColliders)
        {
            hitCollider.GetComponent<EnemyController>().TakeDamage(meleeAttackDamage );
        }
    }

    //��ͼ���ڲ���
    private void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(AttackAreaPos, attackSize);
    }
}
