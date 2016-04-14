using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using com.b2mi.dc.Security;
using com.b2mi.dc.Network.Core;
using com.b2mi.dc.Network.RequestAPI;
using MiniJSON;

public class RequestManager {
	private const string TAG 								= "RequestManager";
	
	private const string URL 								= "http://giftalk.com.vn/";
	private const string URI_USER_FB_LOGIN_INFO 			= "user/deviceinfo";
	private const string URI_SUBMIT_DEVICE_INFO				= "user/deviceinfo";
	private const string URI_USER_GAME_INFO 				= "user/game/level_score";
	private const string URI_USER_GAME_HIGHSCORE 			= "user/game/highscore";
	private const string URI_USER_GAME_LEVEL_COMPLETE 		= "user/game/level_complete";
	private const string URI_USER_FRIEND_INGAME 			= "user/friend/ingame";
	private const string URI_USER_FRIEND_INVITE 			= "user/friend/get_invite";
	private const string URI_SOCIAL_REQUEST_LIST 			= "social/request/get";
	private const string URI_SOCIAL_REQUEST_ACCEPT_SUBMIT 	= "social/request/submit";
	private const string URI_SOCIAL_REQUEST_SEND_SUBMIT 	= "social/request/send";
	private const string URI_SOCIAL_GET_STORY 				= "social/story/getdata";
	private const string URI_SOCIAL_SHARE_STORY 			= "social/story/share";
	private const string URI_IAP_ANDROID_VALID 				= "purchase/android/valid";
	private const string URI_IAP_IOS_VALID 					= "purchase/ios/valid";
	private const string URI_GENERATE_ANONYMOUS_ACCOUNT 	= "user/anonymous";
	private const string URI_UPDATE_ACCOUNT_INFO			= "user/info/update";
	private const string URI_GET_HIGH_SCORES				= "user/game/highscore";
	private const string URI_GET_FRIEND_PLAYED_GAME			= "user/friend/get";
	private const string URI_GET_FRIEND_INVITE				= "user/friend/get_invite";
	private const string URI_FACEBOOK_CONNECT				= "fb/connect";
	private const string URI_LOGIN_FACEBOOK					= "fb/login";
	private const string URI_REQUESTING_LIST				= "request/get";
	private const string URI_SUBMIT_REQUEST_ACCEPT			= "request/accept";
	private const string URI_SEND_REQUEST_FACEBOOK			= "request/send";
	private const string URI_SHARE_STORY					= "social/story/share";
	private const string URI_LOGIN 	= "/user/login";

	private static volatile RequestManager instance 		= null;
	private static object singletonLock 					= new object();
	/**
	 * Singleton methods
	 */
	
	public RequestManager ()
	{
		
	}	
	
	public static RequestManager Instance
	{
		get
		{
			if ( instance == null )
			{
				lock( singletonLock )
				{				
					instance = new RequestManager();
				}
			}
			
			return instance; 
		}
	}	

	public void Init()
	{
	}	

	public void requestSubmitDevice(string platform, string model, string imei, string version, string appVersion, SuccessCallback successCallback, ErrorCallback errorCallback) 
	{		
		Debug.Log( TAG + ": " + "requestSubmitDevice");
		SubmitDeviceRequest request = new SubmitDeviceRequest( URL + URI_SUBMIT_DEVICE_INFO, platform, model, imei, version, appVersion, successCallback, errorCallback);
		request.SendRequest();
	}

	public void requestGenerateAnonymousAccount(string deviceId, SuccessCallback successCallback, ErrorCallback errorCallback) 
	{		
		Debug.Log( TAG + ": " + "requestGenerateAnonymousAccount");
		GenerateAnonymousAcountRequest request = new GenerateAnonymousAcountRequest( URL + URI_GENERATE_ANONYMOUS_ACCOUNT, deviceId, successCallback, errorCallback);
		request.SendRequest();
	}

	public void requestLogin(string userName, string password, SuccessCallback successCallback, ErrorCallback errorCallback) 
	{		
		Debug.Log( TAG + ": " + "LoginRequest" + " URL: " +  URL + URI_LOGIN + " userName: " + userName + " password: " + password);
		LoginRequest request = new LoginRequest( URL + URI_LOGIN, userName, password, successCallback, errorCallback);
		request.SendRequest();
	}

