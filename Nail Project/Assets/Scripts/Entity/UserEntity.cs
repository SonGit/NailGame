using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace com.b2mi.dc.Entity
{
	public class UserEntity
	{
		private string avatar;
		private string name;
		private string userid;
		private string accessToken;
		private Dictionary<int, LevelEntity> levels;
		private LevelEntity currentLevel;
		private LevelEntity highestLevelEntity;
		private Dictionary<int, List<FriendEntity>> friendLevels;

		private int lives;
		private int gold;
		private int droplet;
		private int highestLevel;
		private Texture userTexture;
		
		public UserEntity()
		{
			avatar = "";
			name = "";
			userid = "";
			accessToken = "";
			levels = new Dictionary<int, LevelEntity>(0);
		}
		
		public int Lives
		{
			get{ return lives;  }
			set{ lives = value; }
		}
		
		public int Gold
		{
			get{ return gold;  }
			set{ gold = value; }
		}
		
		public int Droplet
		{
			get{ return droplet;  }
			set{ droplet = value; }
		}
		
		public int HighestLevel
		{
			get{ return highestLevel;  }
			set{ highestLevel = value; }
		}
		
		public string UserId
		{
			get { return userid;}
			set { userid = value;}
		}
		
		public string Avatar
		{
			get { return avatar;}
			set { avatar = value;}
		}
		
		public string Name
		{
			get { return name;}
			set { name = value;}
		}
		
		public string AccessToken
		{
			get { return accessToken;}
			set { accessToken = value;}
		}
		
		public Dictionary<int, LevelEntity> Levels
		{
			get { return levels;}
			set { levels = value;}
		}

		public  Dictionary<int, List<FriendEntity>> FriendLevels
		{
			get { return friendLevels;}
			set { friendLevels = value;}
		}
		
		public Texture UserTexture
		{
			get { return userTexture;}
			set { userTexture = value;}
		}
		
		public LevelEntity CurrentLevel
		{
			get { return currentLevel;}
			set { currentLevel = value;}
		}

		public LevelEntity HighestLevelEntity
		{
			get { return highestLevelEntity;}
			set { highestLevelEntity = value;}
		}
	}
}
