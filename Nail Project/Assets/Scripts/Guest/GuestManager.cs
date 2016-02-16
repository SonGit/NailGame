using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GuestManager : MonoBehaviour {
	
	public GuestPlaceholder[] _guests;
	public Text _numQueueLabel;
	int _numqueue;
	int _totalGuest;
	public int _numQueue
	{
		get
		{
			return _numqueue;
		}

		set
		{
			_numqueue = value;
			if(value > -1)
			_numQueueLabel.text = _numQueue.ToString ();
		}
	}

	int _currentOrderNo;

	void Awake()
	{

	}
	
	// Use this for initialization
	void Start () {
		_currentOrderNo = 0;
		_numQueue = 2;
		_totalGuest = _numQueue + 3; //total = guest on the bar + queued guest
		InitGuests ();
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
		if (_numQueue <= 0)
			return;

		_numQueue --;

		InitGuest (guest);
	}

	void InitGuest(GuestPlaceholder guest)
	{
		int rand = UnityEngine.Random.Range (0,2);
		//if(rand == 0)
		guest.Init( GetRandomizedGuestType(), GetRandomizedJewelType(), GetRamdomizedQuantity(),_currentOrderNo ++ );
		//else
			//guest.Init( GetRandomizedGuestType(), GetRandomizedItemType() );
	}
	
	public GuestPlaceholder GetGuestThatNeedJewel(int jewelType)
	{
		List<GuestPlaceholder> guestList = new List<GuestPlaceholder>();
		foreach (GuestPlaceholder guest in _guests) {
			if(guest._requiredJewel == jewelType && guest._readyToTakeOrder)
			{
				guestList.Add(guest);
			}
		}

		if (guestList.Count == 0)
			return null;
		else 
			return GetPrioritizedGuest (guestList);
	}

	public GuestPlaceholder GetGuestThatNeedItem(ItemType itemType)
	{
		List<GuestPlaceholder> guestList = new List<GuestPlaceholder>();

		foreach (GuestPlaceholder guest in _guests) {
			if(guest._requiredItem == itemType && guest._readyToTakeOrder)
			{
				guestList.Add(guest);
			}
		}

		if (guestList.Count == 0)
			return null;
		else 
			return GetPrioritizedGuest (guestList);
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

	public void FillGuest(int jewelType,int score)
	{
		var guestFound = GetGuestThatNeedJewel (jewelType);
		if (guestFound != null)
			guestFound.Fill (score);
	}

	GuestPlaceholder GetPrioritizedGuest(List<GuestPlaceholder> guestList)
	{
		//Old guests has priority over recently added guest
		guestList = guestList.OrderBy (v => v._numOrder).ToList();
		return guestList [0];
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
	
	public void OnLeaveQueue()
	{
		_totalGuest --;

		if(_totalGuest <= 0)
			Supporter.sp.StartEndgamePhase();
	}
	
}
