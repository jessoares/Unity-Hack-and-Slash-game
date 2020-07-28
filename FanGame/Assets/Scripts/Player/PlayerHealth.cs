using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Space]
    [Header("Game Object")]
    public GameObject player;
    [Space]
    [Header("Life values")]
    float currentHealth;
    public int maxHealth;
    [Space]
    [Header("Health UI")]
    public int numOfHearts;
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Space]
    [Header("Damage and Death")]
    public bool isDead;
    public bool isBlocking;
    public bool isHurt;

    [Space]
    [Header("Animation and Effects")]
    public ParticleSystem blockEffect;
    public Animator animator;
    public Animator hurtAnimation;
    public Color flashColor;
    public Color regularColor;
    public float flashDuration;
    public int numberOffFlashes;
    public SpriteRenderer mySprite;
    void Start()
    {
        currentHealth = maxHealth;
        isDead = false;
        isBlocking = false;
        isHurt = false;
    }
    private void Update()
    {
        //set HUD off if player is dealt damage to
        for(int i = 0; i < hearts.Length;i++)
        {
            if(i < currentHealth)
            {
                //hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].gameObject.SetActive(false);
            }
        }
    }
    public void TakeDamage(int damage)
    {
        currentHealth = currentHealth - damage;
        if (currentHealth >= 1)
        {
            StartCoroutine(FlashCoroutine());
            animator.SetTrigger("Hurt");
            hurtAnimation.SetTrigger("isHurt");
        }
        else 
        {
            GetComponent<PlayerAttack>().enabled = false;
            GetComponent<PlayerController>().playerRb.velocity = Vector2.zero;
            GetComponent<PlayerController>().enabled = false;
            animator.SetTrigger("Hurt");
            Die();
        }


    }
    void Die()
    { 
        animator.SetTrigger("Dead");
        StartCoroutine(DeathCoroutine());
        FindObjectOfType<GameManager>().EndGame();
  
    }
    public IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(2f);
        this.gameObject.SetActive(false);

    }

    //checks if player object's collider is triggered by other colliders
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Projectile") && other.GetComponent<Projectile>().reflected == false)
        {
            //if contact happens but player is blocking arrow is supposed to be reverted
            if (isBlocking == true)
            {
                other.transform.position = GetComponent<PlayerController>().sword.transform.position;
                other.GetComponent<Rigidbody2D>().rotation = other.GetComponent<Rigidbody2D>().rotation + Random.Range(-40f, 40f);
                other.GetComponent<Projectile>().dir = Vector2.left;
                other.GetComponent<Projectile>().reflected = true;
                ParticleSystem slashVelocity = Instantiate(blockEffect, GetComponent<PlayerController>().sword.position, Quaternion.identity);
                StartCoroutine(ParticleCoroutine(slashVelocity));
            }
            //if not, take damage
            else if(isBlocking == false && isHurt == false)
            {
                if (GetComponent<PlayerHealth>().isDead == false)
                {
                    other.GetComponent<Projectile>().DestroyProjectile();
                    TakeDamage(1);

                }
            }
        }


    }
    public void OnRaycastEnter2D(RaycastHit2D playerHit)
    {    
            if (isBlocking == true)
            {
                ParticleSystem slashVelocity = Instantiate(blockEffect, GetComponent<PlayerController>().sword.position, Quaternion.identity);
                StartCoroutine(ParticleCoroutine(slashVelocity));
            }
            //if not, take damage
            else if (isBlocking == false && isHurt == false)
            {
                if (GetComponent<PlayerHealth>().isDead == false)
                {
                    TakeDamage(1);

                }
            }
        }

    

    public IEnumerator ParticleCoroutine(ParticleSystem particle)
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(particle.gameObject);
    }

   public IEnumerator FlashCoroutine()
    {
        int temp = 0;
       isHurt = true;
        while (temp < numberOffFlashes)
        {
            mySprite.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            mySprite.color = regularColor;
            yield return new WaitForSeconds(flashDuration);
            temp++;
        }
        isHurt = false;
    }

}
