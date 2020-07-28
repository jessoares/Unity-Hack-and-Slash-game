using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Space]
    [Header("Attack")]
    public ParticleSystem blockEffect;
    public Transform attackPoint;
    public Transform chargeAttackPoint;
    public LayerMask enemyLayers;
    public LayerMask bulletLayers;
    public int attackDamage;
    public float attackRange;
    float nextAttackTime = 0f;
    public float attackRate;
    public ParticleSystem slash;
    public ParticleSystem chargeSlash;
    public Transform slashPoint;
    public Transform chargeSlashPoint;


    public bool isSpecial = false;
    public bool isBortz;
    public bool isDia;
    public float bortzTimer = 0;
    public bool bortzSpecialReady = false;
    public bool bortzSpecialLoad = false;
    public State state = State.Normal;
    public float timer = 0f;
    public enum State
    {
        Normal,
        Special    
    }
    [Space]
    [Header("Animation")]
    public Animator animator;
    public Animator slashAnimator;

    [Space]
    [Header("Blocking")]
    public float nextBlockTime;
    public float blockTime = 0f;
    public float nextBlock = 0f;
    public float blockRate = 1f;
    void Update()
    {
        ProcessInputs();
    }
    void Attack()
    {
        ParticleSystem slashVelocity = Instantiate(slash, slashPoint.position, GetComponent<PlayerController>().sword.rotation);
        slashVelocity.transform.parent = this.transform;
        if (GetComponent<PlayerController>().facingRight == false)
        {
            Vector3 theScale = slashVelocity.transform.localScale;
            theScale.x *= -1;
            slashVelocity.transform.localScale = theScale;
        }
        StartCoroutine(ParticleCoroutine(slashVelocity));
        animator.SetBool("IsBlocking", false);
        blockTime = 0f;
        animator.SetTrigger("Attack");
        StartCoroutine(SlashCoroutine());
        GetComponent<PlayerHealth>().isBlocking = false;

    }
    public IEnumerator SlashCoroutine()
    {
        //basically makes the code wait for 0.2 seconds to draw the attack's collider
        yield return new WaitForSeconds(0.15f);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        Collider2D[] hitBullet = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, bulletLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHealth>().TakeDamage(attackDamage);
        }
        if (isDia)
        {
            foreach (Collider2D bullet in hitBullet)
            {
                bullet.GetComponent<Rigidbody2D>().rotation = GetComponent<PlayerController>().angle + 90f;
                //bullet.GetComponent<Projectile>().dir = Vector2.left;
                bullet.GetComponent<Projectile>().reflected = true;
                ParticleSystem slashVelocity = Instantiate(blockEffect, bullet.transform.position, Quaternion.identity);
                StartCoroutine(ParticleCoroutine(slashVelocity));
            }
        }
        else
        {
            foreach (Collider2D bullet in hitBullet)
            {
                Destroy(bullet);
                ParticleSystem slashVelocity = Instantiate(blockEffect, bullet.transform.position, Quaternion.identity);
                StartCoroutine(ParticleCoroutine(slashVelocity));
            }
        }
    }
    public IEnumerator ParticleCoroutine(ParticleSystem particle)
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(particle.gameObject);
    }
    private void ProcessInputs()
    {
        //logic for the special attack's rate
        if (state == State.Normal && GetComponent<PlayerController>().state == PlayerController.State.Normal && isSpecial == false)
        {
            //logic for the attack's rate
            if (Time.time >= nextAttackTime && GetComponent<PlayerController>().state == PlayerController.State.Normal)
            {
                // if player presses mouse's right button do attack
                if (Input.GetButtonDown("Fire1"))
                {
                    Attack();
                    nextAttackTime = Time.time + 1f / attackRate;
                    bortzTimer = 0f;
                }
                if (isBortz == true && Input.GetButton("Fire1"))
                {
                    bortzTimer += Time.deltaTime;
                    GetComponent<PlayerController>().moveSpeed = Mathf.Lerp(5f, 0f, bortzTimer);
                    if (bortzTimer > 0.1f)
                    {
                        animator.SetBool("Charge", true);
                    }
                    if (bortzTimer > 0.5f)
                    {
                        bortzSpecialReady = true;
                        Debug.Log("LOADED");
                    }
                }
                if (isBortz == true && Input.GetButtonUp("Fire1") && bortzSpecialReady == true)
                {   
                    animator.SetBool("Charge", false);
                    state = State.Special;
                    GetComponent<PlayerController>().movement = Vector2.zero;
                    StartCoroutine(LockCoroutine());
                    animator.SetTrigger("Special");
                    StartCoroutine(BortzSpecialAttack());
                    bortzTimer = 0f;
                }
                if (isBortz == true && Input.GetButtonUp("Fire1") && bortzSpecialReady == false)
                {
                    animator.SetBool("Charge", false);
                    bortzTimer = 0f;
                    bortzSpecialReady = false;
                }


            }
            //logic for the block's rate
            if (Time.time >= nextBlock && GetComponent<PlayerHealth>().isBlocking == false && GetComponent<PlayerController>().state == PlayerController.State.Normal)
            {
                //blocking input
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    GetComponent<PlayerHealth>().isBlocking = true;
                    nextBlockTime = Time.time + 1f / blockRate;
                }
            }
            if (GetComponent<PlayerHealth>().isBlocking == true)
            {
                animator.SetBool("IsBlocking", true);
                blockTime += Time.deltaTime;
                if (blockTime > 3f)
                {
                    animator.SetBool("IsBlocking", false);
                    GetComponent<PlayerHealth>().isBlocking = false;
                    blockTime = 0f;
                }
            }
        }
    }

     public IEnumerator BortzSpecialAttack()
        {

            //basically makes the code wait for 0.2 seconds to draw the attack's collider
            yield return new WaitForSeconds(0.15f);
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(chargeAttackPoint.position, attackRange*2, enemyLayers);
            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<EnemyHealth>().TakeDamage(attackDamage);
            }
            bortzSpecialReady = false;
            GetComponent<PlayerController>().moveSpeed = GetComponent<PlayerController>().baseMoveSpeed;
            ParticleSystem slashVelocity = Instantiate(chargeSlash, chargeSlashPoint.position, GetComponent<PlayerController>().sword.rotation);
            slashVelocity.transform.parent = this.transform;
             if (GetComponent<PlayerController>().facingRight == false)
                    {
                        Vector3 theScale = slashVelocity.transform.localScale;
                        theScale.x *= -1;
                        slashVelocity.transform.localScale = theScale;
                    }
                    StartCoroutine(ParticleCoroutine(slashVelocity));
        }
    public IEnumerator LockCoroutine()
    {
        yield return new WaitForSeconds(0.8f);
        state = State.Normal;
    }

}

    

    
  

   

   


    



