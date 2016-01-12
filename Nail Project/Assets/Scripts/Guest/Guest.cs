using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public enum GuestType{
	BEE,
	HUMMING_BIRD,
	//LADY_BUG,
	FIREFLY,
	NONE = 0xffffff
}

public class Guest : MonoBehaviour {

	public Text _label;

	public SkeletonAnimation _anim;

	public GuestType _type;

	public int _requiredItem;

	public int _requiredQuantity;

	public GuestGlass _glass;

	private int _currentQuantity;

	private GuestManager _guestManager;

	private bool _isDisappearing;

	private static readonly List<string> _animations = new List<string>(){
		ResourcesMgr.Anim_Bee,
		ResourcesMgr.Anim_HummingBird,
		//		ResourcesMgr.Anim_Ladybug,
		ResourcesMgr.Anim_Firefly,
	};

	// Use this for initialization
	void Start () {
		_guestManager = this.GetComponentInParent<GuestManager>();
	}

	public void Init(GuestType guestType,int itemType,int quantity )
	{
		_type = guestType;
		_requiredItem = itemType;
		_requiredQuantity = quantity;

		_currentQuantity = 0;
		_glass.Init (itemType);
		_label.enabled = true;
		_label.text = quantity.ToString();
		UpdateAnimation (_animations [(int)guestType]);
		PlayAppearAnim ();
	}
	
	public void Fill(int score)
	{
		StartCoroutine (Fill_Async(score));
	}
	
	private IEnumerator Fill_Async(int score)
	{
		yield return new WaitForSeconds (0.85f);//By this time, all cells have reached the glasses.
		_currentQuantity += score;
		if (_currentQuantity >= _requiredQuantity) {
			PlayDisappearAnim ();
			_label.enabled = false;
		} else {
			_label.text = (_requiredQuantity - _currentQuantity).ToString();
			_glass.Fill ((_currentQuantity * 100)/_requiredQuantity);
		}
	}
	
	private void UpdateAnimation ( string name ){
		_anim.skeletonDataAsset = ResourcesMgr.GetAnimation( name );
		_anim.Reset();
		_anim.skeleton.SetToSetupPose ();
		//_anim.state.SetAnimation (0, GameService.Anim_Value_Normal, true);
	}
	
	private void PlayAppearAnim()
	{
		StartCoroutine (Appear_Async ());
	}
	
	private void PlayDisappearAnim()
	{
		StartCoroutine (PlayDisappearAnim_Async ());
	}
	
	private void PlayIdleAnim()
	{
		StartCoroutine (PlayIdleAnim_Async ());
	}
	
	private IEnumerator Appear_Async( ){
		_anim.state.SetAnimation( 0, ResourcesMgr.Anim_Guest_Value_Appear , false );
		_anim.state.End += OnEndAppear;
		yield return null;
	}
	
	private void OnEndAppear( Spine.AnimationState state , int trackIndex)
	{
		_anim.state.End -= OnEndAppear;
		_isDisappearing = false;
		PlayIdleAnim ();
	}
	
	private IEnumerator PlayIdleAnim_Async( ){
		yield return new WaitForSeconds (7f);
		while(!_isDisappearing)
		{
			int percentage = Random.Range (0, 100);
			if (percentage < 50) {
				_anim.state.SetAnimation( 0, ResourcesMgr.Anim_Guest_Value_Blink , false );
			}
			if( percentage > 50){
				_anim.state.SetAnimation( 0, ResourcesMgr.Anim_Guest_Value_Idle , false );
			}
			
			yield return new WaitForSeconds ((float)Random.Range(4,7));
		}
	}
	
	private IEnumerator PlayDisappearAnim_Async( ){
		_isDisappearing = true;
		_anim.state.SetAnimation( 0, ResourcesMgr.Anim_Guest_Value_Disappear , false );
		_anim.state.End += OnEnd;
		yield return null;
	}
	
	private void OnEnd( Spine.AnimationState state , int trackIndex)
	{
		_anim.state.End -= OnEnd;
//		_guestManager.RefillGuest (this);
		_glass.Fill (0);
	}
}
