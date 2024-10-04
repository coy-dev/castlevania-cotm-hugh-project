using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TreeEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 0.7f;
    public float runSpeed = 1.2f;
    public float jumpForce = 2f;
    public float jumpTime = 0.35f;
    private float jumpTimeCounter;

    private float moveSpeed;
    private bool isJumping = false;
    private bool isGrounded = false;
    private UnityEngine.Vector2 movement;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask whatIsGround;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isRunning = false;
    private float doubleTapTime;
    private KeyCode lastKeyCode;

    private bool isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        moveSpeed = walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleRun();
        HandleJump();
        HandleAttack();
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        Debug.Log("Move Input: " + moveInput);

        FlipCharacter(moveInput);

        if (isAttacking && isGrounded)
        {
            rb.velocity = new UnityEngine.Vector2(0, rb.velocity.y);
        }
        else
        {
            rb.velocity = new UnityEngine.Vector2(moveInput * moveSpeed, rb.velocity.y);
        }

        if ((moveInput != 0 && isGrounded) || moveSpeed == runSpeed)
        {
            animator.SetBool("isWalking", !isRunning);
            animator.SetBool("isRunning", isRunning);
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }
    }

    private void HandleJump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        if (Input.GetButtonDown("Jump") && isGrounded && !isAttacking)
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.velocity = new UnityEngine.Vector2(rb.velocity.x, jumpForce);
            animator.SetTrigger("Jump");
        }

        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = new UnityEngine.Vector2(rb.velocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

        if (!isGrounded)
        {
            animator.SetBool("isJumping", true);
        }
        else
        {
            animator.SetBool("isJumping", false);
        }
    }

    private void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1") && !isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("SwordSlash");
        }
    }

    public void FinishAttack()
    {
        isAttacking = false;
    }

    private void HandleRun()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (lastKeyCode == KeyCode.A || lastKeyCode == KeyCode.LeftArrow && (Time.time - doubleTapTime) < 0.3f)
            {
                isRunning = true;
                moveSpeed = runSpeed;
            } 
            else
            {
                lastKeyCode = KeyCode.A;
                doubleTapTime = Time.time;
            }
        } else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (lastKeyCode == KeyCode.D || lastKeyCode == KeyCode.RightArrow && (Time.time - doubleTapTime) < 0.3f)
            {
                isRunning = true;
                moveSpeed = runSpeed;
            } 
            else
            {
                lastKeyCode = KeyCode.D;
                doubleTapTime = Time.time;
            }
        }

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            isRunning = false;
            moveSpeed = walkSpeed;
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new UnityEngine.Vector2(rb.velocity.x, rb.velocity.y);
    }

    private void FlipCharacter(float moveInput)
    {
        UnityEngine.Vector3 localScale = transform.localScale;

        // Check if moving left
        if (moveInput < 0 && localScale.x > 0)
        {
            // Flip to face left
            localScale.x = -Mathf.Abs(localScale.x);
        }
        // Check if moving right
        else if (moveInput > 0 && localScale.x < 0)
        {
            // Flip to face right
            localScale.x = Mathf.Abs(localScale.x);
        }

        // Apply the new scale without modifying position
        transform.localScale = localScale;
    }

}
