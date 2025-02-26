using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ArrowTrap : MonoBehaviour
{
    public GameObject arrowPrefab; // 箭矢的预制体
    public Transform firePoint; // 箭矢发射点
    public float interval = 2.0f; // 箭矢发射速率
    public bool isRight = true; // 箭矢发射方向
    private float timer = 0.0f; // 计时器

    private ObjectPool<GameObject> arrowPool;//箭矢对象池

    private void Awake()
    {
        arrowPool = new ObjectPool<GameObject>(createFunc,actionOnGet,actionOnRelease,actionOnDestroy,true,10,1000);
    }
    private GameObject createFunc()
    {
        GameObject arrow = Instantiate(arrowPrefab);
        arrow.GetComponent<Arrow>().arrowPool = arrowPool;
        arrow.SetActive(false);
        return arrow;
    }
    private void actionOnGet(GameObject arrow)
    {
        arrow.SetActive(true);
        arrow.transform.position = firePoint.position;
        if (isRight)
        {
            arrow.gameObject.GetComponent<Arrow>().direction = new Vector2(1, 0);
        }
        else
        {
            arrow.gameObject.GetComponent<Arrow>().direction = new Vector2(-1, 0);
        }
    }
    private void actionOnRelease(GameObject arrow)
    {
        arrow.SetActive(false);
    }
    private void actionOnDestroy(GameObject arrow)
    {
        Destroy(arrow);
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            ShootArrow();
            timer = 0.0f;
        }
    }
    private void ShootArrow()
    {
        GameObject arrow = arrowPool.Get();
    }
}
