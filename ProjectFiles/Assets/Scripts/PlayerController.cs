﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D coll;

    public float maxSpeed, jumpForce, moveForce;
    public Transform groundCheck;
    public Transform restratPoint;
    public LayerMask ground;
    public LayerMask deadZone;

    public bool isGround, isJump, isDead;
    public int jumpAbility;
    public PhysicsMaterial2D withFriction;//有摩擦力的材质
    public PhysicsMaterial2D withoutFriction;//没有摩擦力的材质
    public GameplayController gameplayController;
    bool jumpPressed;

    int jumpCount;
    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        restratPoint.parent = null;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump") && jumpCount > 0)
        {
            jumpPressed = true;
        }
        if (isDead)
        {
            transform.position = restratPoint.position;
            isDead = false;
            gameplayController.isWaitingForChangeColor = true;
            gameplayController.j = 0;
        }
    }
    private void FixedUpdate()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, 0.1f, ground);
        isDead = Physics2D.OverlapCircle(groundCheck.position, 0.1f, deadZone);

        GroundMovement();
        Jump();
        ChangePhysicsMaterial2D();
    }
    void GroundMovement()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(rb.velocity.x) < maxSpeed)
        {
            rb.AddForce(new Vector3(horizontalMove, 0, 0) * moveForce);
        }
        else
        {
            rb.velocity = new Vector3(horizontalMove * maxSpeed, rb.velocity.y, 0);
        }

        if (horizontalMove != 0)
        {
            transform.localScale = new Vector3(horizontalMove, 1, 1);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    void Jump()
    {
        if (isGround)
        {
            jumpCount = jumpAbility;
            isJump = false;
        }
        if (jumpPressed && isGround)
        {
            isJump = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount--;
            jumpPressed = false;
        }
        else if (jumpPressed && jumpCount > 0 && isJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount--;
            jumpPressed = false;
        }
    }
    void ChangePhysicsMaterial2D()
    {
        if (isGround)
        {
            rb.sharedMaterial = withFriction;
        }
        else
        {
            rb.sharedMaterial = withoutFriction;
        }
    }
}
