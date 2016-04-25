using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using com.b2mi.dc.Entity;

public class InviteFriendItem : MonoBehaviour {

	public RawImage Avatar;
	public Text NameLabel;

	// Use this for initialization
	void Start () {
	
	}
	
	public void Init(FriendEntity friend)
	{
		NameLabel.text = friend.Name;
		StartCoroutine ( UserImage(friend.Avatar,Avatar));
	}

	IEnumerator UserImage(string url,RawImage image)
	{
		WWW www = new WWW(url); 
		Texture2D textFb2 = new Texture2D(128, 128, TextureFormat.DXT1, false); //TextureFormat must be DXT5
		yield return www;
		Avatar.texture = www.texture;
	}
}
