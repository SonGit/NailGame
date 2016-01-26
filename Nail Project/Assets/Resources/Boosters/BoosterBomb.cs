using UnityEngine;
using System.Collections;

public class BoosterBomb : BoosterBase  {

	public override void OnClick ()
	{
		Vector2 loc = GetRandomLocation (2,2);
		Supporter.sp.SpawnJewelPower (8, (int)GameController.Power.MAGIC , loc ,true);
	}

}
