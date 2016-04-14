using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;

public class LoginPopup : MonoBehaviour {

	// Use this for initialization
	void Start () {
		DatabaseManager.Instance.createTable ();

		AccountType accountType = SessionManager.Instance.LoadSession ();

		if (accountType == AccountType.ANONYMOUS) {
			print ("ANON");
			requestLogin ();
		}
		if (accountType == AccountType.FACEBOOK) {
			print ("FB");
			FacebookManager.Instance.RequestConnectFB ();
		}

		//SessionManager.Instance.ClearSession ();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GenerateAnonymousAccount()
	{
		RequestManager.Instance.requestSubmitDevice ("" + Application.platform, SystemInfo.deviceModel, SystemInfo.deviceUniqueIdentifier, "2", "2", SubmitDeviceCallback, SubmitDeviceError);
	}

	void SubmitDeviceError(object data, int errorCode)
	{
		Debug.Log("SubmitDeviceError result with errorCode " + errorCode);
	}

	void SubmitDeviceCallback(object data, int errorCode)
	{
		Debug.Log("SubmitDeviceCallback result with errorCode " + errorCode);
		if (errorCode == 0)
		{
			PlayerPrefs.SetInt("smdv", 1);

			Dictionary<string, object> resultData = (Dictionary<string, object>)data;
			string id =  (string)resultData[GameConstants.RESPONE_KEY_DEVICE_ID];
			insertAccount(SessionManager.Instance.Account.UserId, SessionManager.Instance.Account.UserId, SessionManager.Instance.Account.UserId, GameConstants.ACCOUNT_TYPE_ANONYMOUS);
			RequestManager.Instance.requestGenerateAnonymousAccount (id, GenerateAnonymousCallback, LoginError);
		}
	}

	void GenerateAnonymousCallback(object data, int errorCode)
	{
		Debug.Log("GenerateAnonymousAccountCallback result with errorCode " + errorCode);
		if (errorCode == 0)
		{
			Dictionary<string, object> resultData = (Dictionary<string, object>)data;
			SessionManager.Instance.Account.UserName = (string)resultData[GameConstants.RESPONE_KEY_USER_NAME];	
			SessionManager.Instance.Account.Password = (string)resultData[GameConstants.RESPONE_KEY_PASSWORD];	
			insertAccount(SessionManager.Instance.Account.UserId, SessionManager.Instance.Account.UserName, SessionManager.Instance.Account.Password, GameConstants.ACCOUNT_TYPE_ANONYMOUS);
			requestLogin();
		}
	}

	private void requestLogin()
	{
		Debug.Log("requestLogin");
		//RequestManager.Instance.requestLogin ("65603929", "6f@LkBdkQg5", LoginCallback, LoginError);
		RequestManager.Instance.requestLogin (SessionManager.Instance.Account.UserName, SessionManager.Instance.Account.Password, LoginCallback, LoginError);
	}

	void LoginCallback(object data, int errorCode)
	{
		Debug.Log("requestLoginCallback result with errorCode " + errorCode);
		if (errorCode == 0)
		{
			Dictionary<string, object> resultData = (Dictionary<string, object>)data;
			SessionManager.Instance.SessionKey = (string)resultData[GameConstants.RESPONE_KEY_SESSION_KEY];	
			//requestGetFriendPlayedGame(1, 500);

			SessionManager.Instance.SaveSession ();
			NextScene ();
		}
	}

	void LoginError(object data, int errorCode)
	{
		Debug.Log("requestLoginError result with errorCode " + errorCode);
	}

	private void insertAccount(string userId, string userName, string password, string accountType)
	{
		DatabaseManager.Instance.InsertAccount (userId, userName, password, accountType);
	}

	private void insertSession(string userId, string sessionKey)
	{
		DatabaseManager.Instance.InsertSession (userId, sessionKey);
	}
		



	private void requestGetFriendPlayedGame(int page, int count)
	{
		RequestManager.Instance.requestGetFriendPlayedGame (SessionManager.Instance.SessionKey, "" + page, "" + count, GetFriendPlayedGameCallback, GetFriendPlayedGameError);
	}

	void GetFriendPlayedGameCallback(object data, int errorCode)
	{
		Debug.Log("GetFriendPlayedGameCallback result with errorCode " + errorCode);
		if (errorCode == 0)
		{
			
	
		}
	}

	void GetFriendPlayedGameError(object data, int errorCode)
	{
		Debug.Log("GetFriendPlayedGameError result with errorCode " + errorCode);
	}	

	void NextScene()
	{
		Application.LoadLevel("MapScene");
		CameraMovement.StarPointMoveIndex = -1;
	}
}
