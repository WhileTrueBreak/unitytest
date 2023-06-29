using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    
    public float wallJumpBufferTime = 0.5f;
    public float jumpBufferTime = 0.5f;
    public float coyoteBufferTime = 0.5f;
    
    private Vector3 vel;
    
    // public float jumpForce = 10;
    
    private float lastGrounded;
    private bool isGrounded = false;
    private bool collidedDown = false;
    private bool collidedRight = false;
    private bool collidedLeft = false;
    private bool collidedUp = false;
    
    private BoxCollider2D collider;
    
    // Start is called before the first frame update
    void Start() {
        vel = new Vector3(0,0,0);
        collider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update() {
        checkCollisions();
        move();
    }
    
    [SerializeField] private LayerMask ground;
    
    void checkCollisions(){
        collidedDown = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0f, Vector2.down, .1f, ground);
        collidedUp = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0f, Vector2.up, .1f, ground);
        collidedLeft = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0f, Vector2.left, .1f, ground);
        collidedRight = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0f, Vector2.right, .1f, ground);
        
        // check if just left ground
        if(isGrounded && !collidedDown) lastGrounded = Time.time;
        
        isGrounded = collidedDown;
        
    }
    
    void move(){
        calcHorizontal();
        calcVertical();
        Debug.Log(vel);
        transform.position += vel*Time.deltaTime;
    }
    
    public float maxHorizontalSpeed = 4f;
    public float groundedHorizontalAccelTime = 2f;
    public float groundedHorizontalDecelTime = 3f;
    public float aerialHorizontalAccelTime = 2f;
    public float aerialHorizontalDecelTime = 3f;
    
    void calcHorizontal(){
        // get the current horizontal vel
        var horizontalVel = vel.x;
        // get key input
        var left = Input.GetKey("left");
        var right = Input.GetKey("right");
        // if both down or up do nothing
        var acc = 0f;
        if (!left ^ right) {
            // vel.x = 0;
            acc = Mathf.Min(maxHorizontalSpeed/(isGrounded?groundedHorizontalDecelTime:aerialHorizontalDecelTime), Mathf.Abs(horizontalVel)/Time.deltaTime) * -Mathf.Sign(horizontalVel);
        }
        // calc horizontal acc
        else if(left) acc = -maxHorizontalSpeed/(isGrounded?groundedHorizontalAccelTime:aerialHorizontalAccelTime);
        else if(right) acc = maxHorizontalSpeed/(isGrounded?groundedHorizontalAccelTime:aerialHorizontalAccelTime);
        // added to vel
        vel.x += acc*Time.deltaTime;
        vel.x = Mathf.Clamp(vel.x, -maxHorizontalSpeed, maxHorizontalSpeed);
        
        if(collidedLeft) vel.x = Mathf.Max(vel.x, 0);
        if(collidedRight) vel.x = Mathf.Min(vel.x, 0);
    }
    
    public float maxFallSpeed = 10f;
    public float gravity = -0.01f;
    
    void calcVertical(){
        vel.y += gravity*Time.deltaTime;
        vel.y = Mathf.Min(vel.y, -maxFallSpeed);
        
        if(collidedDown) vel.y = Mathf.Max(vel.y, 0);
        if(collidedUp) vel.y = Mathf.Min(vel.y, 0);
    }
    
    
    
}
