using UnityEngine;
using System.Collections;

public class PopupMapSetting : MonoBehaviour {
	
	private bool IsSoundOn = true;
	private bool IsMusicOn = true;
	private static GameObject _instance = null;
	public static bool IsShowed = false;

	public GameObject SoundOnBtn;
	public GameObject SoundOffBtn;
	public GameObject MusicOffBtn;
	public GameObject MusicOnBtn;

	public static bool ShowPopup() {
		if (_instance == null) {
			GameObject mapLayer =  GameObject.Find("Canvas");
			_instance = Instantiate( Resources.Load("Prefabs/Popup/PopupMapSetting") as GameObject );

			_instance.transform.parent = (mapLayer.transform);
			_instance.transform.localScale = Vector3.one;
		}
		IsShowed = !IsShowed;
		if (IsShowed) {
			_instance.GetComponent<PopupMapSetting> ().Show();
		} else {
			_instance.GetComponent<PopupMapSetting> ().Close();
		}
		return IsShowed;
	}
	
	void OnDestroy () {
		_instance = null;
		IsShowed = false;
	}
	
	void OnClick() {
		Close ();
	}
	
	void Show() {
		_instance.gameObject.SetActive (true);
		_instance.gameObject.transform.localPosition = new Vector3 (-306, 0, 0);
		GameObject anchor = GameObject.Find("AnchorPoint");
		iTween.MoveTo(gameObject,iTween.Hash("position",new Vector3(anchor.transform.localPosition.x,0),
		                                     "islocal",true,
		                                     "easetype",iTween.EaseType.easeInOutSine,
		                                     "time",.5f));
	}
	
	void Close() {
		GameObject mapLayer = GameObject.Find ("Canvas");
		iTween.MoveTo(gameObject,iTween.Hash("position",new Vector3(-306,0,0),
		                                     "islocal",true,
		                                     "easetype",iTween.EaseType.easeInOutSine,
		                                     "time",.25f));
		Invoke ("CloseSettingLayer", 0.6f);
		IsShowed = false;
	}
	
	void CloseSettingLayer() {
		_instance.gameObject.SetActive (false);
		_instance.gameObject.transform.localPosition = new Vector3 (-9999, 0, 0);
		
		GameObject.Destroy (gameObject);
	}
	
	public void BackToMainMenu() {
		//Application.LoadLevel ("MenuScene");
	} 
	
	public void BackToMap () {
		Close ();
	}
	
	public void SoundHandle () {
		IsSoundOn = !IsSoundOn;
		if (IsSoundOn) {
			SoundOnBtn.SetActive(true);
			SoundOffBtn.SetActive(false);
		} else {
			SoundOnBtn.SetActive(false);
			SoundOffBtn.SetActive(true);
		}
	}
	
	public void MusicHandle () {
		IsMusicOn = !IsMusicOn;
		if (IsMusicOn) {
			MusicOnBtn.SetActive(true);
			MusicOffBtn.SetActive(false);
		} else {
			MusicOnBtn.SetActive(false);
			MusicOffBtn.SetActive(true);
		}
	}
}
