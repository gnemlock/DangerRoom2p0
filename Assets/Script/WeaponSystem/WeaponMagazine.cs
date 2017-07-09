using UnityEngine;
using System.Collections;

/// <summary>Holds the weapon ammunition, applying logic for ammo checks and reloading.</summary>
public class WeaponMagazine : Attatchment
{
    #region Ammo Counts

    /// <summary>Maximum amount of bullets held by magazine.</summary>
	public int magazineCapacity;
    /// <summary>Current amount of bullets held by magazine.</summary>
	public int magazineLoaded;

    public int currentBullets = 10;

    #endregion

    /// <summary>Checks if the current weapon has ammunition loaded and proceeds to shoot the weapon if there is.
    /// Should only be checked when the player fires, as function also handles deducting the bullet if it is available
    /// to fire.</summary>
	/// <returns><c>true</c>, if weapon has ammo loaded and can fire, else <c>false</c>.</returns>
	public bool AmmoCheck()
	{
        if(magazineLoaded >= 1)     // if there are bullets loaded into the magazine
        {
            magazineLoaded--;       // decrease loaded bullets by one as we prepare to fire
            return true;            // return true to confirm continue
        }
        else                        // if there are no bullets loaded into the magazine
        {
            // reload               // as the player has attempted to fire, with no bullets, we should reload.
            return false;           // return false to deny continue
        }
	}

    /// <summary>Cross-references the amount of ammo needed for a complete reload with the players current ammo count, 
    /// applying logic to determine if a full or partial reload is possible, and completing the reload and associated
    /// change to the player ammo reserve if possible.</summary>
    public void Reload()
    {
        int ammoRequest = magazineCapacity - magazineLoaded;    // determine how many bullets are required to completely refill magazine
        int ammoReserve = currentBullets;

        if (ammoReserve >= 1)       // confirm we have ammo in our ammo reserve
        {
            if (ammoReserve >= ammoRequest)     // if we have enough ammo reserved to refill magazine
            {
                magazineLoaded = magazineCapacity;      // perform a full reload, magazine now holds maximum ammo
                ammoReserve -= ammoRequest;             // subtract passed ammo count from ammo reserve to update players ammo count
            }
            else                                // we don't have enough bullets left to completely refill magazine
            {
                magazineLoaded += ammoReserve;          // reload the amount of remaining bullets into magazine
                ammoReserve = 0;                        // we have used up all our ammo reserve, so ammoReserve = 0
            }

            // visual reloading is unaffected by the amount of bullets loaded. do this here.

            currentBullets = ammoReserve;   // update player's bullet count
        }
        else                        // we don't have any ammo left to reload
        {
            // NO AMMO!!
        }   
    }
}
