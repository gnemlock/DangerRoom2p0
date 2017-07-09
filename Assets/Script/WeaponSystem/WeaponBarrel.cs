using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>Handles bullet trajectory and firing. 
/// Origin of actual game object should be the bullets logical resting position before being triggered.</summary>
public class WeaponBarrel : Attatchment 
{
    /// <summary>Maximum distance bullets can travel from origin inside barrel.</summary>
    public float maximumRange;

    /// <summary>References a successful hit for calling reference to the hit gameobject</summary>
    RaycastHit hitInfo;
    /// <summary>References an enemy script for calling reaction to successful hits.</summary>
    //Enemy hitTarget;

    public void FireBullet()
    {
        /*if(Physics.Raycast(transform.position, transform.forward, out hitInfo, maximumRange, 1))
        {
            hitTarget = hitInfo.transform.GetComponent<Enemy>();    // link hitTarget to a reference of the hit transforms Enemy script, if available.

            if(hitTarget != null)   // if hitTarget contains a script reference, we have hit an enemy.
            {
                //hitTarget.TakeHit(hitInfo.point);
            }
        }

        hitTarget = null;   // lose our reference to the hitTarget*/
    }
}
