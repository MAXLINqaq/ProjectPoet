using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerControls controls;
    private Rigidbody2D rb;
    private Collider2D coll;
    public PhysicsMaterial2D withFriction;//有摩擦力的材质
    public PhysicsMaterial2D withoutFriction;//没有摩擦力的材质
    public GameplayController gameplayController;
    public float groundMoveSpeed, skyMoveSpeed;
    public float jumpForce, jumpHoldForce, moveForce;
    public Transform groundCheck;
    public Transform restratPoint;
    public LayerMask ground;
    public LayerMask deadZone;

    public bool isGround, isJump, isDead;
    public int jumpAbility;
    bool jumpPressed, jumpFrameCount, jumpHold, jumpStart;
    int jumpCount, jumpFrameFall, jumpFrameLeaveGround;
    float jumpTime;
    float horizontalMove, moveSpeed;
    int moveSpeedUpFrame, moveSpeedDownFrame, moveTurnFrame;
    public Vector3 v;


    // Start is called before the first frame update
    private void Awake()
    {
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        restratPoint.parent = null;
        controls.Gameplay.Jump.started += ctx => jumpHold = true;
        controls.Gameplay.Jump.canceled += ctx => jumpHold = false;
        controls.Gameplay.Jump.performed += ctx => jumpPress();//lamda 表达式
        controls.Gameplay.Move.performed += ctx => horizontalMove = ctx.ReadValue<float>();
        controls.Gameplay.Move.canceled += ctx => horizontalMove = 0;
    }


    // Update is called once per frame
    void Update()
    {
        if (jumpHold)
        {
            jumpTime += Time.deltaTime;
        }
        else
        {
            jumpTime = 0;
        }
        if (isDead)
        {
            transform.position = restratPoint.position;
            isDead = false;
            gameplayController.isWaitingForChangeColor = true;
            gameplayController.j = 0;
        }
        v = rb.velocity;

    }
    private void FixedUpdate()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, 0.1f, ground);
        isDead = Physics2D.OverlapCircle(groundCheck.position, 0.1f, deadZone);
        if (isGround)
        {
            moveSpeed = groundMoveSpeed;
        }
        else
        {
            moveSpeed = skyMoveSpeed;
        }
        Jump();
        ChangePhysicsMaterial2D();
        JumpAddMoreFoce();
        GroundMovement();
    }
    void GroundMovement()
    {

        if (horizontalMove > 0.001)
        {
            horizontalMove = 1;
        }
        else if (horizontalMove < -0.001)
        {
            horizontalMove = -1;
        }
        else
        {
            horizontalMove = 0;
        }
        if (horizontalMove != 0)
        {

            if (rb.velocity.x * horizontalMove == 0)//为移动赋予初始速度
            {
                //moveSpeedUpFrame++;
                rb.velocity = new Vector3(horizontalMove * moveSpeed / 6, rb.velocity.y, 0);
            }
            else if (rb.velocity.x * horizontalMove > 0)//当前速度与移动方向相同
            {

                if (moveSpeedUpFrame < 7 && moveSpeedUpFrame != 0)
                {
                    moveSpeedUpFrame++;
                    rb.velocity = new Vector3(horizontalMove * moveSpeed * moveSpeedUpFrame / 6, rb.velocity.y, 0);
                }
                else
                {
                    rb.velocity = new Vector3(horizontalMove * moveSpeed, rb.velocity.y, 0);
                    moveSpeedUpFrame = 0;
                }
            }
            else if (rb.velocity.x * horizontalMove < 0)
            {
                moveTurnFrame++;
                if (moveTurnFrame < 2 && moveTurnFrame != 0)
                {
                    rb.velocity = new Vector3(moveSpeed * moveTurnFrame / 2, rb.velocity.y, 0);
                }
                else
                {
                    rb.velocity = new Vector3(0, rb.velocity.y, 0);
                }
            }
        }
        else
        {
            if (rb.velocity.x != 0) //减速
            {
                moveSpeedDownFrame++;
                if (moveSpeedDownFrame < 4 && moveSpeedDownFrame != 0)
                {
                    rb.velocity = new Vector3(moveSpeed * (3 - moveSpeedDownFrame) / 3, rb.velocity.y, 0);
                }
                else
                {
                    rb.velocity = new Vector3(0, rb.velocity.y, 0);
                    moveSpeedDownFrame = 0;
                }
            }
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
        //落地前3帧摁下跳跃仍然能够起跳
        if (jumpPressed && !isGround)
        {
            jumpFrameFall++;
        }
        if (isGround)
        {
            jumpCount = jumpAbility;
            isJump = false;
            if (jumpFrameFall < 3 && jumpFrameFall > 0)
            {
                JumpAddForce();
                jumpFrameFall = 0;
            }
        }
        //离开平台后一段时间内仍然可以起跳
        if (isGround && jumpFrameLeaveGround < 4)
        {
            jumpFrameLeaveGround++;
        }
        if (!isGround && jumpFrameLeaveGround > 4)
        {
            jumpFrameLeaveGround = 0;
        }
        if (!isGround && jumpFrameLeaveGround < 4)
        {
            JumpAddForce();
            jumpFrameLeaveGround = 0;
        }


        if (jumpPressed && isGround)
        {
            isJump = true;
            JumpAddForce();
        }
        else if (jumpPressed && jumpCount > 0 && isJump)
        {
            JumpAddVelocity();
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

    private void jumpPress()
    {
        if (jumpCount > 0)
        {
            jumpPressed = true;
        }
    }
    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }
    private void JumpAddForce()
    {
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        jumpCount--;
        jumpPressed = false;
        jumpStart = true;
    }
    private void JumpAddMoreFoce()
    {
        if (jumpTime > 0.15 && jumpStart && rb.velocity.y > 0)
        {
            rb.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);
            jumpHold = false;
            jumpStart = false;
        }
    }
    private void JumpAddVelocity()
    {
        rb.velocity = new Vector3(rb.velocity.x, 12* moveForce * Time.deltaTime, 0);
        jumpCount--;
        jumpPressed = false;
        jumpStart = true;
    }
}
