using UnityEngine;
using System.Collections;

public class BoosterCombo : BoosterBase {
	
	public override void OnClick ()
	{
		Vector2 loc = GetRandomLocation (2,2);
		Supporter.sp.SpawnJewelPower ( Random.Range(0,6), (int)GameController.Power.STRIPED_HORIZONTAL , loc ,true);
		
		Vector2[] neighboringLocations = Supporter.sp.Get4DirectionVector (loc);
		
		foreach(Vector2 vect in neighboringLocations)
		{
			if(Supporter.sp.CheckOutOfBounds(vect))
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

}
