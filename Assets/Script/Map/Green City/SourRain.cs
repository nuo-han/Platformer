using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourRain : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerController player;
        if(collision.TryGetComponent<PlayerController>(out player) && !player.isHaveUmbrella)
        {
            player.PlayerHurt(1);
            StartCoroutine(player.PlayerInvincibleCoroutine());
        }
    }
}
