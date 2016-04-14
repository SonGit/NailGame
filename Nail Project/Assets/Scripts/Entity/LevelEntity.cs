using UnityEngine;
using System.Collections;

namespace com.b2mi.dc.Entity
{
	public class LevelEntity 
	{
		private int level;
		private int score;
		private int rate;
		private int replay;
		
		public int Level
		{
			get{ return level;  }
			set{ level = value; }
		}
		
		public int Score
		{
			get{ return score;  }
			set{ score = value; }
		}
		
		public int Rate
		{
			get{ return rate;  }
			set{ rate = value; }
		}
		
		public int Replay
		{
			get{ return replay;  }
			set{ replay = value; }
		}
		
		public void Reset() 
		{
			level = 0;
			rate = 0;
			score = 0;
		}
		
		public void UpdateLevel(LevelEntity levelEntity)
		{
			score = levelEntity.Score;
			rate = levelEntity.Rate;
			level = levelEntity.Level;
		}		
	}
}
