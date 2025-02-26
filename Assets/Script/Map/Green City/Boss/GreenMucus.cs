using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GreenMucus : MonoBehaviour
{
    protected PlayerAInput playerA;
    protected PlayerBInput playerB;
    BossMucusPool bossMucusPool;

    protected int playerObject;

    protected float speed;

    private void OnEnable()
    {
        playerA = FindAnyObjectByType<PlayerAInput>();
        playerB = FindAnyObjectByType<PlayerBInput>();

        bossMucusPool = FindAnyObjectByType<BossMucusPool>();

        speed = bossMucusPool.mucusSpeed;
    }

    private void Update()
    {
        ChaseToPlayer();
    }

    protected virtual void ChaseToPlayer()
    {
        if(playerObject == 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerA.transform.position, speed * Time.deltaTime);
        }
        else if(playerObject == 2)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerB.transform.position, speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player;
        if(collision.TryGetComponent<PlayerController>(out player))
        {
            player.PlayerHurt(1);
            gameObject.SetActive(false);
        }
    }

    public void ChaseToPlayerA()
    {
        playerObject = 1;
    }

    public void ChaseToPlayerB()
    {
        playerObject = 2;
    }
}
