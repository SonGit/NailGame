using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public enum GuestType{
	HIPPO,
	CAT,
	RABBIT,
	NONE = 0xffffff
}

public enum GuestState{
	WALK_IN,
	WALK_OUT,
	IDLE,
	SIT_DOWN,
	STAND_UP,
	STAND_BY
}

public partial class Guest : MonoBehaviour {
	//ANIMATION STUFFS//

	protected const float DEFAULT_X_POS = 1000;
	protected const float DEFAULT_Y_POS = 1000;
	protected const float BACK_CHAIR_Z = 0;
	protected const float FRONT_CHAIR_Z = -0.7f;

	private GuestState state;

	private GuestState _state
	{
		get { return state;}
		set 
		{
			state = value;

			if (state == GuestState.WALK_IN || state == GuestState.WALK_OUT || state == GuestState.IDLE) {
				_anim.Reset();
				_anim.skeleton.SetToSetupPose ();
				_anim.state.SetAnimation( 0, _animValues[state] , true );
			}

			if (state == GuestState.SIT_DOWN) {
				_anim.skeleton.SetToSetupPose ();
				_anim.state.SetAnimation( 0, _animValues[state] , false );
			}

			if (state == GuestState.STAND_UP) {
				_anim.skeleton.SetToSetupPose ();
				_anim.state.SetAnimation( 0, _animValues[state] , false );
			}

			_anim.state.End += OnEnd;
		}
	}

	public SkeletonAnimation _anim;

	Dictionary<GuestState,string> _animValues = new Dictionary<GuestState, string>
	{
		{GuestState.WALK_IN,"walking"},
		{GuestState.WALK_OUT,"walking"},
		{GuestState.IDLE,"idle"},
		{GuestState.SIT_DOWN,"sitdown"},
		{GuestState.STAND_UP,"standup"},
	};
		
	void WalkIn()
	{
		_state = GuestState.WALK_IN;

		_anim.transform.localPosition = new Vector3 ( DEFAULT_X_POS, 0 , BACK_CHAIR_Z );

		iTween.MoveTo(_anim.gameObject,iTween.Hash("position",Vector3.zero,
												   "easetype",iTween.EaseType.easeInSine,
												   "islocal",true,
												   "time",3f,
											       "oncomplete" , "OnEndWalkIn",
												   "oncompletetarget" , this.gameObject));
	}

	void WalkOut()
	{
		_state = GuestState.WALK_OUT;

		_anim.transform.localPosition = Vector3.zero;
		iTween.MoveTo(_anim.gameObject,iTween.Hash("position", new Vector3(-1000,0,0),
												"easetype",iTween.EaseType.easeInSine,
												"islocal",true,
												"time",3f,
												"oncomplete" , "OnEndWalkOut",
												"oncompletetarget" , this.gameObject));
	}

	void StandUp()
	{
		_state = GuestState.STAND_UP;
	
		iTween.MoveTo(_anim.gameObject,iTween.Hash("position", (_anim.transform.localPosition + new Vector3(0,10,0)),
			"easetype",iTween.EaseType.easeInSine,
			"islocal",true,
			"time",0.5f,
			"oncomplete" , "WalkOut",
			"oncompletetarget" , this.gameObject));
	}

	void OnEndWalkIn()
	{
		_anim.transform.localPosition += new Vector3 ( 0, 50 , FRONT_CHAIR_Z);
		_state = GuestState.SIT_DOWN;
	}

	void OnEndWalkOut()
	{
		_state = GuestState.STAND_BY;
		_guestManager.RefillGuest (this);
	}

	void OnEndSitdown()
	{
		_state = GuestState.IDLE;
		Invoke ("EnableBalloon",.25f);
	}
		
	private void OnEnd( Spine.AnimationState state , int trackIndex)
	{
		_anim.state.End -= OnEnd;

		if (_state == GuestState.SIT_DOWN) {
			OnEndSitdown();
		}
	}

	public GuestState GetState()
	{
		return _state;
	}
		
}



public partial class Guest
{
	public Text _quantityLabel;

	public Image _shellacSprite;

	public Image _ballonSprite;

	public int _requiredShellac;

	private GuestManager _guestManager;

	public GuestType _guestType;

	public int _numOrder;

	public int requiredQuantity;

	private int _requiredQuantity
	{
		get { return requiredQuantity; }
		set 
		{
			requiredQuantity = value;
			_quantityLabel.text = value.ToString ();
		}
	}

	void Start()
	{
		_guestManager = this.GetComponentInParent<GuestManager>();
	}

	public void Init(GuestType guestType,int shellacType,int quantity,int numOrder)
	{
		_requiredShellac = shellacType;
		_guestType = guestType;
		_requiredQuantity = quantity;
		_numOrder = numOrder;
		HideBalloon ();
		WalkIn ();
	}

	void HideBalloon()
	{
		_shellacSprite.enabled = false;
		_quantityLabel.enabled = false;
		_ballonSprite.enabled  = false;
	}

	void EnableBalloon()
	{
		_shellacSprite.enabled = true;
		_quantityLabel.enabled = true;
		_ballonSprite.enabled  = true;
		_shellacSprite.sprite = Resources.Load<Sprite>( GetShellacSpriteName() );
	}

	string GetShellacSpriteName()
	{
		string prefix = "Sprites/Order Icon/coloricon";
		string url = prefix + (_requiredShellac + 1).ToString ();

		return url;
	}

	public void Fill(int score)
	{
		if (_state != GuestState.IDLE || _requiredQuantity <= 0)
			return;
		
		_requiredQuantity -= score;

		if (_requiredQuantity <= 0) {
			StartCoroutine (OnCompletedOrder ());
		}
	}

	IEnumerator OnCompletedOrder()
	{
		_quantityLabel.text = "OK!";
		yield return new WaitForSeconds (2f);
		HideBalloon ();
		WalkOut();
	}
}
