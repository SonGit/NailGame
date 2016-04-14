using UnityEngine;
using System.Collections;
namespace com.b2mi.dc.Entity
{
	public class FriendEntity 
	{		
		private string avatar;
		private string name;
		private string userid;
		private string gender;
		private string score;
		private string fbId;
		private int level;
		
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
		
		public string Gender
		{
			get { return gender;}
			set { gender = value;}
		}
		
		public int Level
		{
			get { return level;}
			set { level = value;}
		}

		public string Score
		{
			get { return score;}
			set { score = value;}
		}

		public string FBId
		{
			get { return fbId;}
			set { fbId = value;}
		}

		public int CompareTo(FriendEntity other)
		{
			// Alphabetic sort if salary is equal. [A to Z]
			if (this.UserId == other.UserId)
			{
				return 0;
			}
			// Default to salary sort. [High to low]
			return -1;
		}
		
		public override string ToString()
		{
			// String representation.
			return this.UserId.ToString();
		}
	}
}
