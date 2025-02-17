using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Golem : MonoBehaviour
{
    public bool isRevive=false;
    public bool beHurt=false;
    public float moveSpeed;
    public float health;
    public float damage;
    public float attackRange;
    public float coolDown;
    public int attackTime;
    private int currentAttackTime=0;
    public GameObject playerDetector;
    public GolemTrapManager currentTrapManager;
    public List<GolemTrapManager> TrapManagers = new List<GolemTrapManager>();
    public Collider2D attackRangeCollider;
    private bool isMoving = false;
    private bool isAttacking = false;
    private Transform player;
    private float coolDownTimer=0;
    public int trapIndex = 0;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        if (!isRevive)
        {
            if (playerDetector.GetComponent<GolemPlayerDetector>().playerIn)
            {
                isRevive = true;
            }
            return;
        }

        sr.flipX = ((transform.position.x - player.position.x) > 0);
        float distance = Vector2.Distance(transform.position, player.position);
        if (coolDownTimer <= 0)
        {
            MoveTowardPlayer();
            if (distance <= attackRange)
            {
                rb.velocity = Vector2.zero;
                anim.Play("Attack");
                coolDownTimer = coolDown;
            }
        }
        coolDownTimer -= Time.deltaTime;
    }
    public void getPlayerPosition(Transform player)
    {
        this.player = player;
    }
    public void Attack()
    {
        attackRangeCollider.enabled = true;
    }
    public void StopAttack()
    {
        attackRangeCollider.enabled = false;
    }
    public void Attack_Trap()
    {
        currentAttackTime++;
        if (currentAttackTime==attackTime)
        {
            currentAttackTime = 0;
            beHurt=true;
            anim.Play("BeHurt");
        }
        TrapManagers[trapIndex].TrapOpen();
        int random = Random.Range(1, 3);
        trapIndex = (trapIndex + random) % TrapManagers.Count;
    }
    private void MoveTowardPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity=new Vector2(direction.x,0).normalized*moveSpeed;
    }
    
}
