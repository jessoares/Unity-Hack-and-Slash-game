using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RANGE : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange;
    private void OnDrawGizmosSelected()//attack collider visualization in-engine
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
