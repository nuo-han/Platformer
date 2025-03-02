using System.Net.NetworkInformation;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WeaponSystem : MonoBehaviour
{
    public GameObject[] weapons; // ��������Ԥ�Ƽ�������
    private int currentWeaponIndex = 0;

    void Start()
    {
        // ��ʼ��ʱ�������������������һ������
        SetAllWeaponsInactive();
        SwitchWeapon(currentWeaponIndex);
    }

    void Update()
    {
        // ʹ�����ּ��л�
        for (int i = 0; i < weapons.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SwitchWeapon(i);
            }
        }

    }

    void SetAllWeaponsInactive()
    {
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }
    }

    void SwitchWeapon(int newIndex)
    {
        if (newIndex >= 0 && newIndex < weapons.Length)
        {
           
            weapons[currentWeaponIndex].SetActive(false);
            currentWeaponIndex = newIndex;
            weapons[currentWeaponIndex].SetActive(true);
            

        }
    }

}
