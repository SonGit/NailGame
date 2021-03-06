﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Direction
{
	NONE,
	VERTICAL,
	HORIZONTAL
}

public class JewelObj : MonoBehaviour
{

    public Jewel jewel;

	public SkeletonAnimation render;

    private const float DELAY = 0.2f;

    public bool Checked;

    public bool isMove;

	public Direction direction; //completely optional,only for strpied candy effect

	IEnumerator Start()
	{
		render =  transform.GetChild(0).GetComponent<SkeletonAnimation>();
		yield return null;
	}

	public void SetSkin(int type,int power = -1)
	{
		render = transform.GetChild (0).GetComponent<SkeletonAnimation> ();
		render.enabled = true;

		string skinName = "item" + (type + 1).ToString ();

		if (power != -1)
			skinName = SetSkin_Power (power, skinName);

		if (render.skeleton.data.FindSkin (skinName) != null) {
			render.skeleton.SetSkin (skinName);
		} else {
			render.skeleton.SetSkin ("item8"); //if invalid skin name, fall to this skin
		}
	
		render.gameObject.transform.localScale = Vector3.one;
		render.enabled = false;
	}

	string SetSkin_Power(int power,string skinName)
	{
		if (power == (int)GameController.Power.STRIPED_VERTICAL)
			return skinName + "v";
		if (power == (int)GameController.Power.STRIPED_HORIZONTAL)
			return skinName + "h";
		if (power == (int)GameController.Power.WRAPPER)
			return skinName + "s";
		if (power == (int)GameController.Power.MAGIC)
			return "item7";
		
		return skinName;
	}

    //delete jewel
    public void Destroy()
    {
		int score = 1;
		Guest guest = GameController.action._guestManager.FillGuest (jewel.JewelType,score);

		if(guest != null)
			EffectSpawner.effect.MiniStar (transform.position,guest.transform.position + new Vector3(0,1,0));

        RemoveFromList((int)jewel.JewelPosition.x, (int)jewel.JewelPosition.y);
        StartCoroutine(_Destroy());
    }

    /// <summary>
    /// power of jewel
    /// </summary>
    /// <param name="power"></param>
    void PowerProcess(int power)
    {
        switch (power)
        {
            case 1:
                GameController.action.PBoom((int)jewel.JewelPosition.x, (int)jewel.JewelPosition.y);
                EffectSpawner.effect.boom(this.gameObject.transform.position);
                break;

            case 2:
                EffectSpawner.effect.FireArrow(transform.position, false);
                GameController.action.PDestroyRow((int)jewel.JewelPosition.x, (int)jewel.JewelPosition.y);
                break;

            case 3:
                EffectSpawner.effect.FireArrow(transform.position, true);
                GameController.action.PDestroyCollumn((int)jewel.JewelPosition.x, (int)jewel.JewelPosition.y);
                break;

            case 4:
                GameController.action.PBonusTime();
				break;

			case 5: //WRAPPER
				EffectSpawner.effect.WrapperEffect(transform.position,jewel.JewelType);
				Supporter.sp.DestroyAdjacentJewel(this);
                break;

			case 6: //STRIPED_VERTICAL
				EffectSpawner.effect.StripedEffect(transform.position,jewel.JewelType,true);
				//EffectSpawner.effect.FireArrow(transform.position, false);
				GameController.action.PDestroyCollumn((int)jewel.JewelPosition.x, (int)jewel.JewelPosition.y);
				break;

			case 7: //STRIPED_HORIZONTAL
				EffectSpawner.effect.StripedEffect(transform.position,jewel.JewelType);
				GameController.action.PDestroyRow((int)jewel.JewelPosition.x, (int)jewel.JewelPosition.y);
				break;

			case 98://Play 2 stripe effect and destroy columns accordingly.Happen only in stripe+stripe combo
				EffectSpawner.effect.StripedEffect(transform.position,jewel.JewelType);
				EffectSpawner.effect.StripedEffect(transform.position,jewel.JewelType,true);
				GameController.action.PDestroyCollumn((int)jewel.JewelPosition.x, (int)jewel.JewelPosition.y);
				GameController.action.PDestroyRow((int)jewel.JewelPosition.x, (int)jewel.JewelPosition.y);
				break;
        }

		EffectSpawner.effect.ComBoInc();
    }

