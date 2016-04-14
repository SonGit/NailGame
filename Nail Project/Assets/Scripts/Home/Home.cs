using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Facebook.Unity;
using Facebook.MiniJSON;
using System.Collections.Generic;

public class Home : MonoBehaviour
{
	public SpriteRenderer test;
    void Start()
    {
        // hidden banner (banner only show on Game Play scene)
//        GoogleMobileAdsScript.advertise.HideBanner();
        MusicController.Music.BG_menu();
		ResourcesMgr.Init ();
    }

	public void TestFacebook()
	{
		FB.Init(this.OnInitFB);
	}

	void OnInitFB()
	{
		FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, this.HandleResult);
		//requestConnectFB();
	}



	protected void HandleResult(IResult result)
	{
		if (result == null)
		{
			print ("nyll");
			return;
		}

		SessionManager.Instance.UserInfo.UserId = Facebook.Unity.AccessToken.CurrentAccessToken.UserId;
		//SessionManager.Instance.UserInfo.AccessToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
		SessionManager.Instance.Token = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
		//print (Facebook.Unity.AccessToken.CurrentAccessToken.TokenString);
		requestLoginFB();
	}

	private void requestLoginFB()
	{
		Debug.Log("requestLoginFB");
		RequestManager.Instance.requestLoginFacebook (SessionManager.Instance.Token, LoginFBCallback, LoginFBError);
	}

	void LoginFBCallback(object data, int errorCode)
	{
		Debug.Log("LoginFBCallback result with errorCode " + errorCode);
		if (errorCode == 0)
		{
			//Dictionary<string, object> resultData = (Dictionary<string, object>)data;
			//SessionManager.Instance.SessionKey = (string)resultData[GameConstants.RESPONE_KEY_SESSION_KEY];	
			//insertSession(SessionManager.Instance.Account.UserId, SessionManager.Instance.SessionKey);
			//requestGetFriendPlayedGame(1, 500);
		}
	}

	void LoginFBError(object data, int errorCode)
	{
		Debug.Log("LoginFBError result with errorCode " + errorCode);
	}
		

    void Update()
    {
        // Exit game if click Escape key or back on mobile
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitOK();
        }
    }

    /// <summary>
    /// Exit game
    /// </summary>
    public void ExitOK()
    {
        Application.Quit();
    }

}
