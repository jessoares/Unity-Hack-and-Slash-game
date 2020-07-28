using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    [Space]
    [Header("General")]
    public float speed;
    [Space]
    [Header("Animation")]
    public Animator animator;
    [Space]
    [Header("Movement AI")]
    public float stoppingDistance;
    public float retreatDistance;
    public float retreatTime = 0f;
    Transform player;
    float advanceTime = 0f;
    Vector3 randomSpot;
    public Transform[] randomPosition;
    private int randomPoint;
    public float roamTime = 0f;
    private bool isRoaming = false;
    public bool isShooting;
    [Space]
    [Header("Shooting AI")]
    private bool facingRight = true;
    private float timeBtwShots;
    public float startTimeBtwShots;
    [Space]
    [Header("Shooting")]
    public Transform tip;
    public GameObject projectile;
    private Vector3 playerDir;
    Coroutine attack;
    public Rigidbody2D bow;
    public Vector2 target;
    public float angle;
    public enum State
    {
        Spawning,
        Normal,
        Shooting,
        Roaming,
        Wait,
    }
    public State state;
    [Space]
    [Header("Spawning")]
    GameObject spawn;

    private void Awake()
    {
        state = State.Spawning;
        player = GameObject.FindGameObjectWithTag("Player").transform; //Locates the position of the player in game
        spawn = GameObject.FindGameObjectWithTag("Manager"); // locates the game manager for spawning logic
        timeBtwShots = startTimeBtwShots;
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponentInChildren<SortingGroup>().sortingOrder = 5;
        StartCoroutine(SpawnCoroutine());
    }

    private void Update()
    {
        if (GetComponent<EnemyHealth>().isDead == true)
        {
            if (isShooting == true)
            {
                StopCoroutine(attack);
            }
            return;
        }
        else
        {
            switch (state)
            {
                case State.Spawning:
                    Spawning();
                    break;
                case State.Normal:
                    Moving();
                    Aiming();
                    FlipState();
                    break;
                case State.Shooting:
                    Shoot();
                    break;
                case State.Roaming:
                    FlipState();
                    Aiming();
                    Roam();
                    FlipState();
                    break;
                case State.Wait:
                    Aiming();
                    FlipState();
                    break;
            }
        }
    }
    
    void Spawning()
    {
        transform.Translate(Vector3.down * Time.deltaTime * speed);
    }
    private void Aiming()
    {
        //rotates the firing point for aiming towards the player
        target = (player.transform.position - transform.position).normalized;
         angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg - 90f;
        bow.rotation = angle;
    }
    private void Moving()
    {
        //if player dies, enemy stops moving
        if (player.GetComponent<PlayerHealth>().isDead == true) { 
        
            state = State.Wait;
       }
        //if the player's position is bigger than a minimum distance, enemy is supposed to go after them
        if (Vector2.Distance(transform.position, player.position) > stoppingDistance)
        {
            retreatTime = 0;
            advanceTime += Time.deltaTime; //timer for "delay" in enemy's following
            //if the timer is more than 0.5 seconds enemy will follow
            if (advanceTime > 0.5f)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
                animator.SetBool("IsRunning", true);
            }
            else
            {
                animator.SetBool("IsRunning", false);
            }
        }
        //if the player is in firing range, goes to the firing state
        else if (Vector2.Distance(transform.position, player.position) < stoppingDistance && Vector2.Distance(transform.position, player.position) > retreatDistance && isRoaming == false)
        {
            animator.SetBool("IsRunning", false);
            state = State.Shooting;
        }
        //if the player is closer than the minimum distance the enemy will move away from them
        else if (Vector2.Distance(transform.position, player.position) < retreatDistance && isRoaming == false)
        {
            advanceTime = 0;
            retreatTime += Time.deltaTime;//timer for how long the enemy is able to retreat
            if (retreatTime > 1 && retreatTime < 1.5f)
            {
                animator.SetBool("IsRunning", true);
                transform.position = Vector2.MoveTowards(transform.position, player.position, -speed * Time.deltaTime);
            }
            if (retreatTime > 1.5f) //if retreattime runs out the enemy will shoot
            {
                animator.SetBool("IsRunning", false);
                state = State.Shooting;
            }
            if (retreatTime < 1f)
            {
                animator.SetBool("IsRunning", false);
            }
        }
    }
    public void Shoot()
    {
        isRoaming = false;
        timeBtwShots -= Time.deltaTime;
        animator.SetBool("IsRunning", false);
        retreatTime = 0f;
        advanceTime = 0f;
        if (timeBtwShots <= 0)
        {
            animator.SetTrigger("Attack"); 
            attack = StartCoroutine(AttackCoroutine()); // starts attack coroutine
            timeBtwShots = startTimeBtwShots;
            state = State.Wait;
        }
    }
  
    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    public IEnumerator AttackCoroutine()
    {
        isShooting = true;
        yield return new WaitForSeconds(0.7f);
        //"creates" arrow on the scene
        GameObject arrow = Instantiate(projectile, tip.position, bow.transform.rotation);
        //logic for obtaining the correct rotation of the arrow object, its supposed to point towards the player
        Rigidbody2D arrowrb = arrow.GetComponent<Rigidbody2D>();
        arrowrb.rotation = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        isShooting = false;
        StartCoroutine(RoamCoroutine());
    }
    public IEnumerator RoamCoroutine()
    {
        //picks between 10 random directions to make the enemy briefly roam to
        yield return new WaitForSeconds(1.5f);
        float randomSpot = Random.Range(1f, 10f);
        state = State.Roaming;
    }
    public void Roam()
    {
        //brief roaming logic
        if (roamTime < 1f)
        {
            animator.SetBool("IsRunning", true);
            transform.position = Vector2.MoveTowards(transform.position, randomPosition[randomPoint].position, speed * Time.deltaTime);
            roamTime += Time.deltaTime;
        }
        if (roamTime > 1f)
        {
            animator.SetBool("IsRunning", false);
            roamTime = 0;
            randomPoint = Random.Range(0, randomPosition.Length);
            state = State.Normal;
        }       
    }
    public IEnumerator SpawnCoroutine()
    {
        yield return new WaitForSeconds(4f);
        animator.SetTrigger("Spawn");
        state = State.Normal;
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponentInChildren<SortingGroup>().sortingOrder = 5;
    }

    void FlipState()
    {
        if (player.position.x < transform.position.x && facingRight)
        {
            Flip();
        }
        else
        if (player.position.x > transform.position.x && !facingRight)
        {
            Flip();
        }
    }

}

   

