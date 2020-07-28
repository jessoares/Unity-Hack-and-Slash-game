using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Rendering;

public class LunarianLancerAI : MonoBehaviour
{
    [Space]
    [Header("General")]
    public float speed;
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
    public Transform tipAnimationPoint;
    [Space]
    [Header("Spawning")]
    GameObject spawn;

    public float nextWaypointDistance;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndofPath = false;

    public bool isSpawning;

    Seeker seeker;
    Rigidbody2D rb;



    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; //Locates the position of the player in game
        spawn = GameObject.FindGameObjectWithTag("Manager"); // locates the game manager for spawning logic
        timeBtwShots = 0;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("UpdatePath", 0f, .5f);
        isShooting = false;
        isSpawning = true;
        GetComponentInChildren<SortingGroup>().sortingOrder = 5;
        StartCoroutine(SpawnCoroutine());
        GetComponent<BoxCollider2D>().enabled = false;
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath(rb.position, player.position, OnPathComplete);
    }

    // Update is called once per frame
    void FixedUpdate()
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

            if (isSpawning == true)                   // if the lancer is spawning, it will go down only until the coroutine started at Awake ends
            {
                transform.Translate(Vector3.down * Time.deltaTime * speed);
            }
            else
            {
                if (isShooting == false)
                {
                    Aiming();
                }
                timeBtwShots -= Time.deltaTime;

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
                    Aiming();
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



    }
    

    void OnPathComplete(Path p)
    {
        if (!p.error)
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
            if (GetComponent<EnemyHealth>().isDead == true)
            {
                StopCoroutine(attack);
                return;
            }
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
        yield return new WaitForSeconds(0.5f);
        ParticleSystem slashVelocity = Instantiate(thrust, tipAnimationPoint.position, bow.rotation);
        slashVelocity.transform.parent = this.transform;
        if (facingRight == false)
        {
            Vector3 theScale = slashVelocity.transform.localScale;
            theScale.x *= -1;
            slashVelocity.transform.localScale = theScale;
        }
        StartCoroutine(ParticleCoroutine(slashVelocity));
        RaycastHit2D[] hitPlayer = Physics2D.CircleCastAll(tip.position, 0.5f, shootTarget, attackDistance, playerLayer);
        foreach (RaycastHit2D playerHit in hitPlayer)
        {
            playerHit.collider.gameObject.GetComponent<PlayerHealth>().OnRaycastEnter2D(playerHit);
            if(playerHit.collider.gameObject.GetComponent<PlayerHealth>().isBlocking == true)
            {
                Destroy(slashVelocity.gameObject);
            }

        }
        StartCoroutine(WaitCoroutine());



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
        if (particle != null)
        {
            Destroy(particle.gameObject);
        }
    }
    public IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(1f);
        isShooting = false;
    }

    public IEnumerator SpawnCoroutine()
    {
        yield return new WaitForSeconds(4f);
        animator.SetTrigger("Spawn");
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponentInChildren<SortingGroup>().sortingOrder = 5;
        isSpawning = false;
    }
}