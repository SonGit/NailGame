using UnityEngine;
using System.Collections;

public class PopupInviteFriends : MonoBehaviour {
	private static GameObject _instance = null;
	private static bool IsPopupShow = false;

	public GameObject _InviteFriendItem;
	
	public static void ShowPopup() {
		if (_instance == null) {
			GameObject mapLayer =  GameObject.Find("Canvas");
			_instance =  Instantiate( Resources.Load("Prefabs/Popup/PopupInviteFriends") as GameObject );
			_instance.transform.parent = (mapLayer.transform);
			_instance.transform.localScale = Vector3.one;
		}

		IsPopupShow = !IsPopupShow;
		if (IsPopupShow) {
			_instance.transform.localPosition = new Vector2(-1000,0);
			_instance.SetActive (true);

			iTween.MoveTo(_instance,iTween.Hash("position",new Vector3(6,_instance.transform.localPosition.y,0),
			                                     "islocal",true,
			                                     "easetype",iTween.EaseType.easeInOutSine,
			                                     "time",.5f));
		} else {
			_instance.gameObject.GetComponent<PopupInviteFriends> ().CloseButtonHandle();
		}
	}
	
	void OnDestroy () {
		IsPopupShow = false;
		_instance = null;
	}
	
	void Start() {

	}
	
	void DeletePopup() {
		GameObject.Destroy (gameObject);
	}
	
	public void CloseButtonHandle() {
	iTween.MoveTo(gameObject,iTween.Hash("position",new Vector3(-1000,_instance.transform.localPosition.y,0),
		                                     "islocal",true,
		                                     "easetype",iTween.EaseType.easeInOutSine,
		                                     "time",.25f));
		IsPopupShow = false;
	}
	
	public void SendButtonHandle() {
		CloseButtonHandle ();
	}
}
