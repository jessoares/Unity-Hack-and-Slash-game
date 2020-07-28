using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowSpecial : MonoBehaviour
{
    [Space]
    [Header("Special")]
    public float specialTimer = 0f;
    private Vector3 specialDir;
    public float specialRate = 1f;




    void Update()
    {
        if (Time.time >= specialTimer)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                if (GetComponent<PlayerController>().mousePos.x < transform.position.x && GetComponent<PlayerController>().facingRight == true)
                {
                    GetComponent<PlayerController>().Flip();
                }
                else if (GetComponent<PlayerController>().mousePos.x > transform.position.x && GetComponent<PlayerController>().facingRight == false)
                {
                    GetComponent<PlayerController>().Flip();

                }
                GetComponent<PlayerAttack>().isSpecial = true;
                SpecialAttack();
                specialTimer = Time.time + 1f / specialRate;

            }
        }
    }
    void SpecialAttack()
    {
        GetComponent<PlayerAttack>().animator.SetBool("IsBlocking", false);
        GetComponent<PlayerHealth>().isBlocking = false;
        GetComponent<PlayerAttack>().blockTime = 0f;
        float specialDistance = 5f;

        GetComponent<PlayerAttack>().animator.SetTrigger("Attack");

        specialDir = GetComponent<PlayerController>().lookDir; //direction of the special

        specialDistance = GetComponent<PlayerController>().returnDistance(specialDir, specialDistance);

        RaycastHit2D[] hitEnemies = Physics2D.CircleCastAll(transform.position, 0.2f, specialDir, specialDistance, GetComponent<PlayerAttack>().enemyLayers); //collider for the special attack

        foreach (RaycastHit2D enemy in hitEnemies)
        {
            enemy.collider.gameObject.GetComponent<EnemyHealth>().TakeDamage(100); //for every enemy collider detected, deals damage to each gameobject
        }
        GetComponent<PlayerController>().TrySpecial(specialDir, specialDistance); //does the special's movement logic 
        GetComponent<PlayerAttack>().isSpecial = false;
    }



}

