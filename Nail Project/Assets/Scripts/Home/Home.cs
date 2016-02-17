using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Facebook.Unity;
using Facebook.MiniJSON;
using System.Collections.Generic;

public class Home : MonoBehaviour
{
	public SpriteRenderer test;
    void Start()
    {
        // hidden banner (banner only show on Game Play scene)
//        GoogleMobileAdsScript.advertise.HideBanner();
        MusicController.Music.BG_menu();
		ResourcesMgr.Init ();
    }

	public void TestFacebook()
	{
		FB.Init(this.OnInitFB);
	}

	void OnInitFB()
	{
		FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, this.HandleResult);
	}


	protected void HandleResult(IResult result)
	{
		if (result == null)
		{
			print ("nyll");
			return;
		}

		print (result.RawResult);
	}

    void Update()
    {
        // Exit game if click Escape key or back on mobile
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitOK();
        }
    }

    /// <summary>
    /// Exit game
    /// </summary>
    public void ExitOK()
    {
        Application.Quit();
    }

}
