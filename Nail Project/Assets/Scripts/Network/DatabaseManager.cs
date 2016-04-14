//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using UnityEngine;

using System.Collections;
using System.IO;
using SQLite4Unity3d;
using System.Collections.Generic;
using System.Text;
using System;
using System.Threading;
using com.b2mi.dc.Database;
using com.b2mi.dc.Security;
using com.b2mi.dc.Entity;

public class DatabaseManager
{
	public const string LEVEL 						= "Level";
	public const string SCORE 						= "Score";
	public const string RATE 						= "Rate";
	public const string REPLAY 						= "Replay";
	public const string USER_ID 					= "UserId";
	public const string SESSIONKEY 					= "SessionKey";
	public const string USER_NAME 					= "UserName";
	public const string PASSWORD 					= "Password";
	public const string ACCOUNT_TYPE 				= "account_type";
	public const string FRIEND_INFO 				= "FriendInfo";


	public class LevelTable
	{
		public int Id { get; set; }
		[PrimaryKey]
		public string Level { get; set; }
		public string Score { get; set; }
		public string Rate { get; set; }
		public string Replay { get; set; }
	}

	public class AcountTable
	{
		public int Id { get; set; }
		[PrimaryKey]
		public string UserId { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string AccountType { get; set; }
	}

	public class SessionTable
	{
		public int Id { get; set; }
		[PrimaryKey]
		public string UserId { get; set; }
		public string SessionKey { get; set; }
	}

	public class FriendTable 
	{
		public int Id { get; set; }
		[PrimaryKey]
		public string UserId { get; set; }
		public string DisplayName { get; set; }			
		public string Avatar { get; set; }
		public string Gender { get; set; }
		public string Score { get; set; }
		public int Level { get; set; }
		public string FbId { get; set; }
	}

	private static volatile DatabaseManager instance 		= null;
	private static object singletonLock 					= new object();
	private ISQLiteConnection connection;

	public void createTable ()
	{
		/* create table */
		connection.CreateTable<LevelTable> ();
		connection.CreateTable<AcountTable> ();
		connection.CreateTable<SessionTable> ();
	}
	
	/**
	 * Singleton methods
	 */	
	public DatabaseManager ()
	{
//		sqlDB = new SqliteDatabase ("config12.db");
		if (DBCallbackDispatcher.Singleton == null )
		{
			DBCallbackDispatcher.Init();
		}
		var factory = new Factory();
		string DatabaseName = "config.db";
		#if UNITY_EDITOR
		DatabaseName = "configEditor3.db";
		var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
		#else
		// check if file exists in Application.persistentDataPath
		var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);
		if (!File.Exists(filepath))
		{
			Debug.Log("Database not in Persistent path");
			// if it doesn't ->
			// open StreamingAssets directory and load the db ->
			
			#if UNITY_ANDROID 
			var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
			while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
			// then save to Application.persistentDataPath
			File.WriteAllBytes(filepath, loadDb.bytes);
			#elif UNITY_IOS
			var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
			// then save to Application.persistentDataPath
			// File.Copy(loadDb, filepath);
			#elif UNITY_WP8
			var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
			// then save to Application.persistentDataPath
			// File.Copy(loadDb, filepath);
			
			#elif UNITY_WINRT
			var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
			// then save to Application.persistentDataPath
			File.Copy(loadDb, filepath);
			#endif
			
			Debug.Log("Database written");
		}
		
		var dbPath = filepath;
		#endif
		connection = factory.Create(dbPath);
	}
	
	public static DatabaseManager Instance {
		get
		{
			if ( instance == null )
			{
				lock( singletonLock )
				{				
					instance = new DatabaseManager();
				}
			}
			
			return instance; 
		}
	}

	public void Init()
	{
		createTable ();
	}

	public void Close()
	{
		if (this.connection != null) {
			try {
				this.connection.Close();
			}
			catch (Exception ex) {
				Debug.LogWarning (ex.Message);
			}
		}
	}

	/**
	 * InsertLevel Methods
	 */
	public void InsertLevel (int level, int score, int rate, int replay)
	{
		Hashtable hash = new Hashtable ();
		hash [DatabaseManager.LEVEL] = level;
		hash [DatabaseManager.SCORE] = score;
		hash [DatabaseManager.RATE] = rate;
		hash [DatabaseManager.REPLAY] = replay;
		ThreadPool.QueueUserWorkItem (insertLevel, hash);
	}
	
	void insertLevel (object obj)
	{
		lock (singletonLock)
		{
			try
			{
				Hashtable hash = (Hashtable)obj;
				string encryptLevel = Encryption.Encrypt ("" + hash [DatabaseManager.LEVEL]);
				string encryptScore = Encryption.Encrypt ("" + hash [DatabaseManager.SCORE]);
				string encryptRate = Encryption.Encrypt ("" + hash [DatabaseManager.RATE]);
				string encryptReplay = Encryption.Encrypt ("" + hash [DatabaseManager.REPLAY]);

				var p = new LevelTable{
					Level = encryptLevel,
					Score = encryptScore,
					Rate = encryptRate,
					Replay = encryptReplay
				};
				int isSuccess = connection.Update(p);
				if (isSuccess == 0 )
				{
					connection.Insert(p);
				}
			}
			catch(Exception ex)
			{
				Debug.LogWarning (ex.Message);
			}
		}
	}

