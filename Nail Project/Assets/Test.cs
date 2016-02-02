using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	public GameObject Cube;
	public float offset = 0.2f;

	// Use this for initialization
	void Start () {

		float distance = Vector3.Distance(Cube.transform.position, transform.position);
		float impact = distance / offset;
		StartCoroutine (Boom());
	}
	
	// Update is called once per frame
	void Update () {
		float distance = Vector3.Distance(Cube.transform.position, transform.position);
		float impact =  1 / (distance / offset); //The further the ground zero, the less impact it have

		Vector3 dir = Cube.transform.position - transform.position;
		Vector3 destination =  transform.position - (dir.normalized * impact);
		Debug.DrawLine (transform.position,Cube.transform.position);
		Debug.DrawLine (transform.position, destination);
	}

	IEnumerator Boom()
	{
		Vector3 oldpos;
		float distance = Vector3.Distance(Cube.transform.position, transform.position);
		float impact =  1 / (distance / offset); //The further the ground zero, the less impact it have

		Vector3 dir = Cube.transform.position - transform.position;
		Vector3 destination =  transform.position - (dir.normalized * impact);

		yield return new WaitForSeconds(0.5f);
		oldpos = transform.position;
		iTween.MoveTo(gameObject,iTween.Hash("position",destination,
		                                     "easetype",iTween.EaseType.easeInOutSine,
		                                     "time",.8f));

		yield return new WaitForSeconds(0.8f);

		iTween.MoveTo(gameObject,iTween.Hash("position",oldpos,
		                                     "easetype",iTween.EaseType.easeInOutSine,
		                                     "time",.8f));
	}
}
