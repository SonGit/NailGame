using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuestGlass : MonoBehaviour {

	
	public SkeletonAnimation _glassAnim;
	
	string skin;
	
	int _animIndex;
	
	private static readonly List<string> _glassFillAnimations = new List<string>(){
		"level10",
		"level20",
		"level30",
		"level40",
		"level50",
		"level60",
		"level70",
		"level80",
		"level90",
		"level100",
	};
	
	private static readonly List<string> _glassIdleAnimations = new List<string>(){
		"level10idle",
		"level20idle",
		"level30idle",
		"level40idle",
		"level50idle",
		"level60idle",
		"level70idle",
		"level80idle",
		"level90idle",
		"level100idle",
	};
	
	private static readonly Dictionary<int,string> _glassSkin = new Dictionary<int,string>(){
		{0,ResourcesMgr.Glass_HoaCucXanh_Skin},
		{1,ResourcesMgr.Glass_HoaHong_Skin},
		{2,ResourcesMgr.Glass_HoaHuongDuong_Skin},
		{3,ResourcesMgr.Glass_Lavender_Skin},
		{4,ResourcesMgr.Glass_HoaLoaKen_Skin},
		{5,ResourcesMgr.Glass_Tulip_Skin},
	};
	
	public void Init(int itemType)
	{
		_animIndex = 0;
		skin = _glassSkin[itemType];
		_glassAnim.skeleton.SetSkin(skin);
	}
	
	public void Fill(float percent)
	{
		_animIndex = (int)(percent/10);
		if (_animIndex >= _glassFillAnimations.Count)
			_animIndex = _glassFillAnimations.Count - 1;
		
		FillAnimation (_glassFillAnimations[_animIndex]);
	}
	
	private void FillAnimation ( string name ){
		_glassAnim.Reset();
		_glassAnim.skeleton.SetToSetupPose ();
		_glassAnim.state.SetAnimation (0, name, false);
		_glassAnim.skeleton.SetSkin(skin);
		_glassAnim.state.End += OnEnd;
	}
	
	private void OnEnd( Spine.AnimationState state , int trackIndex)
	{
		_glassAnim.state.End -= OnEnd;
		IdleAnimation (_glassIdleAnimations[_animIndex]);
	}
	
	private void IdleAnimation ( string name ){
		_glassAnim.Reset();
		_glassAnim.skeleton.SetToSetupPose ();
		_glassAnim.state.SetAnimation (0, name, true);
		_glassAnim.skeleton.SetSkin(skin);
	}
}