	/**
	 * GetLevels Methods
	 */
	public void GetLevels (Action<DBCallback> callback)
	{
		ThreadPool.QueueUserWorkItem (getLevels, callback);
	}  

	void getLevels (object obj)
	{
		try
		{
			Action<DBCallback> callback = (Action<DBCallback>)obj;
			Dictionary<int, LevelEntity> levels = new Dictionary<int, LevelEntity> ();
			IEnumerable<LevelTable> dataTable = connection.Table<LevelTable>();
			foreach (LevelTable item in dataTable)
			{
				LevelEntity levelEntity = new LevelEntity ();				
				string levelDecrypt = Encryption.Decrypt (item.Level);
				string scoreDecrypt = Encryption.Decrypt (item.Score);
				string rateDecrypt = Encryption.Decrypt (item.Rate);
				string replayDecrypt = Encryption.Decrypt (item.Replay);

				try {
					levelEntity.Level = Convert.ToInt32 (levelDecrypt);
					levelEntity.Score = Convert.ToInt32 (scoreDecrypt);
					levelEntity.Rate = Convert.ToInt32 (rateDecrypt);
					levelEntity.Replay = Convert.ToInt32 (replayDecrypt);
					
					// Add Score Entity to Score List
					levels[levelEntity.Level] = levelEntity;
				} catch (Exception ex) {
					Debug.LogException (ex);
				}
			}
			Debug.LogWarning("getLevels");
			DBCallback dbCallback = new DBCallback ();
			dbCallback.Data = levels;
			dbCallback.completedCallback = callback;
			DBCallbackDispatcher.Singleton.requests.Enqueue (dbCallback);
		}
		catch(Exception ex)
		{
			Debug.LogException (ex);
		}
	}

	/**
	 * InsertAccount Methods
	 */
	public void InsertAccount (string userId, string userName, string password, string accountType)
	{
		Hashtable hash = new Hashtable ();
		hash [DatabaseManager.USER_ID] = userId;
		hash [DatabaseManager.USER_NAME] = userName;
		hash [DatabaseManager.PASSWORD] = password;
		hash [DatabaseManager.ACCOUNT_TYPE] = accountType;

		ThreadPool.QueueUserWorkItem (insertAccount, hash);
	}
	
	void insertAccount (object obj)
	{
		lock (singletonLock)
		{
			try
			{
				//deleteAllAccount();

				Hashtable hash = (Hashtable)obj;
				string encryptUserId = Encryption.Encrypt ("" + hash [DatabaseManager.USER_ID]);
				string encryptUserName = Encryption.Encrypt ("" + hash [DatabaseManager.USER_NAME]);
				string encryptPassword = Encryption.Encrypt ("" + hash [DatabaseManager.PASSWORD]);
				string encryptAccountType = Encryption.Encrypt ("" + hash [DatabaseManager.ACCOUNT_TYPE]);

				var p = new AcountTable{
					UserId = encryptUserId,
					UserName = encryptUserName,
					Password = encryptPassword,
					AccountType = encryptAccountType
				};
				int isSuccess = connection.Update(p);
				if (isSuccess == 0 )
				{
					connection.Insert(p);
				}
			}
			catch(Exception ex)
			{
				Debug.LogWarning (ex.Message);
			}
		}
	}

	/**
	 * GetAccount Methods
	 */
	public void GetAccount (Action<DBCallback> callback)
	{
		ThreadPool.QueueUserWorkItem (getAccount, callback);
	}
	
	void getAccount (object obj)
	{
		lock (singletonLock)
		{
			try
			{
				Action<DBCallback> callback = (Action<DBCallback>)obj;
				AccountEntity  accountEntity = new AccountEntity ();
				IEnumerable<AcountTable> dataTable = connection.Table<AcountTable>();
				foreach (AcountTable item in dataTable)
				{
					string userId = Encryption.Decrypt (item.UserId);
					string userName = Encryption.Decrypt (item.UserName);
					string password = Encryption.Decrypt (item.Password);
					string accountType = Encryption.Decrypt (item.AccountType);
					
					try {
						accountEntity.UserId = userId;
						accountEntity.UserName = userName;
						accountEntity.Password = password;
						accountEntity.AccountType = accountType;						
					} catch (Exception ex) {
						Debug.LogException (ex);
					}
					break;
				}
				
				DBCallback dbCallback = new DBCallback ();
				dbCallback.Data = accountEntity;
				dbCallback.completedCallback = callback;
				DBCallbackDispatcher.Singleton.requests.Enqueue (dbCallback);
			}
			catch(Exception ex)
			{
				Debug.LogWarning (ex.Message);
			}
		}
	}

	public void deleteAllAccount() 
	{
		IEnumerable<AcountTable> dataTable = connection.Table<AcountTable>();
		foreach (AcountTable item in dataTable)
		{
			connection.Delete(item);
			break;
		}
	}

