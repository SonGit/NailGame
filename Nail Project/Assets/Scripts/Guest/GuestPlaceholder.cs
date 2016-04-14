using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GuestPlaceholder : MonoBehaviour {

	public Animation anim ;

	public GuestType _type;
	
	public int _requiredJewel;
	
	public int _requiredQuantity;

	public ItemType _requiredItem;
	
	public Image _jewelSprite;

	public Image _itemSprite;

	public Text _quantityLabel;

	public SpriteRenderer _guestSprite;

	public Image _ballonSprite;

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

		//_guestSprite.sprite = ResourcesMgr.GetSprite ( GetGuestSpriteName(guestType) );
		_jewelSprite.sprite = Resources.Load<Sprite>( GetJewelSpriteName() );
		_quantityLabel.text = quantity.ToString ();

		_requiredItem = ItemType.NONE;

		_jewelSprite.enabled = false;
		_quantityLabel.enabled = false;
		_ballonSprite.enabled = false;
		anim.Play("GuestMoveIn");

		Invoke ("StartOrder",2.1f);
	}

	string GetJewelSpriteName()
	{
		string prefix = "Sprites/Order Icon/coloricon";
		string url = prefix + (_requiredJewel + 1).ToString ();

		return url;
	}

	void StartOrder()
	{
		_jewelSprite.enabled = true;
		_quantityLabel.enabled = true;
		_ballonSprite.enabled = true;
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

	public void OnEndMoveOut () {
		//_guestManager.RefillGuest (this);
	}

	string GetGuestSpriteName(GuestType type)
	{
		return "char1";
	}

	void MoveOut()
	{
		_isActive = false;
		_readyToTakeOrder = false;
		_quantityLabel.text = "OK!!";
		_guestManager.OnLeaveQueue ();

		Invoke ("PlayMoveout_Anim",1);


	}

	void PlayMoveout_Anim()
	{
		_jewelSprite.enabled = false;
		_quantityLabel.enabled = false;
		_ballonSprite.enabled = false;

		anim.Play ("GuestMoveOut");

		Invoke ("OnEndMoveOut",2f);
	}
}

