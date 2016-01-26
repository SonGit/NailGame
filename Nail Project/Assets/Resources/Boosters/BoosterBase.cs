using UnityEngine;
using System.Collections;

public abstract class BoosterBase : MonoBehaviour {

	public BoosterType type;

	public abstract void OnClick ();

	public Vector2 GetRandomLocation(int offset_x , int offset_y)
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