    //move jewel and destroy
    public void ReGroup(Vector2 pos)
    {

        StartCoroutine(_ReGroup(pos));
    }

    IEnumerator _ReGroup(Vector2 pos)
    {
        RemoveFromList((int)jewel.JewelPosition.x, (int)jewel.JewelPosition.y);
        yield return new WaitForSeconds(DELAY - 0.015f);
        Ulti.MoveTo(this.gameObject, pos, DELAY);

        StartCoroutine(_Destroy());
    }
    IEnumerator _Destroy()
    {
        GribManager.cell.GribCellObj[(int)jewel.JewelPosition.x, (int)jewel.JewelPosition.y].CelltypeProcess();
        GameController.action.CellRemoveEffect((int)jewel.JewelPosition.x, (int)jewel.JewelPosition.y);

        yield return new WaitForSeconds(DELAY);
        if (jewel.JewelPower > 0)
        {
            PowerProcess(jewel.JewelPower);
        }
        GameController.action.drop.DELAY = GameController.DROP_DELAY;
        JewelCrash();
        yield return new WaitForEndOfFrame();
        EffectSpawner.effect.ScoreInc(this.gameObject.transform.position);
        yield return new WaitForEndOfFrame();
        EffectSpawner.effect.ContinueCombo();
        yield return new WaitForEndOfFrame();
        Supporter.sp.RefreshTime();
        StopAllCoroutines();        
        Destroy(gameObject);
    }
    void JewelCrash()
    {
        int x = (int)jewel.JewelPosition.x;
        int y = (int)jewel.JewelPosition.y;

        EffectSpawner.effect.JewelCrashArray[x, y].transform.position = new Vector3(transform.position.x, transform.position.y, -0.2f);
        EffectSpawner.effect.JewelCrashArray[x, y].SetActive(false);
        EffectSpawner.effect.JewelCrashArray[x, y].SetActive(true);
    }

    public void getNewPosition()
    {
        int newpos = (int)jewel.JewelPosition.y;
        int x = (int)jewel.JewelPosition.x;
        int oldpos = (int)jewel.JewelPosition.y;

        for (int y = newpos - 1; y >= 0; y--)
        {
            if (GribManager.cell.Map[x, y] != 0 && GribManager.cell.GribCellObj[x, y].cell.CellEffect != 4 && JewelSpawner.spawn.JewelGribScript[x, y] == null)
                newpos = y;
            else if (GribManager.cell.Map[x, y] != 0 && GribManager.cell.GribCellObj[x, y].cell.CellEffect == 4)
            {
                break;
            }
        }
        JewelSpawner.spawn.JewelGribScript[x, (int)jewel.JewelPosition.y] = null;
        JewelSpawner.spawn.JewelGrib[x, (int)jewel.JewelPosition.y] = null;

        jewel.JewelPosition = new Vector2(x, newpos);
        JewelSpawner.spawn.JewelGribScript[x, newpos] = this;
        JewelSpawner.spawn.JewelGrib[x, newpos] = this.gameObject;

        if (oldpos != newpos)
            StartCoroutine(Ulti.IEDrop(this.gameObject, jewel.JewelPosition, GameController.DROP_SPEED));
    }


    public List<JewelObj> GetRow(Vector2 Pos, int type, JewelObj bonus)
    {
        List<JewelObj> tmp1 = GetLeft(Pos, type);
        List<JewelObj> tmp2 = GetRight(Pos, type);
        if (tmp1.Count + tmp2.Count > 1)
        {
            return Ulti.ListPlus(tmp1, tmp2, bonus);
        }

        else
            return new List<JewelObj>();
    }

    public List<JewelObj> GetCollumn(Vector2 Pos, int type, JewelObj bonus)
    {
        List<JewelObj> tmp1 = GetTop(Pos, type);
        List<JewelObj> tmp2 = GetBot(Pos, type);
        if (tmp1.Count + tmp2.Count > 1)
        {
            return Ulti.ListPlus(tmp1, tmp2, bonus);
        }
        else
            return new List<JewelObj>();
    }

