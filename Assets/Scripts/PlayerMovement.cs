using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    
    public float wallJumpBufferTime = 0.5f;
    public float jumpBufferTime = 0.5f;
    public float coyoteBufferTime = 0.5f;
    
    private bool allowCoyote = false;
    
    private Vector3 vel;
    
    // public float jumpForce = 10;
    
    private float lastGrounded;
    private bool isGrounded = false;
    private bool collidedDown = false;
    private bool collidedRight = false;
    private bool collidedLeft = false;
    private bool collidedUp = false;
    
    [SerializeField] private BoxCollider2D playerCollider;
    
    // Start is called before the first frame update
    void Start() {
        vel = new Vector2(0,0);
    }

    // Update is called once per frame
    void Update() {
        checkCollisions();
        move();
    }
    
    [SerializeField] private LayerMask ground;
    private float collidedCheckRange = 0.1f;
    
    void checkCollisions(){
        collidedDown = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, Vector2.down, collidedCheckRange, ground);
        collidedUp = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, Vector2.up, collidedCheckRange, ground);
        collidedLeft = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, Vector2.left, collidedCheckRange, ground);
        collidedRight = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, Vector2.right, collidedCheckRange, ground);
        
        // check if just left ground
        if(isGrounded && !collidedDown) lastGrounded = Time.time;
        if(collidedDown && !isGrounded) allowCoyote = true;
        
        isGrounded = collidedDown;
        
    }
    
    void move(){
        calcHorizontal();
        calcGravity();
        calcJump();
        // vel.y = -1;
        calcLatestCollision();
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
        
        // stop if colliding
        if((collidedLeft && vel.x < 0)||(collidedRight && vel.y > 0)) vel.x = 0;
    }
    
    public float maxFallSpeed = 10f;
    public float gravity = -0.01f;
    
    void calcGravity(){
        if(!collidedDown){
            vel.y += gravity*Time.deltaTime;
            vel.y = Mathf.Min(vel.y, -maxFallSpeed);
        }
        if(collidedDown && vel.y < 0) vel.y = 0;
    }
    
    
    void calcJump(){
        if(!Input.GetKey("space")) return;
        // if(!(isGrounded || (Time.time-lastGrounded < coyoteBufferTime)&&allowCoyote)) return;
        allowCoyote = false;
        vel.y = 10;
        
        if(collidedUp && vel.y > 0) vel.y = 0;
    }
    
    private float collisionBuffer = 0.01f;
    private int maxIterationSteps = 20;
    
    void calcLatestCollision(){
        if(vel.magnitude == 0) return;
        
        var moveStep = vel*Time.deltaTime;
        var pos = playerCollider.bounds.center;
        var endPos = pos + moveStep;
        var hasCollision = Physics2D.OverlapBox(endPos, playerCollider.bounds.size, 0, ground);
        if(!hasCollision){
            transform.position += moveStep;
            return;
        }
        
        
        // var moveStep = vel*Time.deltaTime;
        // var hasCollision = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, moveStep, moveStep.magnitude+collisionBuffer, ground);
        // if(!hasCollision){
        //     transform.position += moveStep;
        //     return;
        // }
        // Vector3 startPos = new Vector3(0,0,0);
        // Vector3 endPos = moveStep;
        // var lastNonCollidedPos = startPos;
        // for(int i = 0;i < maxIterationSteps;i++){
        //     var checkPos = Vector3.Lerp(startPos, endPos,(float)i/maxIterationSteps);
        //     if(Physics2D.OverlapBox(checkPos+playerCollider.bounds.center, playerCollider.bounds.size, 0, ground)) break;
        //     lastNonCollidedPos = checkPos;
        // }
        // transform.position += lastNonCollidedPos;
        
        
        // Debug.Log("Found Collision!");
        // Debug.Log(moveStep);
        // // if there is collision find earliest non colliding position
        // var lastNonCollidingDist = 0f;
        // var currentStep = moveStep.magnitude/2;
        // var checkDist = 0f;
        // for(var i = 0;i < maxIterationSteps;i++){
        //     if(Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, moveStep, checkDist+collisionBuffer, ground)){
        //         Debug.Log("has Collision!");
        //         checkDist -= currentStep;
        //     }else{
        //         Debug.Log("no Collision!");
        //         lastNonCollidingDist = checkDist;
        //         checkDist += currentStep;
        //     }
        //     currentStep /= 2;
        // }
        // moveStep.Normalize();
        // moveStep = moveStep*lastNonCollidingDist;
        // transform.position += moveStep;
    }
    
    void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        drawRect(GetComponent<BoxCollider2D>().bounds.center+Vector3.down*collidedCheckRange, GetComponent<BoxCollider2D>().bounds.size);
        drawRect(GetComponent<BoxCollider2D>().bounds.center+Vector3.up*collidedCheckRange, GetComponent<BoxCollider2D>().bounds.size);
        drawRect(GetComponent<BoxCollider2D>().bounds.center+Vector3.left*collidedCheckRange, GetComponent<BoxCollider2D>().bounds.size);
        drawRect(GetComponent<BoxCollider2D>().bounds.center+Vector3.right*collidedCheckRange, GetComponent<BoxCollider2D>().bounds.size);
        Gizmos.color = Color.red;
        drawRect(GetComponent<BoxCollider2D>().bounds.center, GetComponent<BoxCollider2D>().bounds.size);
    }
    
    void drawRect(Vector3 center, Vector3 size){
        Vector2 c1 = center - size/2;
        Vector2 c2 = c1 + new Vector2(size.x, 0);
        Vector2 c3 = c1 + new Vector2(0, size.y);
        Vector2 c4 = c1 + new Vector2(size.x, size.y);
        Gizmos.DrawLine(c1, c2);
        Gizmos.DrawLine(c1, c3);
        Gizmos.DrawLine(c2, c4);
        Gizmos.DrawLine(c3, c4);
    }
    
    
}
