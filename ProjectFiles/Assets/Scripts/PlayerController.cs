using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D coll;

    public float speed, jumpForce;
    public Transform groundCheck;
    public LayerMask ground;

    public  bool isGround,isJump;
    public int jumpAbility;
    bool jumpPressed;
    
    int jumpCount;
    // Start is called before the first frame update
    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Jump")&&jumpCount >0)
        {
            jumpPressed =true;
        }
    }
    private void FixedUpdate() 
    {
        isGround =Physics2D.OverlapCircle(groundCheck.position,0.1f,ground);

        GroundMovement();
        Jump();
    }
    void GroundMovement()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        rb .velocity =new Vector2(horizontalMove*speed,rb .velocity.y);
        if(horizontalMove!=0)
        {
           transform .localScale =new Vector3(horizontalMove,1,1); 
        }
    }

    void Jump()
    {
        if(isGround)
        {
            jumpCount =jumpAbility;
            isJump = false;
        }
        if(jumpPressed && isGround)
        {
            isJump = true;
            rb.velocity =new Vector2(rb.velocity.x,jumpForce);
            jumpCount--;
            jumpPressed =false;
        }   
        else if(jumpPressed&&jumpCount>0 && isJump)
        {
            rb.velocity =new Vector2(rb.velocity.x,jumpForce);
            jumpCount --;
            jumpPressed =false;
        }
    }
}
