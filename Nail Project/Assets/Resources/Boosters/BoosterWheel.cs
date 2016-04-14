using UnityEngine;
using System.Collections;

public class BoosterWheel : BoosterBase {
	public override void OnClick ()
	{
		Vector2 loc = GetRandomLocation (2,2);
		Supporter.sp.SpawnJewelPower (Random.Range(1,6), (int)GameController.Power.WRAPPER , loc ,true);
	}
}
