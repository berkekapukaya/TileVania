using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinMovement : MonoBehaviour
{
    [SerializeField] AudioClip coinPickupSFX;
    [SerializeField] Vector2 coinPickupSpeed = new Vector2(0f, 20f);
    [SerializeField] int coinValue = 100;
    
    Rigidbody2D rb;
    CircleCollider2D circleCollider;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            circleCollider.enabled = false;
            AudioSource.PlayClipAtPoint(coinPickupSFX, Camera.main.transform.position);
            StartCoroutine(AnimateCoin());
        }
    
    }
    IEnumerator AnimateCoin()
    {
        rb.velocity = coinPickupSpeed;
        
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
