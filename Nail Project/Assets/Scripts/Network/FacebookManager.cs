
using System.Collections;
using Facebook.Unity;
using UnityEngine;
using System.Collections.Generic;

public class FacebookManager  {
	
	private static volatile FacebookManager instance 		= null;
	private static object singletonLock 					= new object();

	public static FacebookManager Instance
	{
		get
		{
			if ( instance == null )
			{
				lock( singletonLock )
				{				
					instance = new FacebookManager();
					instance.Init ();
				}
			}

			return instance; 
		}
	}

	void Init()
	{

	}

	public void RequestLoginFB()
	{
		if (!FB.IsInitialized)
			FB.Init (this.LoginFB);
		else
			LoginFB ();
	}

	public void RequestConnectFB()
	{
		if (!FB.IsInitialized)
			FB.Init (this. requestLoginFB);
		else
			requestLoginFB() ;
	}

	void LoginFB()
	{
		FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, this.HandleResult);
	}

	protected void HandleResult(IResult result)
	{
		if (result == null)
		{
			
			return;
		}

		SessionManager.Instance.UserInfo.UserId = Facebook.Unity.AccessToken.CurrentAccessToken.UserId;
		SessionManager.Instance.UserInfo.AccessToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;

		requestConnectFB ();
	}

	private void requestConnectFB()
	{
		Debug.Log("requestConnectFB");

		RequestManager.Instance.requestFacebookConnect (SessionManager.Instance.SessionKey, SessionManager.Instance.UserInfo.AccessToken, ConnectFBCallback, ConnectFBError);
	}

	void ConnectFBCallback(object data, int errorCode)
	{
		Debug.Log("ConnectFBCallback result with errorCode " + errorCode);
		if (errorCode == 0)
		{
			SessionManager.Instance.Account.UserId = Facebook.Unity.AccessToken.CurrentAccessToken.UserId;
			SessionManager.Instance.Token = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
			requestLoginFB();
		}
	}

	void ConnectFBError(object data, int errorCode)
	{
		Debug.Log("ConnectFBError result with errorCode " + errorCode);
	}

	private void requestLoginFB()
	{
		Debug.Log("requestLoginFB");
		RequestManager.Instance.requestLoginFacebook (SessionManager.Instance.UserInfo.AccessToken, LoginFBCallback, LoginFBError);
	}

	void LoginFBCallback(object data, int errorCode)
	{
		Debug.Log("LoginFBCallback result with errorCode " + errorCode);
		if (errorCode == 0)
		{
			Dictionary<string, object> resultData = (Dictionary<string, object>)data;
			SessionManager.Instance.SessionKey = (string)resultData[GameConstants.RESPONE_KEY_SESSION_KEY];	
			insertSession(SessionManager.Instance.Account.UserId, SessionManager.Instance.SessionKey);
			OnLoggedIn ();
		}
	}

	void LoginFBError(object data, int errorCode)
	{
		Debug.Log("LoginFBError result with errorCode " + errorCode);
	}

	private void insertSession(string userId, string sessionKey)
	{
		DatabaseManager.Instance.InsertSession (userId, sessionKey);
	}

	void OnLoggedIn()
	{
		SessionManager.Instance.SaveSession ();

		if (Application.loadedLevelName == "HomeScene") {
			Application.LoadLevel("MapScene");
			CameraMovement.StarPointMoveIndex = -1;
		}
	}
}
