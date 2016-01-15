using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class GuestManager : MonoBehaviour {
	
	public GuestPlaceholder[] _guests;
	public Text _numQueueLabel;
	public int _numQueue;

	void Awake()
	{
		ResourcesMgr.LoadResources (ResourcesMgr.ResourceType.SPRITE,
		                            ResourcesMgr.GuestItemAnim);
		ResourcesMgr.LoadResources (ResourcesMgr.ResourceType.SPRITE,
		                            ResourcesMgr.ItemSprites);
	}
	
	// Use this for initialization
	void Start () {
		InitGuests ();
		
		_numQueue = 5;
		_numQueueLabel.text = _numQueue.ToString ();

		
	}
	
	// Update is called once per frame
	public void InitGuests()
	{
		foreach (GuestPlaceholder guest in _guests) {
			InitGuest(guest);
		}
	}
	
	public void RefillGuest(GuestPlaceholder guest)
	{
		OnLeaveQueue ();
		InitGuest (guest);
	}

	void InitGuest(GuestPlaceholder guest)
	{
		int rand = UnityEngine.Random.Range (0,2);
		//if(rand == 0)
			//guest.Init( GetRandomizedGuestType(), GetRandomizedJewelType(), GetRamdomizedQuantity() );
		//else
			guest.Init( GetRandomizedGuestType(), GetRandomizedItemType() );
	}
	
	public GuestPlaceholder GetGuestThatNeedJewel(int jewelType)
	{
		foreach (GuestPlaceholder guest in _guests) {
			if(guest._requiredJewel == jewelType)
			{
				return guest;
			}
		}
		return null;
	}

	public GuestPlaceholder GetGuestThatNeedItem(ItemType itemType)
	{
		foreach (GuestPlaceholder guest in _guests) {
			if(guest._requiredItem == itemType)
			{
				return guest;
			}
		}
		return null;
	}

	public void GiveItemToFirstFoundGuest()
	{
		foreach (GuestPlaceholder guest in _guests) {
			if(guest._requiredItem != ItemType.NONE && guest._requiredItem != null)
			{
				guest.GiveItem();
				return;
			}
		}
	}

	public void FillGuest(int itemType,int score)
	{
		foreach (GuestPlaceholder guest in _guests) {
			if(guest._requiredJewel == itemType)
			{
				guest.Fill(score);
				return;
			}
		}
	}

	public void GiveItemToGuest(BasePowerItem item)
	{
		foreach (GuestPlaceholder guest in _guests) {
			if(guest._requiredItem == item._type)
			{
				guest.GiveItem();
				return;
			}
		}
	}
	
	private GuestType GetRandomizedGuestType()
	{
		int rand = UnityEngine.Random.Range ( 0,(int)Enum.GetNames(typeof(GuestType)).Length-1 );
		return (GuestType)rand;
	}

	private ItemType GetRandomizedItemType()
	{
		return ItemType.ICE_CREAM;
	}
	
	private int GetRandomizedJewelType()
	{
		int r = 0;
		
		if (PLayerInfo.MODE == 1)
			r = UnityEngine.Random.Range(0, 6);
		else
			r = UnityEngine.Random.Range(0, 7);

		return r;
	}
	
	private int GetRamdomizedQuantity()
	{
		return UnityEngine.Random.Range (3,10);
	}
	
	private void OnLeaveQueue()
	{
		if (_numQueue > -1) {
			_numQueue --;
			_numQueueLabel.text = _numQueue.ToString ();
		}
	}
}
