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

		for (int x = 0; x < GameController.WIDTH; x++)
        {
			for (int y = 0; y < GameController.HEIGHT; y++)
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
		if (y + 1 < GameController.HEIGHT && GribManager.cell.GribCellObj[x, y + 1] != null && GribManager.cell.GribCellObj[x, y + 1].cell.CellEffect == 0)
            vtmplist.Add(new Vector2(x, y + 1));
        if (y - 1 >= 0 && GribManager.cell.GribCellObj[x, y - 1] != null && GribManager.cell.GribCellObj[x, y - 1].cell.CellEffect == 0)
            vtmplist.Add(new Vector2(x, y - 1));
		if (x + 1 < GameController.WIDTH && GribManager.cell.GribCellObj[x + 1, y] != null && GribManager.cell.GribCellObj[x + 1, y].cell.CellEffect == 0)
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
			if (CheckOutOfBounds(direction))
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
	public Vector2[] Get4DirectionVector(Vector2 centerPos)
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

	public bool CheckOutOfBounds(Vector2 pos)
	{
		int bound0 = GameController.WIDTH;
		int bound1 = GameController.HEIGHT;

		if ((pos.x < bound0 && pos.y < bound1) && (pos.x >= 0 && pos.y >= 0)) {
			return true;
		} else
			return false;
	}

	public Vector2[] GetAdjacentVectors_x2(Vector2 pos)
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
		return list.ToArray ();
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

	public void ProcessComboEffect(int ComboCount)
	{
		if (ComboCount == 2)
			EffectSpawner.effect.SweetComboEffect();
		if (ComboCount == 3)
			EffectSpawner.effect.TastyComboEffect ();
		if (ComboCount > 3 && ComboCount <= 6)
			EffectSpawner.effect.DeliciousComboEffect ();
		if (ComboCount > 6)
			EffectSpawner.effect.DivineComboEffect ();
	}

	public void StartEndgamePhase()
	{
		Timer.timer.Win2 ();
		EffectSpawner.effect.EndgameEffect ();
		StartCoroutine (Endgame());
	}

	IEnumerator Endgame()
	{
		GameController.action.dropjewel ();
		yield return new WaitForSeconds(2f);

		int movesLeft = GameController.action.MoveLeft;
		int spawnPerMove = 2;
		int[] validPowers = new int[]
		{
			6,//STRIPED_VERTICAL
			7,//STRIPED_HORIZONTAL
			5,//WRAPPER
		};

		int availableSlots = NormalJewelCount (); //measure to prevent stack overflow.Happens when there is no more available jewel
			
		for (int i = 0; i < movesLeft; i ++) {
			
			for(int j = 0 ; j < spawnPerMove ; j++)
			{
				if( availableSlots > 0) //measure to prevent stack overflow
				{
					Vector2 location = GetRandomLocation();
					int rand = Random.Range(0,validPowers.Length);
					Supporter.sp.SpawnJewelPower (JewelSpawner.spawn.JewelGribScript [ (int)location.x, (int)location.y].jewel.JewelType , validPowers[rand] , location ,true);
					availableSlots --;
				}
			}
		
			yield return new WaitForSeconds(0.5f);
		}

		yield return new WaitForSeconds(2f);

		for (int x = 0; x < GameController.WIDTH; x++) {
			for (int y = 0; y < GameController.HEIGHT; y++) {
				JewelObj tmp = JewelSpawner.spawn.JewelGribScript [x, y];
				if (tmp != null && tmp.jewel != null)
				{
					if ( tmp.jewel.JewelPower != 0) {
						tmp.Destroy();
						yield return new WaitForSeconds(0.5f);
					}
				}
				
			}
		}

		yield return new WaitForSeconds(1.5f);
		Timer.timer.Win ();

	}

	Vector2 GetRandomLocation()
	{
		int randWidth = Random.Range (0, GameController.WIDTH);
		int randHeight = Random.Range (0, GameController.HEIGHT);
		
		JewelObj tmp = JewelSpawner.spawn.JewelGribScript [randWidth, randHeight];
		
		if (tmp != null) 
			if(tmp.jewel.JewelPower == 0)
				return new Vector2 (randWidth, randHeight);
		
		return GetRandomLocation();
	}

	int NormalJewelCount()
	{
		int count = 0;
		for (int x = 0; x < GameController.WIDTH; x++) {
			for (int y = 0; y < GameController.HEIGHT; y++) {
				JewelObj tmp = JewelSpawner.spawn.JewelGribScript [x, y];
				if (tmp != null && tmp.jewel != null)
				{
					if ( tmp.jewel.JewelPower == 0) {
						count ++;
					}
				}
			}
		}

		return count;
	}

	public void SpawnARandomStripe(int jewelType)
	{
		for (int x = 0; x < GameController.WIDTH; x++) {
			for (int y = 0; y < GameController.HEIGHT; y++) {
				JewelObj tmp = JewelSpawner.spawn.JewelGribScript [x, y];
				if (tmp != null && tmp.jewel != null && tmp.jewel.JewelType == jewelType && tmp.jewel.JewelPower == 0)
				{
					int rand = Random.Range(0,2);
					switch(rand)
					{
					case 0:
						Supporter.sp.SpawnJewelPower (jewelType, (int)GameController.Power.STRIPED_HORIZONTAL , tmp.jewel.JewelPosition ,true);
						break;
					case 1:
						Supporter.sp.SpawnJewelPower (jewelType, (int)GameController.Power.STRIPED_VERTICAL , tmp.jewel.JewelPosition ,true);
						break;
					default:
						Supporter.sp.SpawnJewelPower (jewelType, (int)GameController.Power.STRIPED_VERTICAL , tmp.jewel.JewelPosition ,true);
						break;
					}
					return;
				}
				
			}
		}
	}

	public void Boom(Vector3 groundZero)
	{
		for (int x = 0; x < GameController.WIDTH; x++) {
			for (int y = 0; y < GameController.HEIGHT; y++) {
				JewelObj tmp = JewelSpawner.spawn.JewelGribScript [x, y];
				if (tmp != null && tmp.jewel != null)
				{
					tmp.BoomEffect(groundZero);
				}
				
			}
		}
	}

}
