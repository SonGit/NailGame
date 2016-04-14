using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using com.b2mi.dc.Network.Core;
using com.b2mi.dc.Security;
using MiniJSON;
using com.b2mi.dc.Entity;

namespace com.b2mi.dc.Network.RequestAPI
{
	public class GetHighScoresRequest: BaseRequest
	{
		public GetHighScoresRequest (String url, String sessionKey, String level, SuccessCallback successCallback, ErrorCallback errorCallback): base(Method.POST, url, successCallback, errorCallback)
		{
			Hashtable hashtable = new Hashtable ();
			hashtable.Add( GameConstants.REQUEST_KEY_SESSION_KEY, sessionKey );
			hashtable.Add( GameConstants.REQUEST_KEY_LEVEL, level );
			string json = Json.Serialize(hashtable );
			
			this.form.AddField( GameConstants.REQUEST_KEY_PARAMS, Encryption.Encrypt(json) );
			this.AddWWWForm(this.form);
		}
		
		public override object parseDataRespone(object result) 
		{
			Dictionary<string, object> data = (Dictionary<string, object>)result;
			List<object> list = (List<object>) data[GameConstants.RESPONE_KEY_LIST];
			
			List<FriendEntity> resultData = new List<FriendEntity>();

			foreach (object obj in list)
			{
				Dictionary<string, object> friend = (Dictionary<string, object>)obj;

				string avt = Helpers.GetStringNotNull (friend [GameConstants.RESPONE_KEY_AVATAR]);
				string name = Helpers.GetStringNotNull (friend [GameConstants.RESPONE_KEY_DISPLAYNAME]);
				string userId = Helpers.GetStringNotNull (friend [GameConstants.RESPONE_KEY_USER_ID]);
				string level = Helpers.GetStringNotNull (friend [GameConstants.RESPONE_KEY_LEVEL]);
				string score = Helpers.GetStringNotNull (friend [GameConstants.RESPONE_KEY_SCORE]);

				FriendEntity entity = new FriendEntity();
				entity.Avatar = avt;
				entity.Name = name;
				entity.UserId = userId;
				entity.Level = int.Parse(level);
				entity.Score = score;

				resultData.Add(entity);
			}

			return resultData;
		}
	}
}

