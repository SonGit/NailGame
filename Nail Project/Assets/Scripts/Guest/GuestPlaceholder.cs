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

	void Start () {
		_guestManager = this.GetComponentInParent<GuestManager>();
	}
	
	public void Init(GuestType guestType,int jewelType,int quantity )
	{
		GuestUI_Jewel ();

		_type = guestType;
		_requiredJewel = jewelType;
		_requiredQuantity = quantity;

		_guestSprite.sprite = ResourcesMgr.GetSprite ( GetGuestSpriteName(guestType) );
		_jewelSprite.sprite = JewelSpawner.spawn.GetJewelSprite (jewelType);
		_quantityLabel.text = quantity.ToString ();

		_requiredItem = ItemType.NONE;

		anim.Play("GuestMoveIn");
	}

	public void Init(GuestType guestType,ItemType itemType)
	{
		GuestUI_Item ();

		_type = guestType;
		_requiredItem = itemType;
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
		if (_requiredItem != ItemType.NONE)
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
		anim.Play ("GuestMoveOut");
		_quantityLabel.text = "OK!!";
	}
}