	public void requestUpdateAccountInfo(string sessionKey, string displayName, string avatar, SuccessCallback successCallback, ErrorCallback errorCallback) 
	{		
		Debug.Log( TAG + ": " + "requestUpdateAccountInfo");
		UpdateAcountInfoRequest request = new UpdateAcountInfoRequest( URL + URI_UPDATE_ACCOUNT_INFO, sessionKey, displayName, avatar, successCallback, errorCallback);
		request.SendRequest();
	}

	public void requestSubmitUserScores(string sessionKey, string data, SuccessCallback successCallback, ErrorCallback errorCallback) 
	{		
		Debug.Log( TAG + ": " + "requestSubmitUserScores");
		SubmitUserScoresRequest request = new SubmitUserScoresRequest( URL + URI_UPDATE_ACCOUNT_INFO, sessionKey, data, successCallback, errorCallback);
		request.SendRequest();
	}

	public void requestGetHighScores(string sessionKey, string level, SuccessCallback successCallback, ErrorCallback errorCallback) 
	{		
		Debug.Log( TAG + ": " + "requestGetHighScores");
		GetHighScoresRequest request = new GetHighScoresRequest( URL + URI_GET_HIGH_SCORES, sessionKey, level, successCallback, errorCallback);
		request.SendRequest();
	}

	public void requestGetFriendPlayedGame(string sessionKey, string page, string count, SuccessCallback successCallback, ErrorCallback errorCallback) 
	{		
		Debug.Log( TAG + ": " + "requestGetFriendPlayedGame");
		GetFriendPlayedGameRequest request = new GetFriendPlayedGameRequest( URL + URI_GET_FRIEND_PLAYED_GAME, sessionKey, page, count, successCallback, errorCallback);
		request.SendRequest();
	}

	public void requestGetFriendInvite(string sessionKey, string limit, string next, SuccessCallback successCallback, ErrorCallback errorCallback) 
	{		
		Debug.Log( TAG + ": " + "requestGetFriendInvite");
		GetInviteFriendRequest request = new GetInviteFriendRequest( URL + URI_GET_FRIEND_INVITE, sessionKey, limit, next, successCallback, errorCallback);
		request.SendRequest();
	}

	public void requestFacebookConnect(string sessionKey, string token, SuccessCallback successCallback, ErrorCallback errorCallback) 
	{		
		Debug.Log( TAG + ": " + "requestFacebookConnect");
		FacebookConnectRequest request = new FacebookConnectRequest( URL + URI_FACEBOOK_CONNECT, sessionKey, token, successCallback, errorCallback);
		request.SendRequest();
	}

	public void requestLoginFacebook(string token, SuccessCallback successCallback, ErrorCallback errorCallback) 
	{		
		Debug.Log( TAG + ": " + "requestLoginFacebook");
		LoginFacebookRequest request = new LoginFacebookRequest( URL + URI_LOGIN_FACEBOOK, token, successCallback, errorCallback);
		request.SendRequest();
	}

	public void requestGetRequestingList(string sessionKey, SuccessCallback successCallback, ErrorCallback errorCallback) 
	{		
		Debug.Log( TAG + ": " + "requestGetRequestingList");
		GetRequestingListRequest request = new GetRequestingListRequest( URL + URI_LOGIN_FACEBOOK, sessionKey, successCallback, errorCallback);
		request.SendRequest();
	}

	public void requestSubmitRequestAccept(string sessionKey, string reqIds, SuccessCallback successCallback, ErrorCallback errorCallback) 
	{		
		Debug.Log( TAG + ": " + "requestSubmitRequestAccept");
		SubmitRequestAcceptRequest request = new SubmitRequestAcceptRequest( URL + URI_SUBMIT_REQUEST_ACCEPT, sessionKey, reqIds, successCallback, errorCallback);
		request.SendRequest();
	}

	public void requestSendRequestFB(string sessionKey, String uids, String type, SuccessCallback successCallback, ErrorCallback errorCallback) 
	{		
		Debug.Log( TAG + ": " + "requestSendRequestFB");
		SendRequestFBRequest request = new SendRequestFBRequest( URL + URI_SEND_REQUEST_FACEBOOK, sessionKey, uids, type, successCallback, errorCallback);
		request.SendRequest();
	}

	public void requestShareStory(String uid, String type, String data, SuccessCallback successCallback, ErrorCallback errorCallback) 
	{		
		Debug.Log( TAG + ": " + "requestShareStory");
		ShareStoryRequest request = new ShareStoryRequest( URL + URI_SHARE_STORY, uid, type, data, successCallback, errorCallback);
		request.SendRequest();
	}
}
