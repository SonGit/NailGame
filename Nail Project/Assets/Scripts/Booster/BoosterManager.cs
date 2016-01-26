using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum BoosterType
{
	MORE_MOVES,
	TIME,
	COMBO,
	WHEEL,
	COLOUR_BOMB,
}

public class BoosterManager : MonoBehaviour {

	public static BoosterManager Instance ;

	public GameObject boosterBtnPrefab;

	public RectTransform grid;

	void Awake()
	{
		Instance = this;
	}

	public void SetBoosters(List<int> boosterTypes)
	{
		foreach(int type in boosterTypes)
		{
			CreateBoosterButtons(type);
		}
	}

	void CreateBoosterButtons(int boosterType)
	{
		GameObject btnObj = (GameObject) Instantiate( boosterBtnPrefab );
		btnObj.transform.parent = grid;
		btnObj.transform.localScale = Vector3.one;

		Button btn = btnObj.GetComponent<Button>();
		Text label = btn.GetComponentInChildren<Text>();

		switch(boosterType)
		{

		case 0:
			btnObj.AddComponent<BoosterMoreMove>();
			label.text = "MORE MOVE";
			break;

		case 1:
			btnObj.AddComponent<BoosterTime>();
			label.text = "MORE TIME";
			break;

		case 2:
			btnObj.AddComponent<BoosterCombo>();
			label.text = "COMBO";
			break;

		case 3:
			btnObj.AddComponent<BoosterWheel>();
			label.text = "WHEEL";
			break;

		case 4:
			btnObj.AddComponent<BoosterBomb>();
			label.text = "BOMB";
			break;
		}

	
		btn.onClick.AddListener(() => {
			 btnObj.GetComponent<BoosterBase>().OnClick();
		});
	}
	

	public void TimeBooster()
	{
		GameController.action.PBonusTime ();
	}

	public void ColourBombBooster()
	{
		Vector2 loc = GetRandomLocation (2,2);
		Supporter.sp.SpawnJewelPower (8, (int)GameController.Power.MAGIC , loc ,true);
	}

	public void Wrapped_StripedBooster()
	{
		Vector2 loc = GetRandomLocation (2,2);
		Supporter.sp.SpawnJewelPower ( Random.Range(0,6), (int)GameController.Power.WRAPPER , loc ,true);

		Vector2[] neighboringLocations = Supporter.sp.Get4DirectionVector (loc);

		foreach(Vector2 vect in neighboringLocations)
		{
			if(Supporter.sp.CheckValidPosition(vect))
			{
				JewelObj tmp = JewelSpawner.spawn.JewelGribScript [ (int)vect.x, (int)vect.y];
				if (tmp != null) 
				{
					Supporter.sp.SpawnJewelPower (tmp.jewel.JewelType, (int)GameController.Power.STRIPED_HORIZONTAL , vect ,true);
					return;
				}
					
			}
		}
	}

	public void MoreMoveBooster()
	{
		GameController.action.MoveLeft ++;
	}
	
	public void CoconutBooster()
	{
		Vector2 loc = GetRandomLocation (2,2);
		Supporter.sp.SpawnJewelPower (8, (int)GameController.Power.WHEEL , loc ,true);
	}

	Vector2 GetRandomLocation(int offset_x , int offset_y)
	{
		int randWidth = Random.Range (offset_x, GameController.WIDTH);
		int randHeight = Random.Range (offset_y, GameController.HEIGHT);

		JewelObj tmp = JewelSpawner.spawn.JewelGribScript [randWidth, randHeight];

		if (tmp != null) 
			if(tmp.jewel.JewelPower == 0)
				return new Vector2 (randWidth, randHeight);

		return GetRandomLocation(offset_x,offset_y);
	}

}
