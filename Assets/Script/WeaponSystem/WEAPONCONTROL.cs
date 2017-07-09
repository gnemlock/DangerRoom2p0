using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>WEAPONCONTROL. SHOULD ALWAYS HAVE A SINGLE INSTANCE OPERATING, AND NEVER BE DUPLICATED OR INSTANTIATED.</summary>
public class WEAPONCONTROL : MonoBehaviour 
{
    /// <summary>Self-reference.</summary>
	public static WEAPONCONTROL weaponControl;

    /// <summary>Enumeration of possible firing modes.</summary>
    public enum WEAPONFIRETYPE { SINGLE, BURST, AUTO };

    #region ATTATCHMENTS

    // Lists containing reference to the attatchment variations available for random generation
    #region Attatchment Libraries

    public List<GameObject> BarrelLibrary = new List<GameObject>();
    public List<GameObject> HammerLibrary = new List<GameObject>();
    public List<GameObject> MagazineLibrary = new List<GameObject>();
	public List<GameObject> SightLibrary = new List<GameObject>();
	public List<GameObject> StockLibrary = new List<GameObject>();
    public List<GameObject> TriggerLibrary = new List<GameObject>();

    #endregion

    #region Attatchment: Barrel

    /// <returns>The count of available barrels for random generation.</returns>
    public int BarrelCount()
    {
        return BarrelLibrary.Count;
    }

    /// <summary>Picks a random barrel from the barrel library.</summary>
    /// <returns>The picked barrel.</returns>
    public WeaponBarrel GetRandomBarrel()
    {
        float x = Time.time * 1000;
        int y = Mathf.RoundToInt(x);
        int z = y % BarrelLibrary.Count;

        GameObject barrel = Instantiate(BarrelLibrary[z]);
        return barrel.GetComponent<WeaponBarrel>();
    }

    #endregion

    #region Attatchment: Hammer

    /// <returns>The count of available hammers for random generation.</returns>
    public int HammerCount()
    {
        return HammerLibrary.Count;
    }

    /// <summary>Picks a random hammer from the hammer library.</summary>
    /// <returns>The picked hammer.</returns>
    public WeaponHammer GetRandomHammer()
    {
        float x = Time.time * 1000;
        int y = Mathf.RoundToInt(x);
        int z = y % HammerLibrary.Count;

        GameObject hammer = Instantiate(HammerLibrary[z]);
        return hammer.GetComponent<WeaponHammer>();
    }

    #endregion

    #region Attatchment: Magazine

    /// <returns>The count of available magazines for random generation.</returns>
    public int MagazineCount()
    {
        return MagazineLibrary.Count;
    }

    /// <summary>Picks a random magazine from the magazine library.</summary>
    /// <returns>The picked magazine.</returns>
    public WeaponMagazine GetRandomMagazine()
    {
        float x = Time.time * 1000;
        int y = Mathf.RoundToInt(x);
        int z = y % MagazineLibrary.Count;

        GameObject magazine = Instantiate(MagazineLibrary[z]);
        return magazine.GetComponent<WeaponMagazine>();
    }

    #endregion

    #region Attatchment: Sight

    /// <returns>The count of available sights for random generation.</returns>
    public int SightCount()
    {
        return SightLibrary.Count;
    }

    /// <summary>Picks a random sight from the sight library.</summary>
    /// <returns>The picked sight.</returns>
    public GameObject GetRandomSight()
    {
        float x = Time.time * 1000;
        int y = Mathf.RoundToInt(x);
        int z = y % SightLibrary.Count;

        GameObject sight = Instantiate(SightLibrary[z]);
        return sight;
    }

    #endregion

    #region Attatchment: Stock

    /// <returns>The count of available stocks for random generation.</returns>
    public int StockCount()
    {
        return StockLibrary.Count;
    }

    /// <summary>Picks a random stock from the stock library.</summary>
    /// <returns>The picked stock.</returns>
    public WeaponStock GetRandomStock()
    {
        float x = Time.time * 1000;
        int y = Mathf.RoundToInt(x);
        int z = y % StockLibrary.Count;

        GameObject stock = Instantiate(StockLibrary[z]);
        return stock.GetComponent<WeaponStock>();
    }

    #endregion

    #region Attatchment: Trigger

    /// <returns>The count of available triggers for random generation.</returns>
    public int TriggerCount()
    {
        return TriggerLibrary.Count;
    }

    /// <summary>Picks a random trigger from the trigger library.</summary>
    /// <returns>The picked trigger.</returns>
    public GameObject GetRandomTrigger()
    {
        float x = Time.time * 1000;
        int y = Mathf.RoundToInt(x);
        int z = y % TriggerLibrary.Count;

        GameObject trigger = Instantiate(TriggerLibrary[z]);
        return trigger;
    }

    #endregion
    
    #endregion

    void Start()
	{
		weaponControl = this;
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			//GetRandomSight();
		}
	}
}





	