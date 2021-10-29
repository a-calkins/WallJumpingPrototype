using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* wall jumping and wall sliding */
public class PlayerBehavior : MonoBehaviour
{
    private float speed = 3f;
    private float moveSpeedWhenJumping = 3f;
    private Vector3 pos;
    private int maxHealth = 3, currentHealth;
    
    private float jumpPower = 15f;
    private float invincibility, invincibilityTime = 1f;
    private Rigidbody2D rb;
    private BoxCollider2D box;
    private RaycastHit2D hitInfo;
    private RaycastHit2D frontHit;
    private bool isJumping = false;

    private bool wallSliding;
    private bool wallJumping;
    private bool isTouchingFront;
    private bool facingRight;
    private bool invincible = false;
    private bool controllable = true;

    SpriteRenderer sr;
    
    [SerializeField] private LayerMask mask;
    [SerializeField] private float wallSlidingSpeed;
    [SerializeField] private float xWallForce;
    [SerializeField] private float yWallForce;
    [SerializeField] private float wallJumpTime;
    [SerializeField] private Health health;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        health.SetMaxHealth(maxHealth);
        invincibility = invincibilityTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(controllable) {
            Move();
            CheckFront();
        }
        if(invincible && invincibility > 0) {
            sr.color = Color.yellow;
            invincibility -= Time.deltaTime;
        }
        else {
            invincible = false;
            controllable = true;
            invincibility = invincibilityTime;
            sr.color = Color.black;
        }
        
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
            facingRight = false;
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
        } 
        else
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
        float newSlideSpeed = wallSlidingSpeed * 5f;
        if(Input.GetKey("s")) {
            newSlideSpeed *= 10f;
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -newSlideSpeed, float.MaxValue));
        }
        else if(Input.GetKey("a") || Input.GetKey("d")) {
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, (-wallSlidingSpeed * 0.001f), float.MaxValue)); 
        }
        else {
           rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue)); 
        }
            
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

    void OnTriggerEnter2D(Collider2D collider) {
        if(collider.gameObject.tag == "enemy" && !invincible) {
            controllable = false;
            invincible = true;
            if(facingRight) {
                rb.velocity = (Vector3.up + Vector3.left) * jumpPower / 3;
            }
            else {
                rb.velocity = (Vector3.up + Vector3.right) * jumpPower / 3;
            }
            if(currentHealth > 0)
                currentHealth -= 1; 
            health.SetHealth(currentHealth);
            Debug.Log("current health: " + currentHealth);
        }
    }
}