    public void SetBackAnimation(GameObject Obj)
    {
        if (!Supporter.sp.isNomove)
        {
            Vector2 ObjPos = Obj.GetComponent<JewelObj>().jewel.JewelPosition;
            Animation anim = transform.GetChild(0).GetComponent<Animation>();
            anim.enabled = true;

            if (ObjPos.x == jewel.JewelPosition.x)
            {
                if (ObjPos.y > jewel.JewelPosition.y)
                {
					anim.Blend("MoveBack_Up", 1, 0);
                    //anim.Play("MoveBack_Up");
                }
                else
                {
					anim.Blend("MoveBack_Down", 1, 0);
                   // anim.Play("MoveBack_Down");
                }
            }
            else
            {
                if (ObjPos.x > jewel.JewelPosition.x)
                {
					anim.Blend("MoveBack_Right", 1, 0);
                    //anim.Play("MoveBack_Right");
                }
                else
                {
					anim.Blend("MoveBack_Left", 1, 0);
                   // anim.Play("MoveBack_Left");
                }
            }
        }
    }

    List<JewelObj> GetLeft(Vector2 Pos, int type)
    {
        List<JewelObj> tmp = new List<JewelObj>();
        for (int x = (int)Pos.x - 1; x >= 0; x--)
        {

            if (x != jewel.JewelPosition.x && JewelSpawner.spawn.JewelGribScript[x, (int)Pos.y] != null && JewelSpawner.spawn.JewelGribScript[x, (int)Pos.y].jewel.JewelType == type && GribManager.cell.GribCellObj[x, (int)Pos.y].cell.CellEffect == 0)
                tmp.Add(JewelSpawner.spawn.JewelGribScript[x, (int)Pos.y]);
            else
                return tmp;
        }
        return tmp;
    }
    List<JewelObj> GetRight(Vector2 Pos, int type)
    {
        List<JewelObj> tmp = new List<JewelObj>();
		for (int x = (int)Pos.x + 1; x < GameController.WIDTH; x++)
        {
            if (x != jewel.JewelPosition.x && JewelSpawner.spawn.JewelGribScript[x, (int)Pos.y] != null && JewelSpawner.spawn.JewelGribScript[x, (int)Pos.y].jewel.JewelType == type && GribManager.cell.GribCellObj[x, (int)Pos.y].cell.CellEffect == 0)
                tmp.Add(JewelSpawner.spawn.JewelGribScript[x, (int)Pos.y]);
            else
                return tmp;
        }
        return tmp;
    }

    List<JewelObj> GetTop(Vector2 Pos, int type)
    {
        List<JewelObj> tmp = new List<JewelObj>();
        for (int y = (int)Pos.y + 1; y < GameController.HEIGHT; y++)
        {
            if (y != jewel.JewelPosition.y && JewelSpawner.spawn.JewelGribScript[(int)Pos.x, y] != null && JewelSpawner.spawn.JewelGribScript[(int)Pos.x, y].jewel.JewelType == type && GribManager.cell.GribCellObj[(int)Pos.x, y].cell.CellEffect == 0)
                tmp.Add(JewelSpawner.spawn.JewelGribScript[(int)Pos.x, y]);
            else
                return tmp;
        }

        return tmp;
    }
    List<JewelObj> GetBot(Vector2 Pos, int type)
    {
        List<JewelObj> tmp = new List<JewelObj>();
        for (int y = (int)Pos.y - 1; y >= 0; y--)
        {
            if (y != jewel.JewelPosition.y && JewelSpawner.spawn.JewelGribScript[(int)Pos.x, y] != null && JewelSpawner.spawn.JewelGribScript[(int)Pos.x, y].jewel.JewelType == type && GribManager.cell.GribCellObj[(int)Pos.x, y].cell.CellEffect == 0)
                tmp.Add(JewelSpawner.spawn.JewelGribScript[(int)Pos.x, y]);
            else
                return tmp;
        }

        return tmp;
    }

    private void RemoveFromList(int x, int y)
    {
        JewelSpawner.spawn.JewelGribScript[x, y] = null;
        JewelSpawner.spawn.JewelGrib[x, y] = null;

        GetComponent<Collider2D>().enabled = false;
    }

