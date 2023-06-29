using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    
    public float maxDownwardSpeed = 5f;
    public float wallJumpBufferTime = 0.5f;
    public float jumpBufferTime = 0.5f;
    public float coyoteBufferTime = 0.5f;
    
    // public float jumpForce = 10;
    
    private float lastGrounded;
    private bool isGrounded;
    private bool collidedRight;
    private bool collidedLeft;
    private bool collidedUp;
    
    private float moveSpeed;
    
    private BoxCollider2D collider;
    
    // Start is called before the first frame update
    void Start() {
        moveSpeed = 1;
        collider = GetComponent<BoxCollider2D>();
        Debug.Log(collider.size);
        Debug.Log(collider.offset);
    }

    // Update is called once per frame
    void Update() {
        
    }
    
}
