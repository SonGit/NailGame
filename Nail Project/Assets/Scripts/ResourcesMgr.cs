using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourcesMgr{

	private static ResourcesMgr _instance = null;
	
	public static ResourcesMgr Instance{
		get{
			if(_instance == null){
				_instance = new ResourcesMgr();
			}
			
			return _instance;
		}
	}
	
	public enum ResourceType
	{
		SPRITE,
		ANIMATION
	};

	public static readonly List<string> GuestItemAnim2 = new List<string>(){
		{"Animation/Anim_Bee"},
		{"Animation/Anim_HummingBird"},
		{"Animation/Anim_Firefly"},
	//	{"Animation/Anim_Ladybug"},
	};

	public static readonly List<string> GuestItemAnim = new List<string>(){
		{"Sprites/char1"},
		{"Sprites/char2"},
		{"Sprites/char3"},
	};

	public static readonly List<string> ItemSprites = new List<string>(){
		{"Items/item_1"},
	};
	
	private static Dictionary<string,Sprite> _spriteDatas = new Dictionary<string, Sprite>();
	private static Dictionary<string,SkeletonDataAsset> _animationDatas = new Dictionary<string, SkeletonDataAsset>();

	public static void LoadResources( ResourceType type , List<string> listToLoad ){
		
		if( type == ResourceType.SPRITE ){
			foreach( var item in listToLoad ){
				var data = Resources.Load<Sprite>(item);
				_spriteDatas.Add(data.name , data);
			}
		}
	}

	public static Sprite GetSprite ( string name ){
		Sprite data = null;
		if( _spriteDatas.TryGetValue(name, out data) ){
			return data;
		}
		
		return null;
	}
	
	public static SkeletonDataAsset GetAnimation ( string name ){
		SkeletonDataAsset data = null;
		if( _animationDatas.TryGetValue(name, out data) ){
			return data;
		}
		
		return null;
	}

	//Animation Guest
	public static readonly string Anim_Bee = "Anim_Bee";

	public static readonly string Anim_HummingBird = "Anim_HummingBird";

	public static readonly string Anim_Firefly = "Anim_Firefly";

	public static readonly string Anim_Ladybug = "Anim_Ladybug";

	public static readonly string Anim_Guest_Value_Idle = "idle";

	public static readonly string Anim_Guest_Value_Appear = "appear";

	public static readonly string Anim_Guest_Value_Blink = "blinking";

	public static readonly string Anim_Guest_Value_Disappear= "disappear";

	//Animation Glass
	public static readonly string Glass_HoaCucXanh_Skin = "hoacucxanh";

	public static readonly string Glass_HoaHong_Skin = "hoahong";

	public static readonly string Glass_HoaHuongDuong_Skin = "hoahuongduong";

	public static readonly string Glass_HoaLoaKen_Skin = "hoaloaken";

	public static readonly string Glass_Lavender_Skin = "lavender";

	public static readonly string Glass_Tulip_Skin = "tulip";
}
