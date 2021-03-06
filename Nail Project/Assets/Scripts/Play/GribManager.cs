﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GribManager : MonoBehaviour
{

    public static GribManager cell ;

    public GameObject[,] GribCell;

    public CellObj[,] GribCellObj;

    public GameObject GribParent;

    public GameObject CellPrefab;

    public Sprite[] CellSprite;

	public Sprite[] CellEffectSprite;

	public Sprite[] UnlockedItemSprite;

    public GameObject[] border;

    public GameObject[] corner;

    public GameObject BorderParent;

    public int[,] Map;

	public List<Vector2> itemCellPos;

    private GameObject ObjTmp;

    private CellObj cellscript;


    private const string path = "Assets/Resources/Maps/";

    void Awake()
    {
        cell = this;
    }

    /// <summary>
    /// Create Grid map
    /// </summary>
    /// <param name="MapName">name of map</param>
    /// <returns></returns>
    public IEnumerator GribMapCreate(string MapName)
    {
        GribCell = new GameObject[GameController.WIDTH, GameController.HEIGHT];
        Map = MapReader(MapName);
        yield return new WaitForEndOfFrame();
        GribCreate(Map);
        yield return new WaitForEndOfFrame();
        BorderCreate(Map);
        yield return new WaitForEndOfFrame();
        EffectCrash(Map);
        yield return new WaitForSeconds(1);
        JewelSpawner.spawn.JewelMapCreate(Map);
		CellProcess ();
		yield return new WaitForSeconds(0.5f);
		yield return new WaitForEndOfFrame();
        JewelSpawner.spawn.EnableAllJewel();
	
    }

    void GribCreate(int[,] map)
    {
        GameController.action.CellNotEmpty = 0;
		GribCellObj = new CellObj[GameController.WIDTH, GameController.HEIGHT];
		for (int x = 0; x < GameController.WIDTH; x++)
        {
			for (int y = 0 ; y< GameController.HEIGHT; y++)
            {
                if (map[x, y] > 1)
                    GameController.action.CellNotEmpty++;
                if (map[x, y] > 0)
                    CellInstantiate(x, y, map[x, y]);
                EffectSpawner.effect.JewelCrashArray[x, y] = EffectSpawner.effect.JewelCash(new Vector3(x,y));
                
            }
        }
    }

    void EffectCrash(int[,] map)
    {
		for (int x = 0; x < GameController.WIDTH; x++)
        {
			for (int y = 0; y < GameController.HEIGHT; y++)
            {
                if (map[x, y] > 0)
                EffectSpawner.effect.JewelCrashArray[x, y] = EffectSpawner.effect.JewelCash(new Vector3(x, y));
            }
        }
    }

    void CellInstantiate(int x, int y, int type)
    {

        ObjTmp = (GameObject)Instantiate(CellPrefab);
        ObjTmp.transform.SetParent(GribParent.transform, false);
        ObjTmp.transform.localPosition = new Vector3(x, y);
        cellscript = ObjTmp.GetComponent<CellObj>();
        cellscript.CellCode = type;
        cellscript.cell = SetCell(type, x, y);
        cellscript.SetSprite(cellscript.cell.CellType-1);
        GribCell[x, y] = ObjTmp;
        GribCellObj[x, y] = cellscript;

    }

    int[,] MapReader(string mapName)
    {
		int[,] tmp = new int[GameController.WIDTH, GameController.HEIGHT];
        string mapStringdata = "";
        //read string from text file
#if UNITY_EDITOR
        mapStringdata = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(@path + mapName + ".txt").ToString();
#else
	    TextAsset txtass = (TextAsset)Resources.Load ("Maps/" + mapName, typeof(TextAsset));
	    mapStringdata = txtass.ToString ();
#endif
        string[] stringresult = mapStringdata.Split(new char[] { '	', '\n'});
        int dem = 0;
		for (int y = GameController.HEIGHT - 1; y >= 0; y--)
			for (int x = 0; x < GameController.WIDTH; x++)
            {
                tmp[x, y] = int.Parse(stringresult[dem]);
                dem++;
            }

		SetBooster(stringresult);

        return tmp;

    }

	void SetBooster(string[] stringresult)
	{
		int dem = 64;

		if (dem > stringresult.Length)
			return;

		List<int> boosters = new List<int>();

		while (dem < stringresult.Length) {
			boosters.Add( int.Parse(stringresult[dem]) ) ;
			dem ++;
		}

		BoosterManager.Instance.SetBoosters (boosters);
	}

    Cell SetCell(int type,int x,int y)
    {
        Cell script = new Cell();

		if (type > 50) //Special cells
		{
			script.CellType = 2;
			script.CellEffect = 0;
		}

        if (type > 30 && type < 50)
        {
            script.CellType = type / 10;
            script.CellEffect = type % 10;
        }

		if (type < 30)
        {
            script.CellType = type;
            script.CellEffect = 0;
        }
        script.CellPosition = new Vector2(x, y);
        return script;
    }

	/// <summary>
	/// Detect items in the given grid
	/// </summary>
	void CellProcess()
	{
		itemCellPos = new List<Vector2>();
		// Get all bounds before looping.
		int bound0 = Map.GetUpperBound(0) + 1;
		int bound1 = Map.GetUpperBound(1) + 1;

		for(int i = 0 ; i < bound0 ; i ++)
		{
			for(int j = 0 ; j < bound1 ; j ++)
			{
				if(Map[i,j] >= 11 && Map[i,j] <= 20)//only item cells type stay in this range
				{

					if(Map[i,j] == 11) //Rectangular
					{
						Cell[] cells = new Cell[]
						{
							GribCellObj[i,j].cell,
							GribCellObj[i+1,j].cell,
							GribCellObj[i,j-1].cell,
							GribCellObj[i+1,j-1].cell,
						};
						PowerItemManager.Instance.CreatePowerItem(cells,ItemType.ICE_CREAM);
					}
				}

				if(Map[i,j] >= 50)//special jewels that exist before game start
				{
					if(Map[i,j] == 51) //Wheel
						Supporter.sp.SpawnJewelPower (8, (int)GameController.Power.WHEEL , new Vector2(i,j),true);
					if(Map[i,j] == 52) //Lucky
						Supporter.sp.SpawnJewelPower (10, (int)GameController.Power.LUCKY , new Vector2(i,j),true);
				}
			}
		}
	}



    #region boder create
    void BorderCreate(int[,] map)
    {
        for (int x = 0; x < GameController.WIDTH; x++)
        {
			for (int y = 0; y < GameController.HEIGHT; y++)
            {
                int i = map[x, y];
                if (i >0)
                {

                    borderins(GribCell[x, y], left(x, y), right(x, y), top(x, y), bot(x, y));
                    CornerOutChecker(GribCell[x, y], topleft(x, y), topright(x, y), botleft(x, y), botright(x, y),x,y);
                } else
                {
                    boderInChecker(map, x, y);
                }
            }
        }
    }

    bool left(int x, int y)
    {
        if (x == 0)
            return true;
        else if (x - 1 >= 0 && Map[x-1, y] == 0)
            return true;
            
        return false;
    }

    bool right(int x, int y)
    {
        if (x == 6)
            return true;
        else if (x + 1 <= 6 && Map[x+1, y] == 0)
            return true;

        return false;
    }

    bool bot(int x, int y)
    {
            if (y == 0)
                return true;
            else if (x < 7 && y - 1 >= 0 && Map[x, y - 1] == 0)
                return true;

        return false;
    }

    bool top(int x, int y)
    {
        if (y == GameController.HEIGHT - 1)
            return true;
		else if (y + 1 <= GameController.HEIGHT - 1 && Map[x, y+1] == 0)
            return true;

        return false;
    }

    bool topleft(int x, int y)
    {
		if (x - 1 < 0 || y + 1 > GameController.HEIGHT - 1)
            return true;
        else if (x - 1 >= 0 && y + 1 <= 8 &&  Map[x - 1, y + 1] == 0)
            return true;

        return false;
    }

    bool topright(int x, int y)
    {

		if (x + 1 > GameController.WIDTH - 1 || y + 1 > GameController.HEIGHT - 1)
                return true;
		else if (x + 1 <= GameController.WIDTH - 1 && y + 1 <= GameController.HEIGHT - 1 && Map[x + 1, y + 1] == 0)
                return true;

        return false;
    }

    bool botleft(int x, int y)
    {
        if (x - 1 < 0 || y - 1 < 0)
            return true;
        else if (x - 1 >= 0 && y - 1 >= 0 && Map[x - 1, y - 1] == 0)
            return true;

        return false;
    }

    bool botright(int x, int y)
    {
		if (x + 1 > GameController.WIDTH - 1  || y - 1 < 0)
            return true;
		else if (x + 1 <=GameController.WIDTH - 1  && y - 1 >= 0 && Map[x + 1, y - 1] == 0)
            return true;

        return false;
    }

    void borderins(GameObject parent,bool left,bool right,bool top, bool bot)
    {
       // Debug.Log(parent.GetComponent<CellObj>().cell.CellPosition);

        if (left)
        {

                ObjTmp = (GameObject)Instantiate(border[2]);
                ObjTmp.transform.SetParent(BorderParent.transform, false);
                ObjTmp.transform.localPosition += parent.transform.localPosition;
             //   boderInChecker(parent);
        }

                    
        if (right)
        {

                ObjTmp = (GameObject)Instantiate(border[3]);
                ObjTmp.transform.SetParent(BorderParent.transform, false);
                ObjTmp.transform.localPosition += parent.transform.localPosition;
                //boderInChecker(parent);
        }
        if (top)
        {

                ObjTmp = (GameObject)Instantiate(border[1]);
                ObjTmp.transform.SetParent(BorderParent.transform, false);
                ObjTmp.transform.localPosition += parent.transform.localPosition;
        }
        if (bot)
        {

                ObjTmp = (GameObject)Instantiate(border[0]);
                ObjTmp.transform.SetParent(BorderParent.transform, false);
                ObjTmp.transform.localPosition += parent.transform.localPosition;
        }

       
    }


    void CornerOutChecker(GameObject parent, bool topleft, bool topright, bool botleft, bool botright,int x, int y)
    {
        bool _top = top(x,y);
        bool _bot = bot(x,y);
        bool _left = left(x,y);
        bool _right = right(x,y);

        if (topleft &&  _top && _left)
        {
            ObjTmp = (GameObject)Instantiate(corner[0]);
            ObjTmp.transform.SetParent(BorderParent.transform, false);
            ObjTmp.transform.localPosition += parent.transform.localPosition;
        }
        if (topright && _top && _right)
        {
            ObjTmp = (GameObject)Instantiate(corner[1]);
            ObjTmp.transform.SetParent(BorderParent.transform, false);
            ObjTmp.transform.localPosition += parent.transform.localPosition;
        }
        if (botleft && _bot && _left)
        {
            ObjTmp = (GameObject)Instantiate(corner[2]);
            ObjTmp.transform.SetParent(BorderParent.transform, false);
            ObjTmp.transform.localPosition += parent.transform.localPosition;
        }
        if (botright && _bot && _right)
        {
            ObjTmp = (GameObject)Instantiate(corner[3]);
            ObjTmp.transform.SetParent(BorderParent.transform, false);
            ObjTmp.transform.localPosition += parent.transform.localPosition;
        }
     
    }

    void boderInChecker(int[,] map ,int x,int y)
    {
        if (x - 1 >= 0 && y - 1 >= 0 && map[ x - 1, y] > 0  && map[x ,y - 1] > 0 )
        {

            ObjTmp = (GameObject)Instantiate(corner[6]);
            ObjTmp.transform.SetParent(BorderParent.transform, false);
            ObjTmp.transform.localPosition += new Vector3(x-1, y-1);
        }
		if (x - 1 >= 0 && y + 1 < GameController.HEIGHT && map[x - 1, y] > 0 && map[x, y + 1] > 0)
        {

            ObjTmp = (GameObject)Instantiate(corner[4]);
            ObjTmp.transform.SetParent(BorderParent.transform, false);
            ObjTmp.transform.localPosition += new Vector3(x - 1 , y);
        }
		if (x + 1 < GameController.HEIGHT && y - 1 >= 0 && map[x + 1, y] > 0 && map[x, y - 1] > 0)
        {

            ObjTmp = (GameObject)Instantiate(corner[7]);
            ObjTmp.transform.SetParent(BorderParent.transform, false);
            ObjTmp.transform.localPosition += new Vector3(x, y - 1);
        }

       // if (x + 1 < 7 && y + 1 < 9 && map[x + 1, y] > 0 && map[x, y + 1] > 0)
		if (x + 1 < 7 && y + 1 < 7 && map[x + 1, y] > 0 && map[x, y + 1] > 0)
        {

            ObjTmp = (GameObject)Instantiate(corner[5]);
            ObjTmp.transform.SetParent(BorderParent.transform, false);
            ObjTmp.transform.localPosition += new Vector3(x, y);
        }

        
    }

    bool CornerOutCheckTop(GameObject parent)
    {
        CellObj obj = parent.GetComponent<CellObj>();
        int x =(int) obj.cell.CellPosition.x;
        int y = (int)obj.cell.CellPosition.y;
        for (int i = y+1; i < 9; i++)
        {
            if (GribCellObj[x, i] != null)
                return false;
        }
        return true;
    }
    bool CornerOutCheckBot(GameObject parent)
    {
        CellObj obj = parent.GetComponent<CellObj>();
        int x = (int)obj.cell.CellPosition.x;
        int y = (int)obj.cell.CellPosition.y;
        for (int i = y - 1 ; i >=0; i--)
        {
            if (GribCellObj[x, i] != null)
                return false;
        }
        return true;
    }
    bool CornerOutCheckRight(GameObject parent)
    {
        CellObj obj = parent.GetComponent<CellObj>();
        int x = (int)obj.cell.CellPosition.x;
        int y = (int)obj.cell.CellPosition.y;
        for (int i = x + 1; i < 7; i++)
        {
            if (GribCellObj[i, y] != null)
                return false;
        }
        return true;
    }
    bool CornerOutCheckLeft(GameObject parent)
    {
        CellObj obj = parent.GetComponent<CellObj>();
        int x = (int)obj.cell.CellPosition.x;
        int y = (int)obj.cell.CellPosition.y;
        for (int i = x-1; i >= 0; i--)
        {
            if (GribCellObj[i, y] != null)
                return false;
        }
        return true;
    }

#endregion 
}
