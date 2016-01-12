using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerItemManager : MonoBehaviour {

	public static PowerItemManager Instance ;

	public List<GameObject> _powerItems;

	public GameObject[] _powerItemPrefabs;

	public Transform _grid; // for parenting

	// Use this for initialization
	void Awake () {
		Instance = this;
		_powerItems = new List<GameObject>();
	}
	
	public void CreatePowerItem(Cell[] cells,ItemType type)
	{
		Cell firstCell = cells [0];
		GameObject powerItemObj = null;

		if (type == ItemType.ICE_CREAM) {
			powerItemObj = (GameObject)Instantiate(_powerItemPrefabs[0]);
			powerItemObj.transform.parent = _grid;
			powerItemObj.transform.localPosition = GetCenterPoint_Rect(firstCell);
		}

		BasePowerItem powerItem = powerItemObj.GetComponent<BasePowerItem>();
		powerItem.Init(type,cells);

		_powerItems.Add (powerItemObj);
	}

	public void CheckPowerItemForCompletion(Cell cell)
	{

		foreach (GameObject powerItemObj in _powerItems) {
			if(powerItemObj != null)
			{
				BasePowerItem powerItem = powerItemObj.GetComponent<BasePowerItem>();
				powerItem.CheckForItemCompletion(cell);
			}
		}

	}

	Vector3 GetCenterPoint_Rect(Cell cell)
	{
		return new Vector2( cell.CellPosition.x + 0.5f,cell.CellPosition.y - 0.5f );
	}
}
