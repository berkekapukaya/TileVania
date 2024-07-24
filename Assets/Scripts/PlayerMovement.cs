using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 2f;
    public LayerMask groundLayer;
    public LayerMask climbingLayer;
    
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private CapsuleCollider2D cd;
    private Animator _animator;
    private bool playerHasHorizontalSpeed;
    private bool isGrounded;
    private bool isClimbing;
    float gravityScaleAtStart;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CapsuleCollider2D>();
        _animator = GetComponent<Animator>();
        gravityScaleAtStart = rb.gravityScale;
    }
    
    void Update()
    {
        Run();
        ClimbLadder();
        FlipSprite();
    }
    
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    
    void OnJump(InputValue value)
    {
        if(value.isPressed && cd.IsTouchingLayers(groundLayer))
        {
            // Jump();
            rb.velocity += new Vector2(rb.velocity.x, jumpSpeed);
        }
    }
    
    
    void ClimbLadder()
    {
        if(cd.IsTouchingLayers(climbingLayer))
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
    
}
