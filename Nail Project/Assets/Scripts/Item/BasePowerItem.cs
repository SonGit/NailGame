using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BasePowerItem : MonoBehaviour {

	public SpriteRenderer _renderer;
	
	public ItemType _type;
	
	public Cell[] _cells; //Cellls that need to be unlocked first to unlock this item
	
	public int _maxCells;

	public int counter = 0;
	
	Dictionary<ItemType,string> _spriteNames =  new Dictionary<ItemType, string>()
	{
		{ItemType.ICE_CREAM,"item_1"}
	};
	
	void Start()
	{

	}
	
	public void Init(ItemType type,Cell[] cells)
	{
		_renderer.sprite = ResourcesMgr.GetSprite (_spriteNames[type]);
		this._type = type;
		this._cells = cells;
	}
	
	public void FlyTo(Vector3 destination)
	{
		iTween.MoveTo(gameObject,iTween.Hash(
			"position"   , destination,
			"time", 4f,
			"oncomplete" , "OnArrived",
		"oncompletetarget" , this.gameObject));
		
		iTween.ScaleTo(gameObject,iTween.Hash(
			"x"   , 0.4f,
			"y"   , 0.4f,
			"time", 4f,
			"oncomplete" , "OnRemoveItem",
			"oncompletetarget" , this.gameObject));
	}

	public void CheckForItemCompletion(Cell cellToCompare)
	{
		foreach(Cell cell in _cells)
		{
			if(cell.CellPosition.x == cellToCompare.CellPosition.x && cell.CellPosition.y == cellToCompare.CellPosition.y)
			{
				counter ++;
			}
		}

		if (counter >= _maxCells)
			OnComleted ();

	}

	public void ForceComplete()
	{
		OnComleted ();
	}

	void OnComleted()
	{
		GuestPlaceholder guest = GameController.action._guestManager.GetGuestThatNeedItem (_type);
		if(guest != null)
		FlyTo (guest.transform.position);
	}
	
	void OnArrived()
	{
		GameController.action._guestManager.GiveItemToGuest (this);
		RemoveItem ();
	}

	void RemoveItem()
	{
		Destroy (gameObject);
	}
}
