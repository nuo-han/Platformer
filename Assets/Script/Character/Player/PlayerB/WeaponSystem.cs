using System.Net.NetworkInformation;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WeaponSystem : MonoBehaviour
{
    public GameObject[] weapons; // 所有武器预制件的数组
    private int currentWeaponIndex = 0;

    void Start()
    {
        // 初始化时禁用所有武器，激活第一个武器
        SetAllWeaponsInactive();
        SwitchWeapon(currentWeaponIndex);
    }

    void Update()
    {
        // 使用数字键切换
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
