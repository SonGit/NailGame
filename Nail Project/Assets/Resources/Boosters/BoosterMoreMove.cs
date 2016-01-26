using UnityEngine;
using System.Collections;

public class BoosterMoreMove : BoosterBase {

	public override void OnClick ()
	{
		GameController.action.MoveLeft ++;
	}
}
