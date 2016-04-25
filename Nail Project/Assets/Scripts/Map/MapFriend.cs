using UnityEngine;
using System.Collections;
using com.b2mi.dc.Entity;
using System.Collections.Generic;

public class MapFriend : MonoBehaviour {

	public MeshRenderer[] avatarMeshes;

	bool isShowAll;

	// Use this for initialization
	void Awake () {
		avatarMeshes = this.GetComponentsInChildren<MeshRenderer> ();
	}

	public void Init( List<FriendEntity> friends)
	{
		if (friends.Count < avatarMeshes.Length) {
			for (int i = 0; i < friends.Count; i++) {
				StartCoroutine (UserImage (friends [i].Avatar, avatarMeshes [i]));
			}

		} else {
			for (int i = 0; i < avatarMeshes.Length; i++) {
				StartCoroutine (UserImage (friends [i].Avatar, avatarMeshes [i]));
			}
		}

		//First friend will be shown.The rest is shown when player click on this friest friend
		avatarMeshes [0].enabled = true;
	}

	IEnumerator UserImage(string url, MeshRenderer image)
	{

		WWW www = new WWW(url); 
		Texture2D textFb2 = new Texture2D(128, 128, TextureFormat.DXT1, false); //TextureFormat must be DXT5
		yield return www;
		www.LoadImageIntoTexture(textFb2);
		image.material.mainTexture = textFb2;
		image.material.shader = Shader.Find ("Sprites/Default");
		image.material.color = Color.white;
	}

	//On mouse lick, show the rest of friends
	void OnMouseDown()
	{
		isShowAll = !isShowAll;
		for (int i = 1; i < avatarMeshes.Length; i++) {
			avatarMeshes [i].enabled = isShowAll;
		}
	}

}
