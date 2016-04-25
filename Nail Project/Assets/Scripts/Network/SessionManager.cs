using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using com.b2mi.dc.Security;
using com.b2mi.dc.Network.Core;
using com.b2mi.dc.Network.RequestAPI;
using com.b2mi.dc.Entity;
using MiniJSON;

public enum AccountType
{
	NULL,
	ANONYMOUS,
	FACEBOOK
};

public class SessionManager {
	private const string TAG 								= "SessionManager";

	private string sessionKey;
	private string token;

	private AccountEntity account;
	private UserEntity userInfo 							;

	private static volatile SessionManager instance 		= null;
	private static object singletonLock 					= new object();

	/**
	 * Singleton methods
	 */	
	public SessionManager ()
	{

	}	

	public static SessionManager Instance
	{
		get
		{
			if ( instance == null )
			{
				lock( singletonLock )
				{				
					instance = new SessionManager();
					instance.Init ();
				}
			}

			return instance; 
		}
	}	

	public void Init()
	{
		sessionKey = "";
		account = new AccountEntity ();
		userInfo 							= new UserEntity();
	}	

	public void ClearSession()
	{
		PlayerPrefs.DeleteKey ("Username");
		PlayerPrefs.DeleteKey ("Password");
		PlayerPrefs.DeleteKey ("AccessToken");
		PlayerPrefs.DeleteKey ("SessionKey");
	}

	public void SaveSession()
	{
		// No access token = normal account
		if (SessionManager.Instance.UserInfo.AccessToken == String.Empty) {
			PlayerPrefs.SetString ("Username", SessionManager.Instance.Account.UserName);
			PlayerPrefs.SetString ("Password", SessionManager.Instance.Account.Password);
		} else {
			//Facebook account
			PlayerPrefs.SetString ("AccessToken", SessionManager.Instance.UserInfo.AccessToken);
		}

		PlayerPrefs.SetString ("SessionKey", SessionManager.Instance.SessionKey);
	}

	public AccountType LoadSession()
	{
		string accessToken;
		string username;
		string password;
		AccountType accountType = AccountType.NULL;

		if (!PlayerPrefs.HasKey ("SessionKey")) {
			return AccountType.NULL;
		}

		//Facebook account
		if (PlayerPrefs.HasKey ("AccessToken")) {
			accessToken = PlayerPrefs.GetString ("AccessToken");
			if (accessToken != string.Empty) {
				SessionManager.Instance.UserInfo.AccessToken = accessToken;
				accountType = AccountType.FACEBOOK;
			}
		} else {
			//Normal account
			username = PlayerPrefs.GetString ("Username");
			password = PlayerPrefs.GetString ("Password");

			if (username != string.Empty && password != string.Empty) {
				SessionManager.Instance.Account.UserName = username;
				SessionManager.Instance.Account.Password = password;
				accountType = AccountType.ANONYMOUS;
			}
		}

		SessionManager.Instance.SessionKey = PlayerPrefs.GetString ("SessionKey");
		return accountType;
	}

	public string SessionKey
	{
		get { return sessionKey;}
		set { sessionKey = value;}
	}

	public string Token
	{
		get { return token;}
		set { token = value;}
	}

	public UserEntity UserInfo
	{
		get { return userInfo;}
		set { userInfo = value;}
	}

	public AccountEntity Account
	{
		get { return account;}
		set { account = value;}
	}
}
