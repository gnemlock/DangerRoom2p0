using UnityEngine;
using System.Collections;

/// <summary>The weapon class holds instances of weapons, grouping together the attatchments and handling
/// any interaction between them.</summary>
public class Weapon : MonoBehaviour 
{
    /*  FIRING A BULLET - pseudo
     *  1 - player pulls trigger
     *  2 - check if we have ammo loaded; if not, see if we can reload
     *  3 - apply trigger bonuses to bullet and check fire mode
     *  4 - fire bullet
     *  5 - if not single shot, proceed with burst fire or auto fire
     */

    /// <summary>Available firing types for weapon.</summary>
    public WEAPONCONTROL.WEAPONFIRETYPE[] availableFireTypes;
    /// <summary>Currently selected firing type.</summary>
    public WEAPONCONTROL.WEAPONFIRETYPE currentFireType;

    /// <summary>Reference to the bullet used when firing weapon.</summary>
    public Bullet weaponBullet;

#region Testing

    /// <summary>Testing Mode</summary>
    public bool TESTING;

#endregion

    #region Attatchments ----------------------------------------------------------------------

	#region BARREL
	/// <summary>Anchor game object for attatching the barrel. Should be a child of the 
	/// main weapon game object, and follow uniformity across anchors.</summary>
	public GameObject barrelAnchor;
	/// <summary>The current <c>WeaponBarrel</c>.</summary>
	public WeaponBarrel attatchedBarrel;
	/// <summary>Sets a <c>WeaponBarrel</c> randomly, from applicable candidates,
	/// and attatches it to the Weapon.</summary>
	void SetBarrel()
	{
		//WeaponBarrel barrel;
		//SetBarrel (barrel);
	}
	/// <summary>Sets a specified <c>WeaponBarrel</c> to the Weapon Barrel attatchment.</summary>
	/// <param name="barrel">Barrel to attatch.</param>
	void SetBarrel(WeaponBarrel barrel)
	{
		attatchedBarrel = barrel;
	}
	#endregion

    #region HAMMER
    /// <summary>Anchor game object for attatching the hammer. Should be a child of the 
    /// main weapon game object, and follow uniformity across anchors.</summary>
    public GameObject hammerAnchor;
    /// <summary>The current <c>WeaponMagazine</c>.</summary>
    public WeaponHammer attatchedHammer;
    /// <summary>Sets a <c>WeaponHammer/c> randomly, from applicable candidates,
    /// and attatches it to the Weapon.</summary>
    void SetHammer()
    {
        //WeaponHammer hammer;
        //SetHammer (hammer);
    }
    /// <summary>Sets a specified <c>WeaponHammer</c> to the Weapon Hammer attatchment.</summary>
    /// <param name="hammer">Hammer to attatch.</param>
    void SetHammer(WeaponHammer hammer)
    {
        attatchedHammer = hammer;
    }
    #endregion

	#region MAGAZINE
	/// <summary>Anchor game object for attatching the magazine. Should be a child of the 
	/// main weapon game object, and follow uniformity across anchors.</summary>
	public GameObject magazineAnchor;
	/// <summary>The current <c>WeaponMagazine</c>.</summary>
	public WeaponMagazine attatchedMagazine;
	/// <summary>Sets a <c>WeaponMagazine/c> randomly, from applicable candidates,
	/// and attatches it to the Weapon.</summary>
	void SetMagazine()
	{
		//WeaponMagazine magazine;
		//SetMagazine (magazine);
	}
	/// <summary>Sets a specified <c>WeaponMagazine</c> to the Weapon Magazine attatchment.</summary>
	/// <param name="magazine">Magazine to attatch.</param>
	void SetMagazine(WeaponMagazine magazine)
	{
		attatchedMagazine = magazine;
	}
	#endregion

	#region SIGHT
	/// <summary>Anchor game object for attatching the sight. Should be a child of the 
	/// main weapon game object, and follow uniformity across anchors.</summary>
	public GameObject sightAnchor;
	/// <summary>The current <c>WeaponSight</c> attatched to the weapon.
	/// <c>NULL</c> if no scope is attatched.</summary>
	public WeaponSight attatchedSight;

	/// <summary>Sets a <c>WeaponSight</c> randomly, from applicable candidates,
	/// and attatches it to the Weapon.</summary>
	void SetScope()
	{
		//WeaponScope scope;
		//SetScope (scope);
	}
	/// <summary>Sets a specified <c>WeaponSight</c> to the Weapon Sight attatchment.</summary>
	/// <param name="sight">Sight to attatch.</param>
	void SetSight(WeaponSight sight)
	{
		attatchedSight = sight;
	}
	#endregion

	#region STOCK
	/// <summary>Anchor game object for attatching the stock. Should be a child of the 
	/// main weapon game object, and follow uniformity across anchors.</summary>
	public GameObject stockAnchor;
	/// <summary>The current <c>WeaponStock</c>.</summary>
	public WeaponStock attatchedStock;
	/// <summary>Sets a <c>WeaponStock/c> randomly, from applicable candidates,
	/// and attatches it to the Weapon.</summary>
	void SetStock()
	{
		//WeaponStock stock;
		//SetStock (stock);
	}
	/// <summary>Sets a specified <c>WeaponStock</c> to the Weapon Stock attatchment.</summary>
	/// <param name="stock">Stock to attatch.</param>
	void SetStock(WeaponStock stock)
	{
		attatchedStock = stock;
	}
	#endregion

	#region TRIGGER

	/// <summary>Anchor game object for attatching the trigger. Should be a child of the 
	/// main weapon game object, and follow uniformity across anchors.</summary>
	public GameObject triggerAnchor;
	/// <summary>The current <c>WeaponStock</c>.</summary>
	public WeaponTrigger attatchedTrigger;

    #region Setting the Trigger

    /// <summary>Sets a <c>WeaponTrigger/c> randomly, from applicable candidates,
	/// and attatches it to the Weapon.</summary>
	void SetTrigger()
	{
		//WeaponTrigger (trigger;
		//SetTrigger (trigger);
	}

	/// <summary>Sets a specified <c>WeaponTrigger</c> to the Weapon Trigger attatchment.</summary>
	/// <param name="trigger">Trigger to attatch.</param>
	void SetTrigger(WeaponTrigger trigger)
	{
		attatchedTrigger = trigger;
	}

    #endregion

    #endregion

    #endregion ----------------------------------------------------------------------------------

    void Start()
	{
		// go through the attatchments and physically attatch them to the main weapon

		attatchedBarrel.Attatch (barrelAnchor.transform.position, this.transform);
		attatchedStock.Attatch (stockAnchor.transform.position, this.transform);
		attatchedSight.Attatch (sightAnchor.transform.position, this.transform);
		attatchedMagazine.Attatch (magazineAnchor.transform.position, this.transform);
		attatchedTrigger.Attatch (triggerAnchor.transform.position, this.transform);
        attatchedHammer.Attatch(hammerAnchor.transform.position, this.transform);
	}

	void Update()
	{

	}

	#region Player Interaction

	#endregion

    /// <summary>Checks if the weapon can fire, and proceeds if firing conditions are met.</summary>
    void FireWeapon()
    {
        if(attatchedMagazine.AmmoCheck())   // if there is ammo available in the magazine
        {
            attatchedBarrel.FireBullet();   // send the bullet to the barrel to be fired
        }
    }
}
