using UnityEngine;
using System.Collections;

public class DestroyMe : MonoBehaviour {
	public float Time;

	void OnEnable()
	{
		StartCoroutine (Deactivate());
	}

	IEnumerator Deactivate()
	{
		yield return new WaitForSeconds(Time);
		gameObject.SetActive (false);
	}

}
