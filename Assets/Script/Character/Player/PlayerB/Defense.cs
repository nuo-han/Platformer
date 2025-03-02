using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    [Header("���Ʒ���")]
    [SerializeField] private float shieldRange = 1.5f; // ���Ʒ����뾶
    [SerializeField] private LayerMask enemyLayer;    

    private Transform playerTransform;
    private bool isFacingRight = true;

    private Vector2 GetshieldDirection()
    {
        return isFacingRight ? Vector2.right : Vector2.left;
    }

    void Awake()
    {
        playerTransform = GetComponent<Transform>();
    }

    void Update()
    {
        UpdateFacingDirection();
    }

    // ʵʱ���½�ɫ����
    private void UpdateFacingDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal != 0)
        {
            isFacingRight = horizontal > 0;
        }
    }

    // ������⣨�ɹ���ϵͳ���ã�
    public bool CheckDefense(Vector2 attackOrigin)
    {
        // ��������������ҵķ���
        Vector2 toEnemy = (attackOrigin - (Vector2)playerTransform.position).normalized;

        // ������ + ������
        return Vector2.Dot(toEnemy, GetshieldDirection()) > 0.7f &&
               Vector2.Distance(playerTransform.position, attackOrigin) < shieldRange;
    }
}