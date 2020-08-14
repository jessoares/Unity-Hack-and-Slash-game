using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;


public class PlayerController : MonoBehaviour
{
    [Space]
    [Header("Mouse")]
    public Transform sword;
    public Vector3 lookDir;
    public Camera cam;
    public Vector3 mousePos;
    public bool canMove;
    [Space]
    [Header("Animating")]
    public Animator animator;
    [Space]
    [Header("Movement")]
    public Vector3 movement;
    public bool facingRight = true;
    public float moveSpeed;
    public float baseMoveSpeed;
    public Rigidbody2D playerRb;
    float moveBaseSpeed;
    int playerLayer;
    int enemyLayer;
    [Space]
    [Header("Dash")]
    float slideSpeed;
    public State state;
    Vector3 slideDir;
    public float dashRate;
    float nextDashTime;
    int layerMask;
    public float angle;
    public enum State  
    {
        Normal,
        Dodge,
    }
    public void Awake()
    {
        layerMask = LayerMask.GetMask("Obstacle"); //layermask = obstacle layer
        playerLayer = 10; 
        enemyLayer = 8; // initializing player and enemy layers
        state = State.Normal;
        GetComponent<Collider2D>().enabled = true; 
        Physics2D.IgnoreLayerCollision(enemyLayer, playerLayer); //Player's rigidBody wont collide with Enemy's
        moveSpeed = baseMoveSpeed;
    }
    void Update()
    {
        switch (state)
        {
            case State.Normal:   // case state = normal does functions below
                ProcessInputs();
                Move();
                HandleDodgeRoll();
                break;
            case State.Dodge:
                HandleDodgeRollSliding();
                break;

        }
    }
    public void FixedUpdate()
    {
        mousePos = UtilsClass.GetMouseWorldPosition();
        lookDir = (mousePos - sword.position).normalized;
        angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f; 
        sword.eulerAngles = new Vector3(0, 0, angle);
    }
    void ProcessInputs()
    {
        animator.SetFloat("Speed", movement.magnitude);
        if (GetComponent<PlayerAttack>().state == PlayerAttack.State.Normal)
        {
            movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            moveBaseSpeed = Mathf.Clamp(movement.magnitude, 0.0f, 1.0f);
            movement.Normalize();
            if (movement.x > 0 && !facingRight)
            {
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (movement.x < 0 && facingRight)
            {
                Flip();
            }
        }
    }
    public void Move()
    {
        playerRb.velocity = movement * moveBaseSpeed * moveSpeed;    
    }
    public void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    public void HandleDodgeRoll()
    {
        if (Time.time >= nextDashTime)
        {
            if (Input.GetButtonDown("Jump"))
            {
                if(mousePos.x < transform.position.x && facingRight)
                {
                    Flip();
                }
                else if (mousePos.x > transform.position.x && !facingRight)
                {
                    Flip();

                }
                if (playerRb.velocity != Vector2.zero)
                {
                    playerRb.velocity = Vector2.zero; //nullifies velocity bc dash uses another logic for movement
                }
                GetComponent<Collider2D>().enabled = false;
                state = State.Dodge; //changes state for dash 
                slideDir = (UtilsClass.GetMouseWorldPosition() - transform.position).normalized; // position where to dash to 
                slideSpeed = 35f; //dash force
                GetComponent<PlayerAttack>().animator.SetBool("Charge", false);
                GetComponent<PlayerAttack>().bortzTimer = 0f;
                GetComponent<PlayerAttack>().bortzSpecialReady = false;
                moveSpeed = baseMoveSpeed;
                GetComponent<PlayerAttack>().nextBortzSpecialTime = Time.time + 1f / GetComponent<PlayerAttack>().bortzSpecialRate;
            }
        }
    }
    public void HandleDodgeRollSliding()
    {  
        GetComponent<PlayerHealth>().isBlocking = false;
        GetComponent<PlayerAttack>().animator.SetBool("IsBlocking", false);
        GetComponent<PlayerAttack>().blockTime = 0;
        animator.SetBool("IsDashing", true);
        TrySpecial(slideDir, slideSpeed * Time.deltaTime); //dash logic, it checks how far the player can dash
        slideSpeed -= slideSpeed * 10f * Time.deltaTime; // timer for the dash,while player is able to, dash occurs
        if (slideSpeed < 5f)
        {
            //dash ends
            nextDashTime = Time.time + 1f / dashRate;
            GetComponent<Collider2D>().enabled = true;
            state = State.Normal;
            animator.SetBool("IsDashing", false);
        }
    }
    public bool CanMove(Vector3 dir, float distance)
    {
        return Physics2D.Raycast(transform.position, dir, distance, layerMask).collider == null; //checks if there are no obstacles in a single line
    }
    public void TrySpecial(Vector3 baseMoveDir, float distance)
    {
        Vector3 moveDir = baseMoveDir;
        canMove = CanMove(moveDir, distance);
        if (playerRb.velocity != Vector2.zero)
        {
            playerRb.velocity = Vector2.zero;
        }
        if (!canMove)
        {
            while (!canMove)
            {
                distance = distance - 0.1f;
                canMove = CanMove(moveDir, distance);
            }
        }
        transform.position += moveDir * distance;
    }
    public float returnDistance(Vector3 baseMoveDir, float distance)
    {
        float returnD = distance;
        Vector3 moveDir = baseMoveDir;
        canMove = CanMove(moveDir, distance);
        if (playerRb.velocity != Vector2.zero)
        {
            playerRb.velocity = Vector2.zero;
        }
        if (!canMove)
        {
            while (!canMove)
            {
                distance = distance - 0.1f;
                canMove = CanMove(moveDir, distance);
            }
        }
        return distance;
    }
}











