<<<<<<< Updated upstream
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBShadowPool : MonoBehaviour
{
    public static PlayerBShadowPool Instance;

    public GameObject shadowPrefeb;

    public int shadowCount = 10;

    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;

        //³õÊ¼»¯¶ÔÏó³Ø
        FillPool();
    }

    public void FillPool()
    {
        for(int i = 0; i < shadowCount; ++i)
        {
            var newShadow = Instantiate(shadowPrefeb);
            newShadow.transform.SetParent(transform);

            //·µ»Ø¶ÔÏó³Ø
            ReturnPool(newShadow);
        }
    }

    public void ReturnPool(GameObject gameObject)
    {
        gameObject.SetActive(false);

        availableObjects.Enqueue(gameObject);
    }

    public GameObject GetFromPool()
    {
        if( availableObjects.Count == 0)
        {
            FillPool();
        }

        var outShadow = availableObjects.Dequeue();

        outShadow.SetActive(true);

        return outShadow;
    }
}
=======
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBShadowPool : MonoBehaviour
{
    public static PlayerBShadowPool Instance;

    public GameObject shadowPrefeb;

    public int shadowCount = 10;

    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;

        //ï¿½ï¿½Ê¼ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
        FillPool();
    }

    public void FillPool()
    {
        for(int i = 0; i < shadowCount; ++i)
        {
            var newShadow = Instantiate(shadowPrefeb);
            newShadow.transform.SetParent(transform);

            //ï¿½ï¿½ï¿½Ø¶ï¿½ï¿½ï¿½ï¿½
            ReturnPool(newShadow);
        }
    }

    public void ReturnPool(GameObject gameObject)
    {
        gameObject.SetActive(false);

        availableObjects.Enqueue(gameObject);
    }

    public GameObject GetFromPool()
    {
        if( availableObjects.Count == 0)
        {
            FillPool();
        }

        var outShadow = availableObjects.Dequeue();

        outShadow.SetActive(true);

        return outShadow;
    }
}
>>>>>>> Stashed changes
