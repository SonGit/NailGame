using UnityEngine;
using System.Collections;

public class ComboManager : MonoBehaviour {

	public static ComboManager Instance;

	void Awake()
	{
		Instance = this;
	}

	/// <summary>
	/// Check for occurances of combo
	/// </summary>
	/// <param name="jewel"> center jewel gameobject that needs checking </param>
	/// <returns></returns>
	public bool ComboProcess(GameObject obj)
	{
		JewelObj JewelObj = obj.GetComponent<JewelObj>();
		if (JewelObj != null)
			return ComboProcess (JewelObj.jewel);
		else
			return false;
	}

	/// <summary>
	/// Check for occurances of combo
	/// </summary>
	/// <param name="jewel"> center jewel that needs checking </param>
	/// <returns></returns>
	public bool ComboProcess(Jewel jewel)
	{
		//If jewel has no power,no need to check for combo
		if (jewel.JewelPower == 0)
			return false;
		
		Vector2[] directions = Supporter.sp.Get4DirectionVector(jewel.JewelPosition);
		
		foreach( Vector2 direction in directions )
		{
			if ( Supporter.sp.CheckOutOfBounds(direction) )
			{
				JewelObj nearbyJewelObj =  JewelSpawner.spawn.JewelGribScript[(int)direction.x,(int)direction.y];
				
				if(nearbyJewelObj != null)
				{
					Jewel nearbyJewel = JewelSpawner.spawn.JewelGribScript[(int)direction.x,(int)direction.y].jewel;
					
					//When two stripes meet
					if( (jewel.JewelPower == 6 && nearbyJewel.JewelPower == 6) ||  
					   (jewel.JewelPower == 6 && nearbyJewel.JewelPower == 7) ||
					   (jewel.JewelPower == 7 && nearbyJewel.JewelPower == 7) ||
					   (jewel.JewelPower == 7 && nearbyJewel.JewelPower == 6))
					{
						ComboStripe_Stripe(jewel.JewelPosition);
					}
					
					//When stripes + wrapped meet
					if( (jewel.JewelPower == 6  && nearbyJewel.JewelPower  == 5) ||  
					   (jewel.JewelPower  == 5  && nearbyJewel.JewelPower  == 6) ||
					   (jewel.JewelPower  == 7  && nearbyJewel.JewelPower  == 5) ||
					   (jewel.JewelPower  == 5  && nearbyJewel.JewelPower  == 7))
					{
						ComboStripe_Wrapped(jewel.JewelPosition);
					}
					
					//When stripes + color bomb meet
					if( (jewel.JewelPower == 6  && nearbyJewel.JewelPower  == 8) ||  
					   (jewel.JewelPower  == 8  && nearbyJewel.JewelPower  == 6) ||
					   (jewel.JewelPower  == 7  && nearbyJewel.JewelPower  == 8) ||
					   (jewel.JewelPower  == 8  && nearbyJewel.JewelPower  == 7))
					{
						ComboStripe_Color(jewel.JewelType);
					}
					
					//When wrapped + color bomb meet
					if( (jewel.JewelPower == 5  && nearbyJewel.JewelPower  == 8) ||  
					   (jewel.JewelPower  == 8  && nearbyJewel.JewelPower  == 5))
					{
						StartCoroutine(ComboWrapped_Color());
					}
					
					//When color bomb + color bomb meet
					if( (jewel.JewelPower == 8  && nearbyJewel.JewelPower  == 8) ||  
					   (jewel.JewelPower  == 8  && nearbyJewel.JewelPower  == 8))
					{
						StartCoroutine(ComboColor_Color());
					}
					
					//When wrapped + wrapped meet
					if( (jewel.JewelPower == 5  && nearbyJewel.JewelPower  == 5) ||  
					   (jewel.JewelPower  == 5  && nearbyJewel.JewelPower  == 5))
					{
						StartCoroutine(ComboWrapped_Wrapped(jewel.JewelPosition));
					}
					
				}
				
			}
		}
		return false;
	}

	void ComboStripe_Stripe(Vector2 pos)
	{
		EffectSpawner.effect.FireArrow(transform.position, false);
		GameController.action.PDestroyRow((int)pos.x, (int)pos.y);
		GameController.action.PDestroyCollumn((int)pos.x, (int)pos.y);
	}

