using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour {
	
	public static ObjectPool instance;

	public GameObject StripedEffectPrefab;

	public GameObject WrapperEffectPrefab;

	List<GameObject> StripeEffectCache = new List<GameObject>();

	List<GameObject> WrapperEffectCache = new List<GameObject>();

	Vector3 defaultPos = new Vector3(-999,-999,-999);

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		for (int i = 0; i < 5; i++) {
			GameObject go = (GameObject)Instantiate (StripedEffectPrefab, defaultPos, StripedEffectPrefab.transform.rotation);
			go.SetActive (false);
			StripeEffectCache.Add( go );
		}
		for (int i = 0; i < 5; i++) {
			GameObject go = (GameObject)Instantiate (WrapperEffectPrefab, defaultPos, WrapperEffectPrefab.transform.rotation);
			go.SetActive (false);
			WrapperEffectCache.Add( go );
		}
	}

	public GameObject GetStripedEffectObj()
	{
		foreach (GameObject go in StripeEffectCache) {
			if (!go.activeInHierarchy) {
				go.SetActive (true);
				return go;
			}
		}
		return MakeNewStripeEffectObj ();
	}

	public GameObject GetWrapperEffectObj()
	{
		foreach (GameObject go in WrapperEffectCache) {
			if (!go.activeInHierarchy) {
				go.SetActive (true);
				return go;
			}
		}
		return MakeNewWrapperEffectObj ();
	}

	GameObject MakeNewStripeEffectObj()
	{
		GameObject newObject = (GameObject)Instantiate (StripedEffectPrefab, defaultPos, StripedEffectPrefab.transform.rotation);
		StripeEffectCache.Add( newObject );
		return newObject;
	}

	GameObject MakeNewWrapperEffectObj()
	{
		GameObject newObject = (GameObject)Instantiate (WrapperEffectPrefab, defaultPos, WrapperEffectPrefab.transform.rotation);
		WrapperEffectCache.Add( newObject );
		return newObject;
	}


}
