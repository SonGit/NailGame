using UnityEngine;
using System.Collections;

public class TestManager : MonoBehaviour {
	public JewelSpawner _jewelSpawner;
	// Use this for initialization
	void Start () {
		StartCoroutine (Test ());
	}
	
	IEnumerator Test()
	{
		yield return new WaitForSeconds(3f);
		StartCoroutine (_jewelSpawner.Respawn2(array));
	}

	int[,] array = new int[7,9]
	{
		{0, 1, 2 ,3 ,4 ,1 ,2 ,3 ,2 },
		{1, 3, 1 ,4 ,2 ,4 ,4 ,4 ,4 },
		{0, 1, 2 ,1 ,4 ,1 ,2 ,3 ,2 },
		{5, 4, 3 ,2 ,1 ,2 ,1 ,3 ,2},
		{0, 1, 2 ,3 ,2 ,5 ,1 ,2 ,3 },
		{5, 4, 3 ,2 ,4 ,0 ,4 ,4 ,4 },
		{0, 1, 2 ,3 ,1 ,5 ,4 ,4 ,4 },
	};
}