    public int getListcount()
    {
        List<JewelObj> list = Ulti.ListPlus(GetRow(jewel.JewelPosition, jewel.JewelType, null),
                                            GetCollumn(jewel.JewelPosition, jewel.JewelType, null),
                                            this);

        return list.Count;
    }
    public List<JewelObj> getList()
    {
        List<JewelObj> list = Ulti.ListPlus(GetRow(jewel.JewelPosition, jewel.JewelType, null),
                                            GetCollumn(jewel.JewelPosition, jewel.JewelType, null),
                                            this);

        return list;
    }

    public void RuleChecker()
    {
        if (jewel.JewelType != 99)
        {
            List<JewelObj> list = Ulti.ListPlus(GetRow(jewel.JewelPosition, jewel.JewelType, null),
                                                      GetCollumn(jewel.JewelPosition, jewel.JewelType, null),
                                                      this);
		
            if (list.Count >= 3)
            {
                listProcess(list);
                Checked = true;
            }
			else
			{
				//Detect end of move nly when there is no more move
				if(GameController.action.MoveLeft == 0)
				GameController.action.StartCountdown();
			}

        }
        else
        {
            GameController.action.WinChecker();
        }
		
    }

    void listProcess(List<JewelObj> list)
    {
        List<int> _listint = new List<int>();
        for (int i = 0; i < list.Count; i++)
        {
            if (!list[i].Checked)
                _listint.Add(list[i].getListcount());
            else
                _listint.Add(list.Count);
        }
        int max = Mathf.Max(_listint.ToArray());
        int idx = _listint.IndexOf(max);
        GameController.action.JewelProcess(list[idx].getList(), this.gameObject);
    }

    public void Bounce()
    {
        if (GameController.action.GameState == (int)Timer.GameState.PLAYING && !Supporter.sp.isNomove)
        {
            Animation anim = render.GetComponent<Animation>();
            anim.enabled = true;
            anim.Play("bounce");
        }
    }

    public void JewelDisable()
    {
        //Animation anim = render.GetComponent<Animation>();
        //anim.enabled = true;
        //anim.Play("Disable");
		//render.Reset();
		render =  transform.GetChild(0).GetComponent<SkeletonAnimation>();
		render.enabled = true;
		render.skeleton.SetToSetupPose ();
		render.state.SetAnimation (0, "disappear", false);
    }

    public void JewelEnable()
    {
        //Animation anim = render.GetComponent<Animation>();
        //anim.enabled = true;
        //anim.Play("Enable");
		//render.Reset();
		//render.skeleton.SetToSetupPose ();
		render =  transform.GetChild(0).GetComponent<SkeletonAnimation>();
		render.enabled = true;
		render.state.SetAnimation (0, "appear", false);
    }

    public void JewelSuggesttion()
    {
        //Animation anim = render.GetComponent<Animation>();
        //anim.enabled = true;
        //anim.Play("Suggesttion");
		//render.Reset();
		render =  transform.GetChild(0).GetComponent<SkeletonAnimation>();
		render.enabled = true;
		render.skeleton.SetToSetupPose ();
		render.state.SetAnimation (0, "suggest", false);
    }
    public void JewelStopSuggesttion()
    {
		//render.Reset();
		render =  transform.GetChild(0).GetComponent<SkeletonAnimation>();
		render.enabled = true;
		render.skeleton.SetToSetupPose ();
		render.state.SetAnimation (0, "idle", false);
    }

	public void BoomEffect(Vector3 groundZero)
	{
		StartCoroutine ( Boom(groundZero) );
	}

	IEnumerator Boom(Vector3 groundZero)
	{
		Vector3 oldpos;
		float distance = Vector3.Distance(groundZero, transform.position);
		float impact =  1 / (distance / 4); //The further the ground zero, the less impact it have
		
		Vector3 dir = groundZero - transform.position;
		Vector3 destination =  transform.position - (dir.normalized * impact);
		
		yield return new WaitForSeconds(0.5f);
		oldpos = transform.position;
		iTween.MoveTo(gameObject,iTween.Hash("position",destination,
		                                     "easetype",iTween.EaseType.easeInOutSine,
		                                     "time",.8f));
		
		yield return new WaitForSeconds(0.8f);
		
		iTween.MoveTo(gameObject,iTween.Hash("position",oldpos,
		                                     "easetype",iTween.EaseType.easeInOutSine,
		                                     "time",.8f));
	}
}
