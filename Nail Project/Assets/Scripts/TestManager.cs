using UnityEngine;
using System.Collections;

public class TestManager : MonoBehaviour {
	public JewelSpawner _jewelSpawner;
	// Use this for initialization
	void Start () {
		//StartCoroutine (Test ());
	}
	
	IEnumerator Test()
	{
		print ("sgsdsdgsdg");
		yield return new WaitForSeconds(3f);
		StartCoroutine (_jewelSpawner.Respawn2(array));
	}

	int[,] array = new int[7,7]
	{
		{0, 1, 2 ,3 ,2 ,1 ,2  },
		{1, 3, 4 ,1 ,2 ,4 ,1  },
		{0, 1, 2 ,1 ,4 ,1 ,1  },
		{5, 4, 1 ,2 ,1 ,2 ,1 },
		{0, 1, 2 ,1 ,2 ,5 ,1 },
		{5, 4, 3 ,4 ,1 ,4 ,4  },
		{0, 1, 2 ,3 ,4 ,5 ,4  },
	};
}
