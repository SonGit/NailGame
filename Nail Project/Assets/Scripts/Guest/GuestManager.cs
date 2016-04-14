using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GuestManager : MonoBehaviour {
	
	public GuestPlaceholder[] _guests;
	public Guest[] _guests2;

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
		foreach (Guest guest in _guests2) {
			InitGuest(guest);
		}
	}
	
	public void RefillGuest(Guest guest)
	{
		if (_numQueue <= 0)
			return;

		_numQueue --;

		InitGuest (guest);
	}

	void InitGuest(Guest guest)
	{
		int rand = UnityEngine.Random.Range (0,2);
		//if(rand == 0)
		guest.Init( GetRandomizedGuestType(), GetRandomizedJewelType(), GetRamdomizedQuantity(),_currentOrderNo ++);
		//else
			//guest.Init( GetRandomizedGuestType(), GetRandomizedItemType() );
	}
	
	public Guest GetGuestThatNeedShellac(int shellacType)
	{
		List<Guest> guestList = new List<Guest>();

		foreach (Guest guest in _guests2) {
			if(guest._requiredShellac == shellacType )
			{
				guestList.Add(guest);
			}
		}

		if (guestList.Count == 0)
			return null;
		else 
			return GetPrioritizedGuest (guestList);
	}

	public Guest GetGuestThatNeedItem(ItemType itemType)
	{
		//List<Guest> guestList = new List<Guest>();

		//foreach (Guest guest in _guests) {
			//if(guest._requiredItem == itemType && guest._readyToTakeOrder)
			//{
				//guestList.Add(guest);
			//}
		//}

		//if (guestList.Count == 0)
			//return null;
		//else 
			//return GetPrioritizedGuest (guestList);

		return null;
	}

	public void GiveItemToFirstFoundGuest()
	{
		foreach (GuestPlaceholder guest in _guests) {
			if(guest._requiredItem != ItemType.NONE)
			{
				guest.GiveItem();
				return;
			}
		}
	}

	public Guest FillGuest(int shellacType,int score)
	{
		var guestFound = GetGuestThatNeedShellac (shellacType);
		if (guestFound != null)
			guestFound.Fill (score);

		return guestFound;
	}

	Guest GetPrioritizedGuest(List<Guest> guestList)
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
