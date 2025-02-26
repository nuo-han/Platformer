using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatformController : MonoBehaviour
{
    [SerializeField] MovePlatform movePlatform;
    bool isPlayerAInPlatform = false;
    bool isPlayerBInPlatform = false;

    private void FixedUpdate()
    {
        if ((isPlayerAInPlatform && Input.GetKeyDown(KeyCode.DownArrow)) || (isPlayerBInPlatform && Input.GetKeyDown(KeyCode.S)))
        {
            movePlatform.isActive = !movePlatform.isActive;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerA"))
        {
            isPlayerAInPlatform = true;
        }
        else if(collision.CompareTag("PlayerB"))
        {
            isPlayerBInPlatform = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerA"))
        {
            isPlayerAInPlatform = false;
        }
        else if (collision.CompareTag("PlayerA"))
        {
            isPlayerBInPlatform = false;
        }
    }
}
