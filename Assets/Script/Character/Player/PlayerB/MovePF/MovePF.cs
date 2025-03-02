using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePF : MonoBehaviour
{
    public float moveSpeed;
    public Transform[] wayPoints;
    int pointIndex;
    private bool canMove;
    private bool isForward;

    private void Start()
    {
        pointIndex = 0;
        canMove = false;
        isForward = true;
    }

    private void Update()
    { if (canMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, wayPoints[pointIndex].position, moveSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, wayPoints[pointIndex].position) < 0.1f && isForward)
            {
                pointIndex++;
            }
            if(pointIndex >= wayPoints.Length)
            {
                pointIndex = wayPoints.Length - 2;
                isForward = false;
            }
            Return();
        }
    }

    public void Return()
    {
        if(isForward == false)
        {
            if(Vector2.Distance(transform.position, wayPoints[pointIndex].position) < 0.1f)
            pointIndex--;
            if(pointIndex <= 0)
            {
                isForward = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerA") || collision.CompareTag("PlayerB"))
        {
            canMove = true;
            collision.transform.SetParent(this.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerA") || collision.CompareTag("PlayerB"))
        {
            collision.transform.SetParent(null);
        }
    }
}
