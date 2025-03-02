using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public Transform player;        
    public Transform[] destinations; // 传送目标位置数组

    public void TransfToD(int index)
    {
        if (index >= 0 && index < destinations.Length)
        {
            if (GetComponent<GouHuo>().isActive == true)
            {
                player.position = destinations[index].position;
                GetComponent<GouHuo>().portalPanel.SetActive(false);
                GetComponent<GouHuo>().isPanel = false;
            }
        }
    }
}
