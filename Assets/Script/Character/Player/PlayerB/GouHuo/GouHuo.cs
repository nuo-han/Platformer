using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GouHuo : MonoBehaviour
{
    public GameObject portalPanel; // 关联UI面板
    public bool isActive;//是否激活
    public bool isPanel;//是否处于面板中
    public bool isRange;

    private void Start()
    {
        isActive = false;
        isPanel = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            portalPanel.SetActive(true);
            isActive = true;
            isPanel = true;
            //player.rb.velocity.zero完善禁用玩家移动
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            portalPanel.SetActive(false);
            isPanel = false;
        }
    }
}
