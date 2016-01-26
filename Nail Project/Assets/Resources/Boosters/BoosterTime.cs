using UnityEngine;
using System.Collections;

public class BoosterTime : BoosterBase {

	public override void OnClick ()
	{
		GameController.action.PBonusTime ();
	}
}
