using UnityEngine;
using System.Collections;
using Facebook;
using Facebook.Unity;
using System.Collections.Generic;

public class FacebookManager
{
	private const string TAG  = "FacebookManager";
	private static volatile FacebookManager instance = null;
	private static object singletonLock = new object();

	public enum FBUserType
	{
		ME,
		FRIEND
	}

	public enum FBRequestType
	{
		LIFE,
		PASS
	}

	/**
	 * Singleton methods
	 */

	public FacebookManager ()
	{

	}

	public void Init()
	{
	}

	public static FacebookManager Instance {
		get
		{
			if ( instance == null )
			{
				lock( singletonLock )
				{				
					instance = new FacebookManager();
				}
			}

			return instance; 
		}
	}

	public void GetFbInfo(FacebookDelegate<ILoginResult> callback)
	{
		Debug.Log(TAG + ": " + "GetFbInfo");	
		//FB.API("/me?fields=id,first_name", Facebook.HttpMethod.GET, callback);
	}

	public void LoadPicture(string url, FacebookDelegate<ILoginResult> callback)
	{
		Debug.Log(TAG + ": " + "LoadPicture");		
		//FB.API (url, Facebook.HttpMethod.GET, callback);		
	}

	public void Login(FacebookDelegate<ILoginResult> callback)
	{
		Debug.Log(TAG + ": " + "Login");		
		FB.LogInWithReadPermissions(new List<string>(){"public_profile", "email", "user_friends"}, callback);
	}

	private void AuthCallback (ILoginResult result) {
		if (FB.IsLoggedIn) {
			// AccessToken class will have session details
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
			// Print current access token's User ID
			Debug.Log(aToken
				.UserId);
			// Print current access token's granted permissions
			foreach (string perm in aToken.Permissions) {
				Debug.Log(perm);
			}
		} else {
			Debug.Log("User cancelled login");
		}
	}

	public void Logout()
	{
		Debug.Log(TAG + ": " + "Logout");		
		FB.LogOut ();
	}



}
