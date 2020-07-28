using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class BlockedBullet : MonoBehaviour
{
    public Vector2 target;
    Rigidbody2D arrow;
    Vector3 mousePos;
    public float arrowForce;
    public Transform player;



    // Update is called once per frame
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        mousePos = UtilsClass.GetMouseWorldPosition();

        arrow = GetComponent<Rigidbody2D>();


            float random = Random.Range(0f,260f);
          
            target = new Vector2(Mathf.Cos(random), Mathf.Sin(random));
     

      
        target = target.normalized * arrowForce;

        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;

        arrow.rotation = angle;

        arrow.velocity = new Vector2(target.x, target.y);

        Destroy(gameObject, 4f);


    }

    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            DestroyProjectile();
            other.GetComponent<EnemyHealth>().TakeDamage(100);
        }
    }



}