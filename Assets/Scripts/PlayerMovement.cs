using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 2f;
    [SerializeField] Vector2 deathKick = new Vector2(20f, 20f);
    public LayerMask groundLayer;
    public LayerMask climbingLayer;
    public LayerMask enemyLayer;
    public LayerMask hazardLayer;
    public GameObject _bulletPrefab;
    public Transform _gun;
    
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private CapsuleCollider2D bodyCollider;
    private BoxCollider2D feetCollider;
    private Animator _animator;
    private PlayerInput _playerInput;
    private GameObject _bullet;
    
    
    private bool playerHasHorizontalSpeed;
    private bool isGrounded;
    private bool isClimbing;
    private bool isAlive = true;
    private float gravityScaleAtStart;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        feetCollider = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        gravityScaleAtStart = rb.gravityScale;
    }
    
    void Update()
    {
        if (!isAlive) return;
        Run();
        ClimbLadder();
        FlipSprite();
        Die();
    }
    
    void OnFire(InputValue value)
    {
        if (!isAlive) return;
        
        _bullet = Instantiate(_bulletPrefab, _gun.position, Quaternion.Euler(0, 0 ,Mathf.Sign(transform.localScale.x) * -90));
        _bullet.transform.SetParent(_gun);
        _animator.SetTrigger("Shoot");
    }
    
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    
    void OnJump(InputValue value)
    {
        if(value.isPressed && feetCollider.IsTouchingLayers(groundLayer))
        {
            // Jump();
            rb.velocity += new Vector2(rb.velocity.x, jumpSpeed);
        }
    }
    
    void ClimbLadder()
    {
        if (bodyCollider.IsTouchingLayers(climbingLayer))
        {
            isClimbing = moveInput.y != 0;
            _animator.SetBool("isClimbing", isClimbing);
            rb.gravityScale = 0f;
            Vector2 climbVelocity = new Vector2(rb.velocity.x, moveInput.y * climbSpeed);
            rb.velocity = climbVelocity;
        }
        else
        {
            _animator.SetBool("isClimbing", false);
            rb.gravityScale = gravityScaleAtStart;
        }
    }
    
    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, rb.velocity.y);
        rb.velocity = playerVelocity;
        playerHasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;

        _animator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    void FlipSprite()
    {
        playerHasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1f);
        }
    }

    void Die()
    {
        if (bodyCollider.IsTouchingLayers(enemyLayer) 
            || feetCollider.IsTouchingLayers(enemyLayer) 
            || bodyCollider.IsTouchingLayers(hazardLayer))
        {
            isAlive = false;
            _animator.SetTrigger("Death");
            _playerInput.enabled = false;
            rb.velocity = deathKick;
        }
    }
    
}
