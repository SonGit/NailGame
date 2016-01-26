using UnityEngine;
using System.Collections;

public class BoosterWheel : BoosterBase {
	public override void OnClick ()
	{
		Vector2 loc = GetRandomLocation (2,2);
		Supporter.sp.SpawnJewelPower (8, (int)GameController.Power.WHEEL , loc ,true);
	}
}
