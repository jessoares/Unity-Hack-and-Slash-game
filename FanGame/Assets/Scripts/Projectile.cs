using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float arrowForce;
    public Vector2 dir;
    public bool reflected;
    public Transform position;
    void Start()
    {
        dir = Vector2.right;
        reflected = false;
        Destroy(gameObject,15f);
    }
    void Update()
    {
        transform.Translate(dir * Time.deltaTime * arrowForce); //translates the arrow at each frame in the direction of Vector2 dir
    }
    public void DestroyProjectile()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && reflected == true)
        {
            other.GetComponent<EnemyHealth>().TakeDamage(100);
        }
    }
     
    

}