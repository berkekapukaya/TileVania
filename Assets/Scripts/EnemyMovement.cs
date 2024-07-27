using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    
    Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        rb.velocity = new Vector2(moveSpeed, 0f);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        moveSpeed = -moveSpeed;
        transform.localScale = new Vector2(Mathf.Sign(moveSpeed), 1f);
    }
    
}