	/**
	 * InsertSession Methods
	 */
	public void InsertSession (string userId, string sessionKey)
	{
		Hashtable hash = new Hashtable ();
		hash [DatabaseManager.SESSIONKEY] = sessionKey;
		hash [DatabaseManager.USER_ID] = userId;

		ThreadPool.QueueUserWorkItem (insertSession, hash);
	}
	
	void insertSession (object obj)
	{
		lock (singletonLock)
		{
			try
			{
				deleteAllSession();

				Hashtable hash = (Hashtable)obj;
				string encryptSessionKey = Encryption.Encrypt ("" + hash [DatabaseManager.SESSIONKEY]);
				string encryptUserId = Encryption.Encrypt ("" + hash [DatabaseManager.USER_ID]);

				var p = new SessionTable{
					UserId = encryptUserId,
					SessionKey = encryptSessionKey
				};
				int isSuccess = connection.Update(p);
				if (isSuccess == 0 )
				{
					connection.Insert(p);
				}
			}
			catch(Exception ex)
			{
				Debug.LogWarning (ex.Message);
			}
		}
	}
	
	/**
	 * GetSessionKey Methods
	 */
	public void GetSessionKey (Action<DBCallback> callback)
	{
		ThreadPool.QueueUserWorkItem (getSessionKey, callback);
	}
	
	void getSessionKey (object obj)
	{
		lock (singletonLock)
		{
			try
			{
				Action<DBCallback> callback = (Action<DBCallback>)obj;
				string sessionKey = "";
				IEnumerable<SessionTable> dataTable = connection.Table<SessionTable>();
				foreach (SessionTable item in dataTable)
				{
					sessionKey = Encryption.Decrypt (item.SessionKey);
					break;
				}
				
				DBCallback dbCallback = new DBCallback ();
				dbCallback.Data = sessionKey;
				dbCallback.completedCallback = callback;
				DBCallbackDispatcher.Singleton.requests.Enqueue (dbCallback);
			}
			catch(Exception ex)
			{
				Debug.LogWarning (ex.Message);
			}
		}
	}

	public void deleteAllSession() 
	{
		IEnumerable<SessionTable> dataTable = connection.Table<SessionTable>();
		foreach (SessionTable item in dataTable)
		{
			connection.Delete(item);
			break;
		}
	}

	/**
	 * InsertSession Methods
	 */
	public void InsertFriends (List<FriendEntity> friends)
	{
		Hashtable hash = new Hashtable ();
		hash [DatabaseManager.FRIEND_INFO] = friends;

		ThreadPool.QueueUserWorkItem (insertFriends, hash);
	}
	
	void insertFriends (object obj)
	{
		lock (singletonLock)
		{
			try
			{
				deleteAllFriends();

				Hashtable hash = (Hashtable)obj;
				List<FriendEntity> friends = (List<FriendEntity>) hash[DatabaseManager.FRIEND_INFO];

				for (int i = 0; i < friends.Count; i++)
				{
					var p = new FriendTable{
						UserId = friends[i].UserId,
						DisplayName = friends[i].Name,
						Avatar = friends[i].Avatar,
						Gender = friends[i].Gender,
						Level = friends[i].Level,
						Score = friends[i].Score,
						FbId = friends[i].FBId
					};
					int isSuccess = connection.Update(p);
					if (isSuccess == 0 )
					{
						connection.Insert(p);
					}
				}
			}
			catch(Exception ex)
			{
				Debug.LogWarning (ex.Message);
			}
		}
	}
	
	/**
	 * GetSessionKey Methods
	 */
	public void GetFriends (Action<DBCallback> callback)
	{
		ThreadPool.QueueUserWorkItem (getFriends, callback);
	}
	
	void getFriends (object obj)
	{
		lock (singletonLock)
		{
			try
			{
				Action<DBCallback> callback = (Action<DBCallback>)obj;
				List<FriendEntity> friends = new List<FriendEntity>();
				IEnumerable<FriendTable> dataTable = connection.Table<FriendTable>();
				foreach (FriendTable item in dataTable)
				{
					FriendEntity friend = new FriendEntity();
					friend.UserId = item.UserId;
					friend.Name = item.DisplayName;
					friend.Avatar = item.Avatar;
					friend.Gender = item.Gender;
					friend.Level = item.Level;
					friend.Score = item.Score;
					friend.FBId = item.FbId;

					friends.Add(friend);
				}
				
				DBCallback dbCallback = new DBCallback ();
				dbCallback.Data = friends;
				dbCallback.completedCallback = callback;
				DBCallbackDispatcher.Singleton.requests.Enqueue (dbCallback);
			}
			catch(Exception ex)
			{
				Debug.LogWarning (ex.Message);
			}
		}
	}
	
	public void deleteAllFriends() 
	{
		IEnumerable<FriendTable> dataTable = connection.Table<FriendTable>();
		foreach (FriendTable item in dataTable)
		{
			connection.Delete(item);
			break;
		}
	}
}