	void ComboStripe_Wrapped(Vector2 pos)
	{
		Vector2[] directions = new Vector2[]
		{
			new Vector2( pos.x + 1, pos.y ),
			new Vector2( pos.x - 1, pos.y ),
			
			new Vector2( pos.x, pos.y ),
			
			new Vector2( pos.x, pos.y - 1),
			new Vector2( pos.x, pos.y + 1),
		};
		
		foreach (Vector2 direction in directions) {
			if ( Supporter.sp.CheckOutOfBounds(direction) )
			{
				EffectSpawner.effect.FireArrow(transform.position, false);
				GameController.action.PDestroyRow((int)direction.x, (int)direction.y);
				GameController.action.PDestroyCollumn((int)direction.x, (int)direction.y);
			}
		}
		
	}

	void ComboStripe_Color(int type)
	{
		for (int x = 0; x < GameController.WIDTH; x++) {
			for (int y = 0; y < GameController.HEIGHT; y++) {
				JewelObj tmp = JewelSpawner.spawn.JewelGribScript [x, y];
				if (tmp != null && tmp.jewel.JewelType == type) {
					
					int random = Random.Range(6,8);
					
					StartCoroutine(SpawnJewelPower_Async(type, random, tmp.jewel.JewelPosition));
				}
			}
		}
		StartCoroutine(ActivateStripes(3.5f));;
	}

	
	IEnumerator ComboWrapped_Color()
	{
		int rand = Random.Range (1,6);//range of jewels type
		StartCoroutine (DestroyJewelType(rand));
		yield return new WaitForSeconds(1f);
		rand = Random.Range (1,6);
		StartCoroutine (DestroyJewelType(rand));
	}

	IEnumerator ComboColor_Color()
	{
		for (int x = 0; x < GameController.WIDTH; x++) {
			for (int y = 0; y < GameController.HEIGHT; y++) {
				JewelObj tmp = JewelSpawner.spawn.JewelGribScript [x, y];
				if (tmp != null)
				{
					EffectSpawner.effect.Thunder(GribManager.cell.GribCell[(int)tmp.jewel.JewelPosition.x, (int)tmp.jewel.JewelPosition.y].transform.position);
					tmp.Destroy();
					yield return new WaitForSeconds(0.1f);
				}
			}
		}
		GameController.action.dropjewel ();
	}

	IEnumerator ComboWrapped_Wrapped(Vector2 centerPos)
	{
		Vector2[] directions = Supporter.sp.GetAdjacentVectors_x2 (centerPos);
		JewelObj centerPosJewelObj = JewelSpawner.spawn.JewelGribScript [(int)centerPos.x, (int)centerPos.y];
		
		foreach (Vector2 direction in directions) {
			if ( Supporter.sp.CheckOutOfBounds(direction) )
			{
				JewelObj tmp = JewelSpawner.spawn.JewelGribScript [ (int)direction.x, (int)direction.y];
				if (tmp != null && tmp.jewel != null) {
					tmp.Destroy();
				}
			}
		}
		Supporter.sp.Boom (centerPosJewelObj.transform.position);
		yield return new WaitForSeconds(3f);
		GameController.action.dropjewel ();
	}

	IEnumerator SpawnJewelPower_Async(int type, int power, Vector2 pos,bool playAppearAnim = false)
	{
		yield return new WaitForSeconds(0.6f);
		GameObject tmp = JewelSpawner.spawn.SpawnJewelPower(type, power, pos);
		if(playAppearAnim)
			tmp.GetComponent<JewelObj> ().JewelEnable ();
	}

	//Destroy all stripes
	IEnumerator ActivateStripes(float delay)
	{
		yield return new WaitForSeconds(delay);
		
		for (int x = 0; x < GameController.WIDTH; x++) {
			for (int y = 0; y < GameController.HEIGHT; y++) {
				JewelObj tmp = JewelSpawner.spawn.JewelGribScript [x, y];
				if (tmp != null)
				{
					if ( tmp.jewel.JewelPower == 6 || tmp.jewel.JewelPower == 7) {
						
						tmp.Destroy();
					}
				}
			}
		}
	}

	//Destroy a specific type of jewel
	IEnumerator DestroyJewelType(int type)
	{
		for (int x = 0; x < GameController.WIDTH; x++) {
			for (int y = 0; y < GameController.HEIGHT; y++) {
				JewelObj tmp = JewelSpawner.spawn.JewelGribScript [x, y];
				if (tmp != null && tmp.jewel.JewelType == type)
				{
					EffectSpawner.effect.Thunder(GribManager.cell.GribCell[(int)tmp.jewel.JewelPosition.x, (int)tmp.jewel.JewelPosition.y].transform.position);
					tmp.Destroy();
					yield return new WaitForSeconds(0.1f);
				}
			}
		}
		GameController.action.dropjewel ();
	}
}
