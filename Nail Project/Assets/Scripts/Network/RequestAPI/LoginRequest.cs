

using System;
using System.Collections;
using System.Collections.Generic;

using com.b2mi.dc.Network.Core;
using com.b2mi.dc.Security;
using MiniJSON;
using UnityEngine;

namespace com.b2mi.dc.Network.RequestAPI
{
	public class LoginRequest: BaseRequest
	{
		public LoginRequest (String url, string userName, string password, SuccessCallback successCallback, ErrorCallback errorCallback) : base(Method.POST, url, successCallback, errorCallback) 
		{
			Hashtable hashtable = new Hashtable ();
			hashtable.Add( GameConstants.REQUEST_KEY_USER_NAME, userName );
			hashtable.Add( GameConstants.REQUEST_KEY_PASSWORD, password );

			string json = Json.Serialize(hashtable );

			this.form.AddField( GameConstants.REQUEST_KEY_PARAMS, Encryption.Encrypt(json) );
			this.AddWWWForm(this.form);
		}
		
		public override object parseDataRespone(object result) 
		{
			Dictionary<string, object> data = (Dictionary<string, object>)result;
			Dictionary<string, object> resultData = new Dictionary<string, object>();
			string sessionKey = Helpers.GetStringNotNull (data [GameConstants.RESPONE_KEY_SESSION_KEY]);
			resultData.Add(GameConstants.RESPONE_KEY_SESSION_KEY, sessionKey);

			return resultData;
		}
	}
}
