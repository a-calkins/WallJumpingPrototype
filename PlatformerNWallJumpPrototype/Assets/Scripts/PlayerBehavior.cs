using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    private float speed = 5f;
    private float moveSpeedWhenJumping = 2f;
    private Vector3 pos;
    [SerializeField] private LayerMask mask;
    private float jumpPower = 15f;
    private Rigidbody2D rb;
    private BoxCollider2D box;
    private RaycastHit2D hitInfo;
    private RaycastHit2D frontHit;
    private bool isJumping = false;

    private bool wallSliding;
    private bool wallJumping;
    private bool isTouchingFront;
    private bool facingRight;
    //public Transform frontCheck;
    public float wallSlidingSpeed;
    public float xWallForce;
    public float yWallForce;
    public float wallJumpTime;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        CheckFront();
    }

    //this method handles player movement
    void Move()
    {
        pos = transform.position;
        //grounded movement
        if(Input.GetKey("a") && IsGrounded() && !wallSliding && !wallJumping)
        {
            facingRight = false;
            pos.x -= speed * Time.deltaTime;
            //rb.velocity = Vector3.left * speed;
        }
        if (Input.GetKey("d") && IsGrounded() && !wallSliding && !wallJumping)
        {
            facingRight = true;
            pos.x += speed * Time.deltaTime;
           //rb.velocity = Vector3.right * speed;
        }
        //jumping
        if(Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.velocity = Vector3.up * jumpPower;
            isJumping = true;
        }
        if (isJumping && Input.GetKey("a") && !wallSliding && !wallJumping)
        {
            pos.x -= moveSpeedWhenJumping * Time.deltaTime;
        }
        if (isJumping && Input.GetKey("d") && !wallSliding && !wallJumping)
        {
            facingRight = true;
            pos.x += moveSpeedWhenJumping * Time.deltaTime;
        }
        transform.position = pos;
    }
    //check if player is touching the wall
    void CheckFront()
    {
        if(facingRight)
        {
            frontHit = Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0f, Vector3.right, 0.1f, mask);
        } else
        {
            frontHit = Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0f, Vector3.left, 0.1f, mask);
        }
        
        //start wallsliding
        isTouchingFront = (frontHit.collider != null);
        if(isTouchingFront && !IsGrounded())
        {
            wallSliding = true;
        } else
        {
            wallSliding = false;
        }

        if(wallSliding)
        {
            WallSlide();
        }
        if(Input.GetKeyDown(KeyCode.Space) && wallSliding)
        {
            WallJump();
        }
    }
    //this method handles the physics for wallsliding
    void WallSlide()
    {
        rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
    }
    //this method handles physics for walljumping
    void WallJump()
    {
        wallJumping = true;
        if (Input.GetKey("a"))
        {
            facingRight = false;
            rb.velocity = (Vector3.up + Vector3.left) * jumpPower / 2;
        }
        else if (Input.GetKey("d"))
        {
            facingRight = true;
            rb.velocity = (Vector3.up + Vector3.right) * jumpPower / 2;
        }
        else
        {
            rb.velocity = Vector3.up * jumpPower / 2;
        }
        //disables wallJumping after a set period of time
        Invoke("SetWallJumpingFalse", wallJumpTime);
    }
    //checks if the player is grounded
    private bool IsGrounded()
    {
        hitInfo = Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0f, Vector3.down, 0.1f, mask);
        return hitInfo.collider != null;
    }
    //sets wall jumping to false
    void SetWallJumpingFalse()
    {
        wallJumping = false;
    }
}
