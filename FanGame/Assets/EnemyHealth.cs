using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Space]
    [Header("General")]
    public int maxHealth;
    int currentHealth;
    public bool isDead;
    GameObject spawn;
    [Header("Animation")]
    public Animator animator;
    

    private void Awake()
    {
        spawn = GameObject.FindGameObjectWithTag("Manager"); // locates the game manager for spawning logic     
        isDead = false;
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
        isDead = true;
        spawn.GetComponent<WaveSpawner>().GetSpawnNumber();
        animator.SetBool("Dead", true);
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(DeathCoroutine());

    }
    public IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);

    }
}
