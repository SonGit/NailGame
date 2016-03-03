using UnityEngine;
using System.Collections;

public class MapSceneController : MonoBehaviour {

	public void SettingButtonHandle () {
		PopupMapSetting.ShowPopup ();
	}
	public void InviteButtonHandle () {
		PopupInviteFriends.ShowPopup ();
	}
	public void BuyGoldHandle () {
		PopupBuyGold.ShowPopup ();
	}
	public void SendLivesHandle () {
		PopupSendLives.ShowPopup ();
	}
}
