using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.MiniJSON;
using System;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using com.b2mi.dc.Entity;
using com.b2mi.dc.Network;
using Facebook.Unity;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public Text a;

	enum LoadingState 
	{
		WAITING_FOR_INIT,
		WAITING_FOR_INITIAL_PLAYER_DATA,
		DONE
	};

	public enum Scene 
	{
		MAIN_MENU,
		MAP,
		GAME
	};

	private const int RETRY_FACEBOOK_TIMES = 3;
	private const string VERSION = "1";

	private LoadingState loadingState = LoadingState.WAITING_FOR_INIT;
	private static MainMenu instance;
	private bool haveUserPicture = false;
	private static List<object>                 friends         = null;
	private static Dictionary<string, string>   profile         = null;

	private static Dictionary<string, Texture>  friendImages    = new Dictionary<string, Texture>();
	public Dictionary<string, FriendEntity> Friends 		= new Dictionary<string, FriendEntity>();
	public Dictionary<int, List<FriendEntity>> HighScores 		= new Dictionary<int, List<FriendEntity>> ();
	public Dictionary<int, List<FriendEntity>> FriendLevels 		= new Dictionary<int, List<FriendEntity>> ();
	public LevelEntity 	PreLevel;

	public static List<FbRequestEntity> FBRequests 				= new List<FbRequestEntity>();

	public int GameScene;

	private float popupTime;
	//private GameEventHandler handler;
	private bool isLogin = false;
	private int retryFacebookTimes;


	private Dictionary<string, List<GameObject>> backgroundAnims 	=  new Dictionary<string, List<GameObject>>();
	private List<List<GameObject>> friendLevelTextures 				=  new List<List<GameObject>>();
	private Dictionary<string, List<string>> friendLevelTextureUrls =  new Dictionary<string, List<string>>();
	private List<GameObject> backgrounds 							=  new List<GameObject> ();
	private List<Sprite> backgroundSprites 							=  new List<Sprite> ();
	private List<string> backgroundNames 							=  new List<string> ();

	public bool HasConnectToFacebook {
		set{
			hasConnectToFacebook = value;
		}
		get{
			return hasConnectToFacebook;
		}
	}

	private bool hasConnectToFacebook = false;

	void OnApplicationQuit() {
		DatabaseManager.Instance.Close();
	}

	void Start () 
	{
		//handler = new GameEventHandler();	
		//SoomlaStore.Initialize(new GameAssetStore());

		SessionManager.Instance.Init ();
		initRequest ();
		initFacebook ();
		initDB ();
	}

	void Awake()
	{
		Debug.Log("Awake");		

		// allow only one instance of the Main Menu
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}

		#if UNITY_WEBPLAYER
		// Execute javascript in iframe ;to keep the player centred
		string javaScript = @"
		window.onresize = function() {
		var unity = UnityObject2.instances[0].getUnity();
		var unityDiv = document.getElementById(""unityPlayerEmbed"");

		var width =  window.innerWidth;
		var height = window.innerHeight;

		var appWidth = " + CanvasSize.x + @";
		var appHeight = " + CanvasSize.y + @";

		unity.style.width = appWidth + ""px"";
		unity.style.height = appHeight + ""px"";

		unityDiv.style.marginLeft = (width - appWidth)/2 + ""px"";
		unityDiv.style.marginTop = (height - appHeight)/2 + ""px"";
		unityDiv.style.marginRight = (width - appWidth)/2 + ""px"";
		unityDiv.style.marginBottom = (height - appHeight)/2 + ""px"";
		}

		window.onresize(); // force it to resize now";
		Application.ExternalCall(javaScript);
		#endif

		instance = this;	
		DontDestroyOnLoad(gameObject);

		isLogin = false;
		// Initialize FB SDK
		FB.Init(SetInit, OnHideUnity);
	}	

	public static MainMenu Instance
	{
		get{
			return instance;
		}
	}

	private void SetInit()
	{
		Debug.Log("SetInit");
		InitPrefs ();

		if (FB.IsLoggedIn) {
			Debug.Log ("Already logged in");
			OnLoggedIn ();
		}
	}

	private void OnHideUnity(bool isGameShown)
	{
		Debug.Log("OnHideUnity");
		if (!isGameShown)
		{
			// pause the game - we will need to hide
			Time.timeScale = 0;
		}
		else
		{
			// start the game back up - we're getting focus again
			Time.timeScale = 1;
		}
	}

	public void FacebookLogin()
	{
		FacebookManager.Instance.Login(LoginCallback);
	}

	public void FacebookLogout()
	{
		//FacebookManager.Instance.Logout();
		hasConnectToFacebook = false;

		if (instance.GameScene == (int)Scene.MAIN_MENU)
		{
			//MainMenuController.Instance.HasConnectToFacebook = false;
			//SideMenuController.Instance.HasConnectToFacebook = false;			
		}

		if (instance.GameScene == (int)Scene.MAP) 
		{
			//SideMenuController.Instance.HasConnectToFacebook = false;
			//FriendPopupController.Instance.HasConnectToFacebook = false;
			//LosePopupController.Instance.HasConnectToFacebook = false;
			//WinPopupController.Instance.HasConnectToFacebook = false;
		}
	}

	void LoginCallback(ILoginResult result)
	{
		Debug.Log("LoginCallback access token: " + Facebook.Unity.AccessToken.CurrentAccessToken.TokenString);		
		if (FB.IsLoggedIn)
		{
			isLogin = true;
			OnLoggedIn();
			loadingState = LoadingState.WAITING_FOR_INITIAL_PLAYER_DATA;
			getFriendLevels();
		}
	}

	void OnLoggedIn()
	{
		//a.text = "LOGGED IN";
		hasConnectToFacebook = true;
		initData ();
		requestGetRequestingList ();

		if (instance.GameScene == (int)Scene.MAIN_MENU)
		{
			print ("MAIN_MENU");
			//MainMenuController.Instance.HasConnectToFacebook = true;
			//SideMenuController.Instance.HasConnectToFacebook = true;			
		}

		if (instance.GameScene == (int)Scene.MAP) 
		{
			print ("MAP");
			//SideMenuController.Instance.HasConnectToFacebook = true;
			//FriendPopupController.Instance.HasConnectToFacebook = true;
			//LosePopupController.Instance.HasConnectToFacebook = true;
			//WinPopupController.Instance.HasConnectToFacebook = true;
		}
	}

	void checkIfUserDataReady()
	{
		Debug.Log("checkIfUserDataReady");
		if (loadingState == LoadingState.WAITING_FOR_INITIAL_PLAYER_DATA)
		{
			Debug.Log("user data ready");
			loadingState = LoadingState.DONE;
		}
	}

	/**
	 * Friend request 
	 */	
	private void onBragClicked()
	{
		Debug.Log("onBragClicked");
		//FacebookManager.Instance.PostFeed ();
	}

	public void LevelStart(int level)
	{
		Debug.Log("levelStart");
		if (SessionManager.Instance.UserInfo.Levels != null && SessionManager.Instance.UserInfo.Levels.ContainsKey (level) == true) 
		{
			SessionManager.Instance.UserInfo.CurrentLevel = (LevelEntity)SessionManager.Instance.UserInfo.Levels [level];
			if (SessionManager.Instance.UserInfo.CurrentLevel == null && level > 0) {
				SessionManager.Instance.UserInfo.CurrentLevel = new LevelEntity ();
				SessionManager.Instance.UserInfo.CurrentLevel.Level = SessionManager.Instance.UserInfo.HighestLevel;
			}
		}
		else
		{
			SessionManager.Instance.UserInfo.CurrentLevel = new LevelEntity ();
			SessionManager.Instance.UserInfo.CurrentLevel.Level = SessionManager.Instance.UserInfo.HighestLevel;
			SessionManager.Instance.UserInfo.CurrentLevel.Replay = 1;

			SessionManager.Instance.UserInfo.Levels.Add (SessionManager.Instance.UserInfo.CurrentLevel.Level, SessionManager.Instance.UserInfo.CurrentLevel);
		}
		requestGetHighScores (SessionManager.Instance.UserInfo.CurrentLevel.Level);
	}

	public void LevelComplete(int level, int score, int rate)
	{
		try {
			Debug.Log("levelComplete");
			SessionManager.Instance.UserInfo.CurrentLevel.Level = level;
			SessionManager.Instance.UserInfo.CurrentLevel.Score = score;
			SessionManager.Instance.UserInfo.CurrentLevel.Rate = rate;

			updateLevel ();
			requestSubmitUserScores ();
			requestShareStory ("0", "");

			requestGetHighScores (SessionManager.Instance.UserInfo.CurrentLevel.Level);

			if (SessionManager.Instance.UserInfo.CurrentLevel.Level == SessionManager.Instance.UserInfo.HighestLevel) {
				SessionManager.Instance.UserInfo.HighestLevel++;
				SessionManager.Instance.UserInfo.HighestLevelEntity = new LevelEntity ();
				SessionManager.Instance.UserInfo.HighestLevelEntity.Level = SessionManager.Instance.UserInfo.HighestLevel;
				SessionManager.Instance.UserInfo.HighestLevelEntity.Replay = 1;
				SessionManager.Instance.UserInfo.HighestLevelEntity.Rate = 0;

				SessionManager.Instance.UserInfo.Levels.Add (SessionManager.Instance.UserInfo.HighestLevelEntity.Level, SessionManager.Instance.UserInfo.HighestLevelEntity);
				updateHighestLevel ();
				//MapScrollController.Instance.UpdateMap();
			}

			// Update Level of User
		}
		catch (Exception e){
			Debug.LogError("LevelComplete: " + e.Message);
		}
	}

	/**
	 * Prefs
	 * */
	private void InitPrefs()
	{
		//submit device
		//PlayerPrefs.SetInt("smdv", 0);
		int smdv = PlayerPrefs.GetInt ("smdv", 0);
		if (smdv == 0) {
				requestSubmitDevice();
		}

	}

	/**
	 * Facebook Methods 
	 */

	void initFacebook()
	{
		FacebookManager.Instance.Init();
	}

	void initData()
	{
		SessionManager.Instance.UserInfo.UserId = Facebook.Unity.AccessToken.CurrentAccessToken.UserId;
		SessionManager.Instance.UserInfo.AccessToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
		//SessionManager.Instance.UserInfo.Gold = GameLocalStoreInfo.CurrencyBalance;

		checkIfUserDataReady();

		if (isLogin == true) 	
		{
			requestConnectFB();
			insertFbInfo ();

			//requestGetFriendLevels(1, 500);
		} 
		else
		{
			insertFbInfo ();
			getFriends();
		}
	}
	void MyPictureCallback(string url, Texture texture)
	{
		Debug.Log("MyPictureCallback");	

		SessionManager.Instance.UserInfo.UserTexture = texture;
		haveUserPicture = true;
		checkIfUserDataReady();
	}

	public void SendInvite()
	{
		string[] recipient = {"686081168127733", "768755586479184"};
		//FacebookManager.Instance.SendInviteRequest ((int)FacebookManager.FBRequestType.LIFE , recipient, FBSendInviteCallback);
	}

	void FBSendInviteCallback(IResult result)
	{
		//Debug.Log("FBSendInviteCallback");
		//if (result.Error != null)
		//{
			//Debug.LogWarning(result.Error);
			//return;
		//}
		//requestSubmitRequestSend (result.Text);
	}	
	/**
	 * Database 
	 */

	private void initDB()
	{
		DatabaseManager.Instance.Init ();	
		getSession();
		getLevels ();
	
		if (FB.IsLoggedIn == true)
		{
			if (!Helpers.IsEmpty(SessionManager.Instance.SessionKey)) 
			{
				//getFriends ();
				getFriendLevels();
			}
		}
	}

	private void getFriends()
	{
		bool isRequestGetFriends = false;
		float lastTime = PlayerPrefs.GetFloat("ltgpf", -1); // last time get played friend
		if (lastTime == -1)
			isRequestGetFriends = true;

		float currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
		if (currentTime - lastTime > GameConstants.TIME_GET_PLAYED_FRIEND)
			isRequestGetFriends = true;

		if (isRequestGetFriends == false) 
		{
			//						DatabaseManager.Instance.getFriends (( dbCallback ) => {			
			//							Debug.Log ("get Friends result");
			//							Friends = (Dictionary<string, FriendEntity>)dbCallback.Data;				
			//						});
		} 
		else 
		{
			requestGetFriendPlayedGame(1, 500);
		}
	}

	private void getLevels()
	{
		DatabaseManager.Instance.GetLevels (( dbCallback ) => {
			Debug.Log( "get Levels result" );

			Dictionary<int, LevelEntity> levels = (Dictionary<int, LevelEntity>) dbCallback.Data;
			SessionManager.Instance.UserInfo.Levels = levels;
			try 
			{
				if (SessionManager.Instance.UserInfo.Levels != null && SessionManager.Instance.UserInfo.Levels.Count > 0)
				{
					SessionManager.Instance.UserInfo.HighestLevel = levels[SessionManager.Instance.UserInfo.Levels.Count].Level;
				}	
				else
				{
					SessionManager.Instance.UserInfo.HighestLevel = 1;
				}
			} 
			catch (Exception ex)
			{
				Debug.LogWarning(ex.Message);
				SessionManager.Instance.UserInfo.HighestLevel = 1;
			}
			LevelStart(SessionManager.Instance.UserInfo.HighestLevel);
			requestSubmitUserScores();
		});
	}

	private void getSession()
	{
		DatabaseManager.Instance.GetSessionKey (( dbCallback ) => {
			SessionManager.Instance.SessionKey = (string) dbCallback.Data;
			Debug.Log( "get sessionKey result: "  + SessionManager.Instance.SessionKey);	
		});

	}

	private void getFriendLevels()
	{
		bool isRequestGetFriends = false;
		float lastTime = PlayerPrefs.GetFloat("ltgfl", -1); // last time get played friend
		if (lastTime == -1)
			isRequestGetFriends = true;
		float currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
		if (currentTime - lastTime > GameConstants.TIME_GET_PLAYED_FRIEND)
			isRequestGetFriends = true;

		if (isRequestGetFriends == false) 
		{
			DatabaseManager.Instance.GetFriends (( dbCallback ) => {
				Debug.Log( "get Friend Levels result" );

				FriendLevels.Clear();
				List<FriendEntity> resultData = (List<FriendEntity>)dbCallback.Data;
				if (resultData != null)
				{
					int level = 1;
					foreach(FriendEntity friend in resultData)
					{
						level = friend.Level;
						if (FriendLevels.ContainsKey(friend.Level) == false)
							FriendLevels[friend.Level] = new List<FriendEntity>();
						FriendLevels[friend.Level].Add(friend);
					}

					resultData.Clear();	
				}

			});
		} 
		else 
		{
			requestGetFriendPlayedGame(1, 500);
		}
	}

	private void updateFbInfo()
	{
		//DatabaseManager.Instance.updateFbInfo (SessionManager.Instance.UserInfo.UserId, SessionManager.Instance.UserInfo.Name, Helpers.GetPictureURL ("me", 128, 128), SessionManager.Instance.UserInfo.AccessToken);
	}

	private void insertFbInfo()
	{
		//DatabaseManager.Instance.insertFbInfo (SessionManager.Instance.UserInfo.UserId, SessionManager.Instance.UserInfo.Name, Helpers.GetPictureURL ("me", 128, 128), SessionManager.Instance.UserInfo.AccessToken);
	}

	private void updateLevel()
	{
		DatabaseManager.Instance.InsertLevel (SessionManager.Instance.UserInfo.CurrentLevel.Level, SessionManager.Instance.UserInfo.CurrentLevel.Score, SessionManager.Instance.UserInfo.CurrentLevel.Rate, SessionManager.Instance.UserInfo.CurrentLevel.Replay);
	}

	private void updateHighestLevel()
	{
		DatabaseManager.Instance.InsertLevel (SessionManager.Instance.UserInfo.HighestLevelEntity.Level, SessionManager.Instance.UserInfo.HighestLevelEntity.Score, SessionManager.Instance.UserInfo.HighestLevelEntity.Rate, SessionManager.Instance.UserInfo.HighestLevelEntity.Replay);
	}

	private void insertFriendPlayedGame(List<FriendEntity> friends)
	{
		//DatabaseManager.Instance.insertFriends (friends);
	}

	private void insertFriendLevels(List<FriendEntity> friends)
	{
		DatabaseManager.Instance.InsertFriends (friends);
	}

	private void deleteAllFriends()
	{
		//DatabaseManager.Instance.deleteAllFriends();
	}

	private void deleteFbInfo()
	{
		//DatabaseManager.Instance.deleteFbInfo();
	}

	private void deleteFriend()
	{
		//DatabaseManager.Instance.deleteFriend();
	}

	private void insertAccount(string userId, string userName, string password, string accountType)
	{
		DatabaseManager.Instance.InsertAccount (userId, userName, password, accountType);
	}

	private void insertSession(string userId, string sessionKey)
	{
		DatabaseManager.Instance.InsertSession (userId, sessionKey);
	}

	/**
	 * Request Methods 
	 */

	private void initRequest()
	{
		RequestManager.Instance.Init ();
	}

	//
	private void requestSubmitDevice()
	{
		RequestManager.Instance.requestSubmitDevice ("" + Application.platform, SystemInfo.deviceModel, SystemInfo.deviceUniqueIdentifier, VERSION, VERSION, SubmitDeviceCallback, SubmitDeviceError);
	}

	void SubmitDeviceCallback(object data, int errorCode)
	{
		Debug.Log("SubmitDeviceCallback result with errorCode " + errorCode);
		if (errorCode == ErrorCode.SUCCESS)
		{
			PlayerPrefs.SetInt("smdv", 1);

			Dictionary<string, object> resultData = (Dictionary<string, object>)data;
			SessionManager.Instance.Account.UserId = (string)resultData[GameConstants.RESPONE_KEY_DEVICE_ID];			
			insertAccount(SessionManager.Instance.Account.UserId, SessionManager.Instance.Account.UserId, SessionManager.Instance.Account.UserId, GameConstants.ACCOUNT_TYPE_ANONYMOUS);

			requestGenerateAnonymousAccount();
		}
	}

	void SubmitDeviceError(object data, int errorCode)
	{
		Debug.Log("SubmitDeviceError result with errorCode " + errorCode);
	}

	//
	private void requestGenerateAnonymousAccount()
	{
		Debug.Log("requestGenerateAnonymousAccount");
		RequestManager.Instance.requestGenerateAnonymousAccount (SessionManager.Instance.Account.UserId, GenerateAnonymousAccountCallback, GenerateAnonymousAccountError);
	}

	void GenerateAnonymousAccountCallback(object data, int errorCode)
	{
		Debug.Log("GenerateAnonymousAccountCallback result with errorCode " + errorCode);
		if (errorCode == ErrorCode.SUCCESS)
		{
			Dictionary<string, object> resultData = (Dictionary<string, object>)data;
			SessionManager.Instance.Account.UserName = (string)resultData[GameConstants.RESPONE_KEY_USER_NAME];	
			SessionManager.Instance.Account.Password = (string)resultData[GameConstants.RESPONE_KEY_PASSWORD];	
			insertAccount(SessionManager.Instance.Account.UserId, SessionManager.Instance.Account.UserName, SessionManager.Instance.Account.Password, GameConstants.ACCOUNT_TYPE_ANONYMOUS);

			requestLogin();
		}
	}

	void GenerateAnonymousAccountError(object data, int errorCode)
	{
		Debug.Log("GenerateAnonymousAccountError result with errorCode " + errorCode);
	}

	//
	private void requestLogin()
	{
		Debug.Log("requestLogin");
		RequestManager.Instance.requestLogin (SessionManager.Instance.Account.UserName, SessionManager.Instance.Account.Password, LoginCallback, LoginError);
	}

	void LoginCallback(object data, int errorCode)
	{
		Debug.Log("requestLoginCallback result with errorCode " + errorCode);
		if (errorCode == ErrorCode.SUCCESS)
		{
			Dictionary<string, object> resultData = (Dictionary<string, object>)data;
			SessionManager.Instance.SessionKey = (string)resultData[GameConstants.RESPONE_KEY_SESSION_KEY];	
			insertSession(SessionManager.Instance.Account.UserId, SessionManager.Instance.SessionKey);

			requestGetFriendPlayedGame(1, 500);
		}
	}

	void LoginError(object data, int errorCode)
	{
		Debug.Log("requestLoginError result with errorCode " + errorCode);
	}

	//
	private void requestSubmitUserScores()
	{
		try 
		{
			StringBuilder json = new StringBuilder ("[");
			string s = GameConstants.JSON_LEVEL_SUBMIT;
			bool isHaveLevelSubmit = false;

			for (int i = 0; i < SessionManager.Instance.UserInfo.Levels.Count; i++)
			{
				LevelEntity levelEntity = SessionManager.Instance.UserInfo.Levels[i + 1];
				if (levelEntity.Replay == 1)
				{
					s = s.Replace("level_submit", "" + levelEntity.Level);
					s = s.Replace("score_submit", "" + levelEntity.Level);
					s = s.Replace("rate_submit", "" + levelEntity.Level);
					json.Append(s);
					json.Append(",");
					isHaveLevelSubmit = true;
				}
			}

			if (isHaveLevelSubmit == true)
			{
				json = json.Remove (json.Length - 1, 1);
				json.Append("]");			
				//RequestManager.Instance.requestSubmitUserScores (SessionManager.Instance.UserInfo.UserId, json.ToString(), SubmitScoreCallback, SubmitScoreError);
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning(ex.Message);
		}
	}

	void SubmitScoreCallback(object data, int errorCode)
	{
		Debug.Log("SubmitScoreCallback result with errorCode " + errorCode);
		if (errorCode == ErrorCode.SUCCESS)
		{
			PlayerPrefs.SetInt("smdv", 1);
			int id = int.Parse(string.Format("{0}", data));
			PlayerPrefs.SetInt("dvid", id);
		}
	}

	void SubmitScoreError(object data, int errorCode)
	{
		Debug.Log("SubmitDeviceError result with errorCode " + errorCode);
		if (errorCode == ErrorCode.SUCCESS)
		{

		}
	}

	//
	private void requestConnectFB()
	{
		Debug.Log("requestConnectFB");
		RequestManager.Instance.requestFacebookConnect (SessionManager.Instance.SessionKey, SessionManager.Instance.UserInfo.AccessToken, ConnectFBCallback, ConnectFBError);
	}

	void ConnectFBCallback(object data, int errorCode)
	{
		Debug.Log("ConnectFBCallback result with errorCode " + errorCode);
		if (errorCode == ErrorCode.SUCCESS)
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

	//
	private void requestLoginFB()
	{
		Debug.Log("requestLoginFB");
		RequestManager.Instance.requestLoginFacebook (SessionManager.Instance.Token, LoginFBCallback, LoginFBError);
	}

	void LoginFBCallback(object data, int errorCode)
	{
		Debug.Log("LoginFBCallback result with errorCode " + errorCode);
		if (errorCode == ErrorCode.SUCCESS)
		{
			Dictionary<string, object> resultData = (Dictionary<string, object>)data;
			SessionManager.Instance.SessionKey = (string)resultData[GameConstants.RESPONE_KEY_SESSION_KEY];	
			insertSession(SessionManager.Instance.Account.UserId, SessionManager.Instance.SessionKey);
			requestGetFriendPlayedGame(1, 500);
		}
	}

	void LoginFBError(object data, int errorCode)
	{
		Debug.Log("LoginFBError result with errorCode " + errorCode);
	}

	//
	private void requestGetHighScores(int level)
	{
		if (HighScores.ContainsKey(level) == false && !Helpers.IsEmpty(SessionManager.Instance.SessionKey))
			RequestManager.Instance.requestGetHighScores(SessionManager.Instance.SessionKey, "" + level, GetHighScoresCallback, GetHighScoresError);
	}

	void GetHighScoresCallback(object data, int errorCode)
	{
		Debug.Log("GetHighScoresCallback result with errorCode " + errorCode);
		if (errorCode == ErrorCode.SUCCESS)
		{
			try
			{
				List<FriendEntity> resultData = (List<FriendEntity>)data;
				if (resultData != null)
				{
					List<FriendEntity> friends = new List<FriendEntity>();
					int level = 1;
					foreach(FriendEntity obj in resultData)
					{
						friends.Add(obj);
						level = obj.Level;

						Debug.Log("level result = " + level);
					}
					HighScores[level] = friends;
					resultData.Clear();

					//FriendPopupController.Instance.SetListFriends(friends);
				}
			}
			catch(Exception ex)
			{
				Debug.LogWarning("GetHighScoresCallback result " + ex.ToString());
			}
		}
	}

	void GetHighScoresError(object data, int errorCode)
	{
		Debug.Log("GetHighScoresError result with errorCode " + errorCode);
	}

	//
	private void requestGetFriendPlayedGame(int page, int count)
	{
		RequestManager.Instance.requestGetFriendPlayedGame (SessionManager.Instance.SessionKey, "" + page, "" + count, GetFriendPlayedGameCallback, GetFriendPlayedGameError);
	}

	void GetFriendPlayedGameCallback(object data, int errorCode)
	{
		deleteAllFriends ();//Avoid duplicated entries
		Debug.Log("GetFriendPlayedGameCallback result with errorCode " + errorCode);
		if (errorCode == ErrorCode.SUCCESS)
		{
			try
			{
				FriendLevels.Clear();
				List<FriendEntity> resultData = (List<FriendEntity>)data;
				if (resultData != null)
				{
					List<FriendEntity> friends = new List<FriendEntity>();
					int level = 1;
					foreach(FriendEntity friend in resultData)
					{
						level = friend.Level;					

						if (FriendLevels.ContainsKey(friend.Level) == false) 
							FriendLevels[friend.Level] = new List<FriendEntity>();

						friends.Add(friend);
						if (FriendLevels[friend.Level].Contains(friend)) {
							var i = FriendLevels[friend.Level].FindIndex(x=> x == friend);
							if (i != -1)
								FriendLevels[friend.Level][i] = friend;
						} else {
							FriendLevels[friend.Level].Add(friend);
						}
					}

					resultData.Clear();	
					if (friends != null)
					{
						insertFriendLevels(friends);
						SessionManager.Instance.UserInfo.FriendLevels = FriendLevels;

						if (instance.GameScene == (int)Scene.MAP) 
						{
							DataLoader.Data.SetFriendProgress ();
						}
					}
				}
			}
			catch(Exception ex)
			{
				Debug.LogWarning("GetFriendPlayedGameCallback result " + ex.ToString());
			}
		}
	}

	void GetFriendPlayedGameError(object data, int errorCode)
	{
		Debug.Log("GetFriendPlayedGameError result with errorCode " + errorCode);
	}	

	//
	private void requestGetFriendLevels(int page, int count)
	{
		//		RequestManager.Instance.getFriendLevels (SessionManager.Instance.UserInfo.UserId, "" + page, "" + count, RequestGetFriendLevelsCallback);
	}

	void RequestGetFriendLevelsCallback(object data, int errorCode)
	{
		//		GameUtil.Log("GetFriendLevelsCallback result with errorCode " + errorCode);
		//		if (errorCode == ErrorCode.SUCCESS)
		//		{
		//			try
		//			{
		//				List<object> resultData = (List<object>)data;
		//				List<FriendEntity> friends = new List<FriendEntity>();
		//				if (resultData != null)
		//				{
		//					float currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
		//					PlayerPrefs.SetFloat("ltgfl", currentTime); // last time get friend level
		//					foreach(object obj in resultData)
		//					{
		//						try 
		//						{
		//							FriendEntity friend = new FriendEntity();
		//							friend.UserId = (((Dictionary<string, object>)obj)["uid"]).ToString();
		//							friend.Level = int.Parse(string.Format("{0}", ((Dictionary<string, object>)obj)["level"]));
		//							
		//							
		//							if (FriendLevels[friend.Level] == null)
		//								FriendLevels[friend.Level] = new List<FriendEntity>();
		//							friends.Add(friend);
		//							FriendLevels[friend.Level].Add(friend);
		//						}
		//						catch(Exception ex)
		//						{
		//							GameUtil.LogError("GetFriendLevelsCallback result " + ex.ToString());
		//						}
		//					}
		//					//SendInvite();
		//					resultData.Clear();
		//					
		//					if (friends.Count > 0)
		//					{
		//						deleteFriendLevels();
		//						insertFriendLevels(friends);
		//					}
		//				}				
		//			}
		//			catch(Exception ex)
		//			{
		//				GameUtil.LogError("GetFriendLevelsCallback result " + ex.ToString());
		//			}
		//		}
	}

	//
	private void requestGetRequestingList()
	{
		RequestManager.Instance.requestGetRequestingList ("", RequestGetRequestingListCallback, RequestGetRequestingListError);
	}

	void RequestGetRequestingListCallback(object data, int errorCode)
	{
		Debug.Log("RequestGetRequestingListCallback result with errorCode " + errorCode);
		if (errorCode == ErrorCode.SUCCESS)
		{
			try
			{
				List<object> resultData = (List<object>)data;
				if (resultData != null)
				{
					foreach(object obj in resultData)
					{
						try 
						{
							FbRequestEntity fbRequestEntity = new FbRequestEntity();
							fbRequestEntity.UserIdTo = (((Dictionary<string, object>)obj)["uidt"]).ToString();
							fbRequestEntity.UserIdFrom = (((Dictionary<string, object>)obj)["uidf"]).ToString();
							fbRequestEntity.Type = int.Parse(string.Format("{0}", ((Dictionary<string, object>)obj)["type"]));
							fbRequestEntity.RequestId = int.Parse(string.Format("{0}", ((Dictionary<string, object>)obj)["id"]));

							FBRequests.Add(fbRequestEntity);
						}
						catch(Exception ex)
						{
							Debug.LogError("RequestGetRequestingListCallback result " + ex.ToString());
						}
					}
					resultData.Clear();
				}
			}
			catch(Exception ex)
			{
				Debug.LogWarning("RequestGetRequestingListCallback result " + ex.ToString());
			}
		}
	}

	void RequestGetRequestingListError(object data, int errorCode)
	{
		Debug.Log("RequestGetRequestingListError result with errorCode " + errorCode);
	}

	//
	private void requestSubmitRequestAccept(string requestIds)
	{
		RequestManager.Instance.requestSubmitRequestAccept(SessionManager.Instance.UserInfo.UserId, requestIds, SubmitRequestAcceptCallback, SubmitRequestAcceptError);
	}

	void SubmitRequestAcceptCallback(object data, int errorCode)
	{
		Debug.Log("SubmitRequestAcceptCallback result with errorCode " + errorCode);
		if (errorCode == ErrorCode.SUCCESS)
		{

		}
	}

	void SubmitRequestAcceptError(object data, int errorCode)
	{
		Debug.Log("SubmitRequestAcceptError result with errorCode " + errorCode);		
	}

	//
	private void requestSubmitRequestSend(string uids)
	{
		RequestManager.Instance.requestSendRequestFB(SessionManager.Instance.UserInfo.UserId, uids, "", SubmitRequestSendCallback, SubmitRequestSendError);
	}

	void SubmitRequestSendCallback(object data, int errorCode)
	{
		Debug.Log("SubmitRequestSendCallback result with errorCode " + errorCode);
		if (errorCode == ErrorCode.SUCCESS)
		{

		}
	}

	void SubmitRequestSendError(object data, int errorCode)
	{
		Debug.Log("SubmitRequestSendError result with errorCode " + errorCode);		
	}

	//
	private void requestShareStory(string type, string data)
	{
		RequestManager.Instance.requestShareStory(SessionManager.Instance.UserInfo.UserId, type, data, ShareStoryCallback, ShareStoryError);
	}

	void ShareStoryCallback(object data, int errorCode)
	{
		Debug.Log("ShareStoryCallback result with errorCode " + errorCode);
		if (errorCode == ErrorCode.SUCCESS)
		{

		}
	}

	void ShareStoryError(object data, int errorCode)
	{
		Debug.Log("ShareStoryError result with errorCode " + errorCode);

	}

	public UserEntity CurrentUserInfo{
		get {
			return SessionManager.Instance.UserInfo;
		}
	}

	private float doubleClickStart = 0;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{			
			if ( Time.time - doubleClickStart < 0.3f)
			{ //double click
				Application.Quit();
			}
			else {
				/*int currentLevel = Application.loadedLevel;
				Debug.Log("Current Level is: " + currentLevel);
				int previous = currentLevel-1;
				if (previous >= 0){
					Application.LoadLevel(previous);
				}*/
				doubleClickStart = Time.time;
			}

		}
	}



}
