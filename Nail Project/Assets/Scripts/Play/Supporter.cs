using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum JewelShape
{
	NONE,
	STRAIGHT,
	T_SHAPED,
	L_SHAPED,
}

public enum MotionDirection
{
	NONE,
	UP,
	DOWN,
	LEFT,
	RIGHT
}

public class Supporter : MonoBehaviour
{

    public static Supporter sp;

    public bool isNomove;

    public Vector2[] AvaiableMove;

    JewelObj[] AvaiableObj = new JewelObj[2];

    private float SP_DELAY = 5f;

    List<Vector2> vtmplist;

    JewelObj obj;

    void Awake()
    {
        sp = this;
    }

    void Update()
    {
        if (SP_DELAY > 0 && GameController.action.GameState == (int)Timer.GameState.PLAYING && !isNomove)
        {
            SP_DELAY -= Time.deltaTime;
        }
        else if (!isNomove && GameController.action.GameState == (int)Timer.GameState.PLAYING)
        {
            RefreshTime();
            isNoMoreMove();
            PlaySuggestionAnim();
        }
    }

    public bool isNoMoreMove()
    {
        StopSuggestionAnim();
        AvaiableMove = new Vector2[2];
        AvaiableObj = new JewelObj[2];

        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                if (JewelSpawner.spawn.JewelGribScript[x, y] != null && GribManager.cell.GribCellObj[x, y].cell.CellEffect == 0)
                {
                    obj = JewelSpawner.spawn.JewelGribScript[x, y];
                    JewelObj obj1 = MoveChecker(x, y, obj);
                    if (obj1 != null)
                    {
                        AvaiableMove[0] = obj.jewel.JewelPosition;
                        AvaiableObj[0] = JewelSpawner.spawn.JewelGribScript[(int)AvaiableMove[0].x, (int)AvaiableMove[0].y];
                        AvaiableMove[1] = obj1.jewel.JewelPosition;
                        AvaiableObj[1] = JewelSpawner.spawn.JewelGribScript[(int)AvaiableMove[1].x, (int)AvaiableMove[1].y];
                        isNomove = false;
                        return true;
                    }

                }
            }
        }
        isNomove = true;
        return false;
    }

    public void RefreshTime()
    {
        SP_DELAY = 5f;
    }

    JewelObj MoveChecker(int x, int y, JewelObj obj)
    {
        vtmplist = getListPos(x, y);
        foreach (Vector2 item in vtmplist)
        {
            if (JewelSpawner.spawn.JewelGribScript[(int)item.x, (int)item.y] != null && JewelSpawner.spawn.JewelGribScript[(int)item.x, (int)item.y].jewel.JewelType == 8)
                return JewelSpawner.spawn.JewelGribScript[(int)item.x, (int)item.y];
            else
            {
                List<JewelObj> NeiObj1 = Ulti.ListPlus(obj.GetCollumn(item, obj.jewel.JewelType, null),
                                                       obj.GetRow(item, obj.jewel.JewelType, null), obj);
                if (NeiObj1.Count >= 3)
                    return JewelSpawner.spawn.JewelGribScript[(int)item.x, (int)item.y];
            }
        }

        return null;
    }


    List<Vector2> getListPos(int x, int y)
    {
        vtmplist = new List<Vector2>();
        if (y + 1 < 9 && GribManager.cell.GribCellObj[x, y + 1] != null && GribManager.cell.GribCellObj[x, y + 1].cell.CellEffect == 0)
            vtmplist.Add(new Vector2(x, y + 1));
        if (y - 1 >= 0 && GribManager.cell.GribCellObj[x, y - 1] != null && GribManager.cell.GribCellObj[x, y - 1].cell.CellEffect == 0)
            vtmplist.Add(new Vector2(x, y - 1));
        if (x + 1 < 7 && GribManager.cell.GribCellObj[x + 1, y] != null && GribManager.cell.GribCellObj[x + 1, y].cell.CellEffect == 0)
            vtmplist.Add(new Vector2(x + 1, y));
        if (x - 1 >= 0 && GribManager.cell.GribCellObj[x - 1, y] != null && GribManager.cell.GribCellObj[x - 1, y].cell.CellEffect == 0)
            vtmplist.Add(new Vector2(x - 1, y));
        return vtmplist;
    }

    public void PlaySuggestionAnim()
    {
        if (AvaiableObj[0] != null && AvaiableObj[1] != null)
        {
            AvaiableObj[0].JewelSuggesttion();
            AvaiableObj[1].JewelSuggesttion();
        }
    }
    public void StopSuggestionAnim()
    {
        if (AvaiableObj[0] != null)
        {
            AvaiableObj[0].JewelStopSuggesttion();
        }
        if (AvaiableObj[1] != null)
        {
            AvaiableObj[1].JewelStopSuggesttion();
        }
    }

	//determine the shape of a list of jewels (T shape, + shape,etc...)
	public JewelShape GetJewelShape(List<JewelObj> list, GameObject obj)
	{
		JewelObj _intersectedJewel = obj.GetComponent<JewelObj>();

		List<Vector2> line1 = new List<Vector2> ();
		List<Vector2> line2 = new List<Vector2> ();

		foreach(JewelObj jewelObj in list)
		{
			if(_intersectedJewel.jewel.JewelPosition.x == jewelObj.jewel.JewelPosition.x)
			{
				line1.Add(jewelObj.jewel.JewelPosition);
			}

			if(_intersectedJewel.jewel.JewelPosition.y == jewelObj.jewel.JewelPosition.y)
			{
				line2.Add(jewelObj.jewel.JewelPosition);
			}
		}

		line1.Remove (_intersectedJewel.jewel.JewelPosition);
		line2.Remove (_intersectedJewel.jewel.JewelPosition);

		if(line1.Count == 0 || line2.Count == 0)
			return JewelShape.STRAIGHT;

		if(CheckForTShape(line1, line2,_intersectedJewel))
			return JewelShape.T_SHAPED;

		if(CheckForLShape(line1, line2,_intersectedJewel))
			return JewelShape.L_SHAPED;

		return JewelShape.NONE;
	}

	bool CheckForTShape(List<Vector2> line1,List<Vector2> line2,JewelObj intersectedJewel)
	{
		bool isAtCenter1 = CheckIfVectorAtCenter (line1, intersectedJewel.jewel.JewelPosition);
		bool isAtCenter2 = CheckIfVectorAtCenter (line2, intersectedJewel.jewel.JewelPosition);
		if ((!isAtCenter1 && isAtCenter2) || (isAtCenter1 && !isAtCenter2)) {
			return true;
		}
		return false;
	}

	bool CheckForLShape(List<Vector2> line1,List<Vector2> line2,JewelObj intersectedJewel)
	{
		bool isAtCenter1 = CheckIfVectorAtCenter (line1, intersectedJewel.jewel.JewelPosition);
		bool isAtCenter2 = CheckIfVectorAtCenter (line2, intersectedJewel.jewel.JewelPosition);
		if ((!isAtCenter1 && !isAtCenter2)) {
			return true;
		}
		return false;
	}


	bool CheckIfVectorAtCenter(List<Vector2> vectors,Vector2 compare)
	{
		if (vectors.Count == 0)
			return false;
		Vector2 lineStart = vectors[0];
		Vector2 lineEnd = vectors[vectors.Count - 1];
		Vector2 middlePoint = (lineStart + lineEnd) / 2;

		return middlePoint == compare;
	}

	public IEnumerator DestroyAdjacentJewel_Again (Vector2 jewelPos)
	{
		yield return new WaitForSeconds (GameController.DROP_DELAY + 1.5f);
		JewelObj obj = JewelSpawner.spawn.JewelGribScript[ (int)jewelPos.x,(int)jewelPos.y];
		DestroyAdjacentJewel (obj,false);
		if(obj!= null)
		obj.Destroy ();
		GameController.action.dropjewel ();
	}

	public void DestroyAdjacentJewel(JewelObj jewelObj,bool firstTime = true)
	{

		if (jewelObj == null || jewelObj.jewel == null) {
			print ("NULL");
			return;
		}

		Jewel centerJewel = jewelObj.jewel;
		Vector2 centerJewel_Pos = centerJewel.JewelPosition;

		Vector2[] directions = GetAdjacentVectors(centerJewel_Pos);
		
		foreach(Vector2 direction in directions)
		{
			if (CheckValidPosition(direction))
			{
				JewelObj obj = JewelSpawner.spawn.JewelGribScript[(int)direction.x,(int)direction.y];
				if(obj != null)
					obj.Destroy();
			}
		}

		if (firstTime)
			StartCoroutine (DestroyAdjacentJewel_Again(centerJewel_Pos));
	}

	//Get surrounding tiles
	Vector2[] GetAdjacentVectors(Vector2 centerPos)
	{
		return new Vector2[]
		{
			new Vector2( centerPos.x - 1 , centerPos.y + 1),
			new Vector2( centerPos.x     , centerPos.y + 1),
			new Vector2( centerPos.x + 1 , centerPos.y + 1),
			
			new Vector2( centerPos.x - 1   , centerPos.y ),
			new Vector2( centerPos.x + 1   , centerPos.y ),
			
			new Vector2( centerPos.x - 1 , centerPos.y - 1),
			new Vector2( centerPos.x     , centerPos.y - 1),
			new Vector2( centerPos.x + 1 , centerPos.y - 1),
		};
	}

	//Get tiles up,down,left,right
	Vector2[] Get4DirectionVector(Vector2 centerPos)
	{
		return new Vector2[]
		{
			new Vector2( centerPos.x     , centerPos.y + 1),
			
			new Vector2( centerPos.x - 1   , centerPos.y ),
			new Vector2( centerPos.x + 1   , centerPos.y ),
			
			new Vector2( centerPos.x     , centerPos.y - 1),
		};
	}

	//Figure out if the list of jewels is on horizontal or vertical axis
	public Direction GetJewelListDirection(List<JewelObj> list)
	{
		Vector2 startPoint = list [0].jewel.JewelPosition;
		Vector2 endPoint = list [ list.Count - 1].jewel.JewelPosition;

		if (startPoint.x == endPoint.x)
			return Direction.VERTICAL;
		if (startPoint.y == endPoint.y)
			return Direction.HORIZONTAL;

		return Direction.NONE;
	}

	public bool ComboProcess(GameObject obj)
	{
		JewelObj JewelObj = obj.GetComponent<JewelObj>();
		if (JewelObj != null)
			return ComboProcess (JewelObj.jewel);
		else
			return false;
	}

	public bool ComboProcess(Jewel jewel)
	{
		//If jewel has no power,no need to check for combo
		if (jewel.JewelPower == 0)
			return false;

		Vector2[] directions = Get4DirectionVector(jewel.JewelPosition);
		
		foreach(Vector2 direction in directions)
		{
			if (CheckValidPosition(direction))
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
			if (CheckValidPosition(direction))
			{
				EffectSpawner.effect.FireArrow(transform.position, false);
				GameController.action.PDestroyRow((int)direction.x, (int)direction.y);
				GameController.action.PDestroyCollumn((int)direction.x, (int)direction.y);
			}
		}

	}

	void ComboStripe_Color(int type)
	{
		for (int x = 0; x < 7; x++) {
			for (int y = 0; y < 9; y++) {
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
		for (int x = 0; x < 7; x++) {
			for (int y = 0; y < 9; y++) {
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
		Vector2[] directions = GetAdjacentVectors_x2(centerPos).ToArray();

		foreach (Vector2 direction in directions) {
			if (CheckValidPosition(direction))
			{
				JewelObj tmp = JewelSpawner.spawn.JewelGribScript [ (int)direction.x, (int)direction.y];
				if (tmp != null && tmp.jewel != null) {
					tmp.Destroy();
				}
			}
		}
		yield return new WaitForSeconds(0.5f);
		GameController.action.dropjewel ();
	}

	IEnumerator DestroyJewelType(int type)
	{
		for (int x = 0; x < 7; x++) {
			for (int y = 0; y < 9; y++) {
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

	public void SpawnJewelPower(int type, int power, Vector2 pos,bool playAppearAnim = false)
	{
		StartCoroutine ( SpawnJewelPower_Async(type,power,pos,playAppearAnim) );
	}

	IEnumerator SpawnJewelPower_Async(int type, int power, Vector2 pos,bool playAppearAnim = false)
	{
		yield return new WaitForSeconds(0.6f);
		GameObject tmp = JewelSpawner.spawn.SpawnJewelPower(type, power, pos);
		if(playAppearAnim)
		tmp.GetComponent<JewelObj> ().JewelEnable ();
	}

	IEnumerator ActivateStripes(float delay)
	{
		yield return new WaitForSeconds(delay);

		for (int x = 0; x < 7; x++) {
			for (int y = 0; y < 9; y++) {
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

	public bool CheckValidPosition(Vector2 pos)
	{
		int bound0 = 7;
		int bound1 = 9;

		if ((pos.x < bound0 && pos.y < bound1) && (pos.x >= 0 && pos.y >= 0)) {
			return true;
		} else
			return false;
	}

	List<Vector2> GetAdjacentVectors_x2(Vector2 pos)
	{
		List<Vector2> list = new List<Vector2> ();
		int columnBound = 5;
		int rowBound = 5;

		int columnStart = (int)pos.x - 2;
		int rowStart = (int)pos.y + 2;

		for (int row = rowStart  ; row > (rowStart - rowBound) - 1; row --) {
			for (int column = columnStart - 1; column < (columnStart + columnBound) - 1; column++) {
				list.Add( new Vector2( (float)column, (float)row) );
			}
		}
		return list;
	}

	public MotionDirection GetMotionDirection(Vector2 from , Vector2 to)
	{
		Vector3 vect = from - to;

		if( vect.y == 1)
			return MotionDirection.UP; 
		if( vect.y == -1)
			return MotionDirection.DOWN; 
		if( vect.x == -1)
			return MotionDirection.LEFT; 
		if( vect.x == 1)
			return MotionDirection.RIGHT; 

		return  MotionDirection.NONE; 
	}

	public void DestroyJewelBasedOnMotionDirection(Vector2 startpos,MotionDirection motionDirection)
	{
		StartCoroutine ( DestroyJewelBasedOnMotionDirection_Async(startpos,motionDirection) );
	}

	IEnumerator DestroyJewelBasedOnMotionDirection_Async(Vector2 startpos,MotionDirection motionDirection)
	{
		int boundCol = 9;
		int boundRow = 7;

		List<Vector2> dirs = new List<Vector2>();
		switch (motionDirection) {

		case MotionDirection.RIGHT:
			for(int i = (int)startpos.x ; i < boundRow; i ++)
			{
				dirs.Add( new Vector2( (float)i,startpos.y) );
			}
			break;

		case MotionDirection.LEFT:
			for(int i = (int)startpos.x ; i > -1 ; i --)
			{
				dirs.Add( new Vector2( (float)i,startpos.y) );
			}
			break;

		case MotionDirection.UP:
			for(int i = (int)startpos.y ; i < boundCol ; i ++)
			{
				dirs.Add( new Vector2( startpos.x,(float)i) );
			}
			break;

		case MotionDirection.DOWN:
			for(int i = (int)startpos.y ; i > -1 ; i --)
			{
				dirs.Add( new Vector2( startpos.x,(float)i) );
			}
			break;
		}

		for(int i = 1 ; i < dirs.Count ; i++)
		{
			JewelObj tmp = JewelSpawner.spawn.JewelGribScript [(int)dirs[i].x, (int)dirs[i].y];
			if (tmp != null)
			{
				if( i < 4 )
				{
					int random = Random.Range(6,8);
					StartCoroutine(SpawnJewelPower_Async(tmp.jewel.JewelType, random, tmp.jewel.JewelPosition,true));
				}
				else
				{
					tmp.Destroy();
				}
			}
		}
		yield return new WaitForSeconds(1f);
		GameController.action.dropjewel ();
	}

	public int LuckyCheck(Vector2 pos)
	{
		int x = (int)pos.x;
		int y = (int)pos.y;

		if (JewelSpawner.spawn.JewelGribScript [x + 1, y].jewel.JewelType == JewelSpawner.spawn.JewelGribScript [x + 2, y].jewel.JewelType) {
			return JewelSpawner.spawn.JewelGribScript [x + 1, y].jewel.JewelType;
		}

		if (JewelSpawner.spawn.JewelGribScript [x - 1, y].jewel.JewelType == JewelSpawner.spawn.JewelGribScript [x - 2, y].jewel.JewelType) {
			return JewelSpawner.spawn.JewelGribScript [x - 1, y].jewel.JewelType;
		}

		if (JewelSpawner.spawn.JewelGribScript [x, y + 1].jewel.JewelType == JewelSpawner.spawn.JewelGribScript [x, y + 2].jewel.JewelType) {
			return JewelSpawner.spawn.JewelGribScript [x, y + 1].jewel.JewelType;
		}

		if (JewelSpawner.spawn.JewelGribScript [x, y - 1].jewel.JewelType == JewelSpawner.spawn.JewelGribScript [x, y - 2].jewel.JewelType) {
			return JewelSpawner.spawn.JewelGribScript [x, y - 1].jewel.JewelType;
		}

		if (JewelSpawner.spawn.JewelGribScript [x + 1, y].jewel.JewelType == JewelSpawner.spawn.JewelGribScript [x -1, y].jewel.JewelType) {
			return JewelSpawner.spawn.JewelGribScript [x + 1, y].jewel.JewelType;
		}

		if (JewelSpawner.spawn.JewelGribScript [x, y + 1 ].jewel.JewelType == JewelSpawner.spawn.JewelGribScript [x, y -1].jewel.JewelType) {
			return JewelSpawner.spawn.JewelGribScript [x, y - 1].jewel.JewelType;
		}

		return 10;
	}

	public void LuckEffect( )
	{
		GameController.action._guestManager.GiveItemToFirstFoundGuest ();
	}

}
