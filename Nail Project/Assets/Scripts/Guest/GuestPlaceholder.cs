using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GuestPlaceholder : MonoBehaviour {

	public Animation anim ;

	public GuestType _type;
	
	public int _requiredJewel;
	
	public int _requiredQuantity;

	public ItemType _requiredItem;
	
	public SpriteRenderer _jewelSprite;

	public SpriteRenderer _itemSprite;

	public Text _quantityLabel;

	public SpriteRenderer _guestSprite;

	private GuestManager _guestManager;

	public bool _readyToTakeOrder;

	public bool _isActive;

	public int _numOrder; //order of appearing

	void Start () {
		_guestManager = this.GetComponentInParent<GuestManager>();
	}
	
	public void Init(GuestType guestType,int jewelType,int quantity,int numQueue )
	{
		GuestUI_Jewel ();

		_type = guestType;
		_requiredJewel = jewelType;
		_requiredQuantity = quantity;
		_readyToTakeOrder = false;
		_isActive = true;
		_numOrder = numQueue;

		_guestSprite.sprite = ResourcesMgr.GetSprite ( GetGuestSpriteName(guestType) );
		_jewelSprite.sprite = JewelSpawner.spawn.GetJewelSprite (jewelType);
		_quantityLabel.text = quantity.ToString ();

		_requiredItem = ItemType.NONE;

		_jewelSprite.enabled = false;
		_quantityLabel.enabled = false;
		anim.Play("GuestMoveIn");

		Invoke ("StartOrder",2.1f);
	}

	void StartOrder()
	{
		_jewelSprite.enabled = true;
		_quantityLabel.enabled = true;
		_readyToTakeOrder = true;
		_isActive = true;
	}

	public void Init(GuestType guestType,ItemType itemType,int numQueue)
	{
		GuestUI_Item ();

		_type = guestType;
		_requiredItem = itemType;
		_numOrder = numQueue;
		_guestSprite.sprite = ResourcesMgr.GetSprite ( GetGuestSpriteName(guestType) );

		anim.Play("GuestMoveIn");
	}

	public void GiveItem()
	{
		MoveOut ();
	}

	void GuestUI_Item()
	{
		_jewelSprite.enabled = false;
		_quantityLabel.enabled = false;
		_itemSprite.enabled = true;
	}

	void GuestUI_Jewel()
	{
		_jewelSprite.enabled = true;
		_quantityLabel.enabled = true;
		_itemSprite.enabled = false;
	}


	public void Fill(int score)
	{
		if (_requiredItem != ItemType.NONE || !_readyToTakeOrder)
			return;

		_requiredQuantity -= score;

		if (_requiredQuantity <= 0) {
			MoveOut();
		} else {
			_quantityLabel.text = _requiredQuantity.ToString ();
		}

	}
	

	// This C# function can be called by an Animation Event
	public void OnEndMoveIn () {

	}

	public void OnEndMoveOut () {
		_guestManager.RefillGuest (this);
	}

	string GetGuestSpriteName(GuestType type)
	{
		switch (type) {
		case GuestType.BEE:
			return "char1";
		case GuestType.FIREFLY:
			return "char2";
		case GuestType.HUMMING_BIRD:
			return "char3";
		default:
			return "char1";
		}
	}

	void MoveOut()
	{
		_isActive = false;
		_readyToTakeOrder = false;
		_quantityLabel.text = "OK!!";
		_guestManager.OnLeaveQueue ();

		Invoke ("PlayMoveout_Anim",2);
	}

	void PlayMoveout_Anim()
	{
		anim.Play ("GuestMoveOut");
	}
}

