using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TestAccount : MonoBehaviour {
	public Text text;
	// Use this for initialization
	public void OnClick_FB()
	{
		MainMenu.Instance.FacebookLogin ();
	}

	public void OnClick_Logout()
	{
		SessionManager.Instance.ClearSession ();
		Application.LoadLevel("HomeScene");
		CameraMovement.StarPointMoveIndex = -1;
	}
}
