using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ItemType
{
	NONE,
	ICE_CREAM
}
public class Item : MonoBehaviour {

	public SpriteRenderer _renderer;

	public ItemType _type;

	public Cell[] _cells; //Cellls that need to be unlocked first to unlock this item

	public int _maxCells;

	Dictionary<ItemType,string> _spriteNames =  new Dictionary<ItemType, string>()
	{
		{ItemType.ICE_CREAM,"item_1"}
	};

	void Start()
	{
		Init(ItemType.ICE_CREAM);
	}

	public void Init(ItemType type)
	{
		_renderer.sprite = ResourcesMgr.GetSprite (_spriteNames[type]);
		this._type = type;
	}

	public void FlyTo(Vector3 destination)
	{
		iTween.MoveTo(gameObject,iTween.Hash(
			"position"   , destination,
			"time", 5f));

		iTween.ScaleTo(gameObject,iTween.Hash(
			"x"   , 0.4f,
			"y"   , 0.4f,
			"time", 5f,
			"oncomplete" , "OnRemoveItem",
			"oncompletetarget" , this.gameObject));
	}

	void OnRemoveItem()
	{
		Destroy (gameObject);
	}

}
