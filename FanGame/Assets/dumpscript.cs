using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class dumpScript: MonoBehaviour
{
    [Space]
    [Header("General")]
    public int maxHealth;
    int currentHealth;
    [Space]
    [Header("Animation")]
    public Animator animator;
    [Space]
    [Header("Movement AI")]
    Transform player;
    public bool isShooting;
    [Space]
    [Header("Shooting AI")]
    private bool facingRight = true;
    private float timeBtwShots;
    public float startTimeBtwShots;
    [Space]
    [Header("Shooting")]
    public Transform tip;
    private Vector3 playerDir;
    Coroutine attack;
    public Vector2 shootTarget;
    public Transform bow;
    public float angle;
    public float lanceRange;
    public float attackDistance;
    public LayerMask playerLayer;
    public ParticleSystem thrust;
    [Space]
    [Header("Spawning")]
    GameObject spawn;

    public float speed = 200f;

    public float nextWaypointDistance;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndofPath = false;

    public bool isSpawning;

    Seeker seeker;
    Rigidbody2D rb;
   

    private void Awake()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform; //Locates the position of the player in game
        spawn = GameObject.FindGameObjectWithTag("Manager"); // locates the game manager for spawning logic
        timeBtwShots = 0;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("UpdatePath", 0f, .5f);
        isShooting = false;
        isSpawning = true;
        GetComponentInChildren<Renderer>().sortingOrder = 5;
        StartCoroutine(SpawnCoroutine());
        GetComponent<BoxCollider2D>().enabled = false;
    }

    void UpdatePath()
    {
        if(seeker.IsDone())
        seeker.StartPath(rb.position, player.position, OnPathComplete);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Aiming();
        if (isSpawning == true)                   // if the lancer is spawning, it will go down only until the coroutine started at Awake ends
        {
            transform.Translate(Vector3.down * Time.deltaTime * speed);
        }
        else
        {
            timeBtwShots -= Time.deltaTime;        // attack cooldown, goes down everytime
            if (path == null)                                 // path logic
                return;
            if (currentWaypoint >= path.vectorPath.Count)         // if player is near enemy will start attack sequence
            {
                transform.position = this.transform.position;
                animator.SetBool("IsRunning", false);
                Shoot();
                reachedEndofPath = true;
                return;
            }
            else if (isShooting == false)             // if not, and its not shooting, it may move
            {
                if (player.position.x < transform.position.x && facingRight)
                {
                    Flip();
                }
                else
                {
                    if (player.position.x > transform.position.x && !facingRight)
                    {
                        Flip();
                    }
                }
                if (Vector2.Distance(transform.position, (Vector2)path.vectorPath[currentWaypoint]) > nextWaypointDistance)
                {
                    animator.SetBool("IsRunning", true);
                    transform.position = Vector2.MoveTowards(transform.position, (Vector2)path.vectorPath[currentWaypoint], speed * Time.deltaTime);
                }
                float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
                if (distance < nextWaypointDistance)
                {
                    currentWaypoint++;
                }
                reachedEndofPath = false;
            }
        }
    }

    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void Aiming()
    {
        //rotates the firing point for aiming towards the player
        shootTarget = (player.transform.position - transform.position).normalized;
        angle = Mathf.Atan2(shootTarget.y, shootTarget.x) * Mathf.Rad2Deg - 90f;
        bow.eulerAngles = new Vector3(0, 0, angle);
    }
   
    public void Shoot()
    {
        if (timeBtwShots <= 0)
        {
            isShooting = true;
            animator.SetTrigger("Attack");
            attack = StartCoroutine(AttackCoroutine()); // starts attack coroutine
            timeBtwShots = startTimeBtwShots;
        }
        
    }
    public void TakeDamage(int damage)
    {
        currentHealth = currentHealth - damage;
        animator.SetTrigger("Hurt");
        Debug.Log("HIT ENEMY");
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        this.enabled = false;
        spawn.GetComponent<WaveSpawner>().GetSpawnNumber();
        GetComponent<Collider2D>().enabled = false;
        if (isShooting == true)
        {
            StopCoroutine(attack);
        }
        StartCoroutine(DeathCoroutine());
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
        yield return new WaitForSeconds(0.5f);
        ParticleSystem slashVelocity = Instantiate(thrust, tip.position, bow.rotation);
        slashVelocity.transform.parent = this.transform;
        if (facingRight == false)
        {
            Vector3 theScale = slashVelocity.transform.localScale;
            theScale.x *= -1;
            slashVelocity.transform.localScale = theScale;
        }
        StartCoroutine(ParticleCoroutine(slashVelocity));
        RaycastHit2D[] hitPlayer = Physics2D.CircleCastAll(tip.position, 0.5f, shootTarget, attackDistance,playerLayer);
        foreach (RaycastHit2D playerHit in hitPlayer)
        {
            playerHit.collider.gameObject.GetComponent<PlayerHealth>().TakeDamage(1);
           
        }
        StartCoroutine(WaitCoroutine());
       
        

    }

    public IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);

    }

    private void OnDrawGizmosSelected()//attack collider visualization in-engine
    {
        if (lanceRange == null)
            return;
        Gizmos.DrawWireSphere(tip.position, lanceRange);
    }
    public IEnumerator ParticleCoroutine(ParticleSystem particle)
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(particle.gameObject);
    }
    public IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(1f);
        isShooting = false;
    }

    public IEnumerator SpawnCoroutine()
    {
        yield return new WaitForSeconds(10f);
        animator.SetTrigger("Spawn");
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponentInChildren<Renderer>().sortingOrder = 0;
        isSpawning = false;
    }


}
