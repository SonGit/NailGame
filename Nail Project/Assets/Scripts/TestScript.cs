using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TestScript : MonoBehaviour {

	public static TestScript instance;
	public Text text;
	void Awake()
	{
		instance = this;
	}

	public void Send(string URL,WWWForm form)
	{
		StartCoroutine (Send2(URL,form));
	}
	// Use this for initialization
	void Start () {
		//requestLogin ();
		requestSubmitDevice ();
	}

	IEnumerator Send2(string URL,WWWForm form)
	{
		WWW w = new WWW(URL, form);
		yield return w;
		if (!string.IsNullOrEmpty(w.error)) {

		}
		else {

		}
	}
	
	private void requestAnonymous(string id)
	{
		Debug.Log("requestLogin");
		RequestManager.Instance.requestGenerateAnonymousAccount (id, LoginCallback, LoginError);
	}

	void LoginCallback(object data, int errorCode)
	{
		Debug.Log("requestLoginCallback result with errorCode " + errorCode);
		text.text =  errorCode.ToString();
		if (errorCode == com.b2mi.dc.Network.ErrorCode.SUCCESS)
		{
			//Dictionary<string, object> resultData = (Dictionary<string, object>)data;
			//SessionManager.Instance.SessionKey = (string)resultData[GameConstants.RESPONE_KEY_SESSION_KEY];	
			//insertSession(SessionManager.Instance.Account.UserId, SessionManager.Instance.SessionKey);

			//requestGetFriendPlayedGame(1, 500);
		}
	}

	void LoginError(object data, int errorCode)
	{
		Debug.Log("requestLoginError result with errorCode " + errorCode);
	}

	public void requestSubmitDevice()
	{
		RequestManager.Instance.requestSubmitDevice ("" + Application.platform, SystemInfo.deviceModel, SystemInfo.deviceUniqueIdentifier, "2", "2", SubmitDeviceCallback, SubmitDeviceError);
	}

	void SubmitDeviceCallback(object data, int errorCode)
	{
		Debug.Log("SubmitDeviceCallback result with errorCode " + errorCode);
		if (errorCode == 0)
		{
			PlayerPrefs.SetInt("smdv", 1);

			Dictionary<string, object> resultData = (Dictionary<string, object>)data;
			print ("abc: " + (string)resultData[GameConstants.RESPONE_KEY_DEVICE_ID]);
			//SessionManager.Instance.Account.UserId = (string)resultData[GameConstants.RESPONE_KEY_DEVICE_ID];			
			//insertAccount(SessionManager.Instance.Account.UserId, SessionManager.Instance.Account.UserId, SessionManager.Instance.Account.UserId, GameConstants.ACCOUNT_TYPE_ANONYMOUS);
			requestAnonymous((string)resultData[GameConstants.RESPONE_KEY_DEVICE_ID]);
		
		}
	}

	void SubmitDeviceError(object data, int errorCode)
	{

	}
}
