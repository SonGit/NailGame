using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {
	public Texture2D sourceTex;

	int WIDTH = 7;
	int HEIGHT = 9;

	int[] tiles ;
	int[] blockers = new int[]
	{
		44,
		0,
		45
	};

	int currentDifficulty;
	// Use this for initialization
	void Start() {

		tiles = new int[ WIDTH * HEIGHT];
		tiles = Pattern_1;

		for (int i = 0; i < WIDTH; i ++) {
			for(int j = 0 ; j < HEIGHT ; j++)
			{
				Color pix = sourceTex.GetPixel (i,j);
			}
		}

		patterns_easy = new List<int[]>()
		{
			Pattern_1,
			Pattern_2,
			Pattern_3,
			Pattern_4,
			Pattern_5,
			Pattern_6,
			Pattern_7,
			Pattern_8,
			Pattern_9,
			Pattern_10,
			Pattern_11,
			Pattern_12,
			Pattern_13,
			Pattern_14
		};

		patterns_hard = new List<int[]>()
		{
			Pattern_H_1,
			Pattern_H_2,
			Pattern_H_3,
			Pattern_H_4,
			Pattern_H_5,
			Pattern_H_6,
			Pattern_H_7,
		};

		Test ();
	}

	void Test()
	{
		List<int[]> maps = new List<int[]>();

		int very_easy = 10;
		int easy = 5;
		int medium = 5;
		int hard = 5;

		for( int i = 0 ; i < very_easy ; i++)
		{
			//maps.Add( Make_VeryEasy() );
		}

		for( int i = 0 ; i < easy ; i++)
		{
			maps.Add( Make_Easy() );
		}

		for( int i = 0 ; i < easy ; i++)
		{
			//maps.Add( Make_Easy_2() );
		}

		for( int i = 0 ; i < easy ; i++)
		{
			//maps.Add( Make_Easy_3() );
		}

		for( int i = 0 ; i < medium ; i++)
		{
			//maps.Add( Make_Medium() );
		}

		for( int i = 0 ; i < medium ; i++)
		{
			//maps.Add( Make_Medium_2() );
		}

		for( int i = 0 ; i < hard ; i++)
		{
			//maps.Add( Make_Hard() );
		}

		for( int i = 0 ; i < hard ; i++)
		{
			//maps.Add( Make_Hard_2() );
		}

		int count = 1;
		foreach(int[] map in maps)
		{
			Export( map , count.ToString());
			count ++;
		}

	}

	int[] Make_VeryEasy()
	{
		int rand = Random.Range (0,patterns_easy.Count);
		return patterns_easy[rand];
	}

	int[] Make_Easy()
	{
		int rand1 = Random.Range (0,patterns_easy.Count);
		int rand2 = Random.Range (0,patterns_easy.Count);

		while (rand1 == rand2) {
			rand2 = Random.Range (0,patterns_easy.Count);
		}

		return CombinePattern ( patterns_easy[rand1],patterns_easy[rand2] );
	}

	int[] Make_Easy_2()
	{
		int rand1 = Random.Range (0,patterns_easy.Count);
		int rand2 = Random.Range (0,patterns_easy.Count);
		
		while (rand1 == rand2) {
			rand2 = Random.Range (0,patterns_easy.Count);
		}
		
		return CombinePattern ( patterns_easy[rand1],SetBlockers(patterns_easy[rand2],45)  );
	}

	int[] Make_Easy_3()
	{
		int rand1 = Random.Range (0,patterns_easy.Count);
		int rand2 = Random.Range (0,patterns_easy.Count);
		
		while (rand1 == rand2) {
			rand2 = Random.Range (0,patterns_easy.Count);
		}
		
		return CombinePattern ( patterns_easy[rand1],SetBlockers(patterns_easy[rand2],0)  );
	}

	int[] Make_Medium()
	{
		int rand = Random.Range (0,patterns_hard.Count);
		return SetBlockers (patterns_hard [rand], 45);
	}

	int[] Make_Medium_2()
	{
		int rand = Random.Range (0,patterns_hard.Count);
		return SetBlockers (patterns_hard [rand], 0);
	}

	int[] Make_Hard()
	{
		int rand1 = Random.Range (0,patterns_hard.Count);
		int rand2 = Random.Range (0,patterns_easy.Count);

		return CombinePattern ( SetBlockers(patterns_hard[rand1],45),patterns_easy[rand2] );
	}

	int[] Make_Hard_2()
	{
		int rand1 = Random.Range (0,patterns_hard.Count);
		int rand2 = Random.Range (0,patterns_easy.Count);
		
		return CombinePattern (  SetBlockers(patterns_hard[rand1],0),patterns_easy[rand2] );
	}

	int[] CombinePattern( int[] pattern1 , int[] pattern2 )
	{
		for (int i = 0; i< pattern1.Length; i++) {
			if(pattern1[i] == 0)
			{
				pattern1[i] = pattern2[i];
			} 
		}
		return pattern1;
	}

	int[] SetBlockers(int[] pattern,int type)
	{
		for (int i = 0; i< pattern.Length; i++) {
			if(pattern[i] == 0)
			{
				pattern[i] = type;
			} 
		}
		return pattern;
	}

	int[] Parser(int[] pattern)
	{
		for (int i = 0; i < pattern.Length; i ++) {

			if( pattern[i] == 0)
			{
				pattern[i] = 2; //2 means normal cell
			}

			if( pattern[i] == 1)
			{
				pattern[i] = 45;
			}
			
		}

		return pattern;
	}

	void Export( int[] tiles , string name)
	{
		tiles = Parser(tiles);

		var sr = File.CreateText("\\Nail Project\\Nail Project\\Assets\\GeneratedMap\\" + name +".txt");

		int lineCount = 0;
		for (int i = 0; i < tiles.Length; i ++) {

			if(lineCount == 6)
			{
				//print (tiles[i] + "break");
 				//print (tiles[i]);
				//sr.Write(tiles[i] + "\t");
				sr.Write(tiles[i] + "\r\n");
				lineCount = 0;
			}
			else
			{
				//print (tiles[i]);
				//sr.Write(tiles[i] + "\r\n");
				sr.Write(tiles[i] + "\t");
				lineCount ++;
			}


	
			//sr.Write(tiles[i] + "\t");
			//print (tiles[i] );
			//sr.Write(tiles[i] + "\r\n");
		}
		sr.Close();

	}

	List<int[]> patterns_easy ;
	List<int[]> patterns_hard ;
	//Template
	int[] template = new int[]
	{
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
	};

	//Easy patterns
	int[] Pattern_1 = new int[]
	{
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 1 , 0 , 0 , 0,
		0 , 0 , 0 , 1 , 0 , 0 , 0,
		0 , 0 , 0 , 1 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
	};

	int[] Pattern_2 = new int[]
	{
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 1 , 1 , 1 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
	};

	int[] Pattern_3 = new int[]
	{
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 1 , 0 , 0 , 0,
		0 , 0 , 1 , 0 , 1 , 0 , 0,
		0 , 0 , 0 , 1 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
	};

	int[] Pattern_4 = new int[]
	{
		0 , 1 , 0 , 0 , 0 , 1 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 1 , 0 , 0 , 0 , 1 , 0,
	};

	int[] Pattern_5 = new int[]
	{
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 1 , 0 , 1 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 1 , 0 , 1 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
	};

	int[] Pattern_6 = new int[]
	{
		1 , 0 , 0 , 0 , 0 , 0 , 1,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		1 , 0 , 0 , 0 , 0 , 0 , 1,
	};

	int[] Pattern_7 = new int[]
	{
		1 , 1 , 0 , 0 , 0 , 1 , 1,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
	};

	int[] Pattern_8 = new int[]
	{
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		1 , 1 , 0 , 0 , 0 , 1 , 1,
	};

	int[] Pattern_9 = new int[]
	{
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 1 , 0 , 1 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 1 , 0 , 1 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
	};

	int[] Pattern_10 = new int[]
	{
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 1 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 1 , 0 , 0,
		0 , 1 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 1 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
	};

	int[] Pattern_11 = new int[]
	{
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		1 , 0 , 0 , 0 , 0 , 0 , 1,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		1 , 0 , 0 , 0 , 0 , 0 , 1,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
	};

	int[] Pattern_12 = new int[]
	{
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		1 , 0 , 0 , 0 , 0 , 0 , 1,
		1 , 0 , 0 , 0 , 0 , 0 , 1,
		1 , 0 , 0 , 0 , 0 , 0 , 1,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
	};

	int[] Pattern_13 = new int[]
	{
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		1 , 0 , 0 , 0 , 0 , 0 , 1,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		1 , 0 , 0 , 0 , 0 , 0 , 1,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
	};

	int[] Pattern_14 = new int[]
	{
		0 , 0 , 0 , 1 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 1 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 1 , 0 , 0 , 0,
	};

	int[] Pattern_H_1 = new int[]
	{
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 1 , 0 , 1 , 0 , 0 , 0,
		0 , 0 , 1 , 0 , 0 , 0 , 0,
		0 , 1 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 1 , 0 , 0,
		0 , 0 , 0 , 1 , 0 , 0 , 0,
		0 , 0 , 1 , 0 , 1 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
	};

	int[] Pattern_H_2 = new int[]
	{
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 1 , 0 , 0 , 0,
		0 , 0 , 1 , 0 , 1 , 0 , 0,
		0 , 1 , 0 , 0 , 0 , 1 , 0,
		0 , 0 , 1 , 0 , 1 , 0 , 0,
		0 , 0 , 0 , 1 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
	};

	int[] Pattern_H_3 = new int[]
	{
		1 , 0 , 0 , 1 , 0 , 0 , 1,
		0 , 1 , 0 , 1 , 0 , 1 , 0,
		0 , 0 , 1 , 1 , 1 , 0 , 0,
		0 , 0 , 0 , 1 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
	};

	int[] Pattern_H_4 = new int[]
	{
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 1 , 0 , 1 , 0 , 0,
		0 , 1 , 0 , 0 , 0 , 1 , 0,
		0 , 0 , 1 , 0 , 1 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
	};

	int[] Pattern_H_5 = new int[]
	{
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 1 , 1 , 1 , 0 , 0,
		0 , 0 , 1 , 0 , 1 , 0 , 0,
		0 , 0 , 1 , 1 , 1 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
	};

	int[] Pattern_H_6 = new int[]
	{
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 1 , 1 , 1 , 0 , 0,
		0 , 0 , 1 , 1 , 1 , 0 , 0,
		0 , 0 , 1 , 1 , 1 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
	};

	int[] Pattern_H_7 = new int[]
	{
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 0 , 1 , 0 , 0 , 0,
		0 , 0 , 0 , 1 , 0 , 0 , 0,
		0 , 0 , 1 , 1 , 1 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
		0 , 0 , 1 , 1 , 1 , 0 , 0,
		0 , 0 , 0 , 1 , 0 , 0 , 0,
		0 , 0 , 0 , 1 , 0 , 0 , 0,
		0 , 0 , 0 , 0 , 0 , 0 , 0,
	};
}
