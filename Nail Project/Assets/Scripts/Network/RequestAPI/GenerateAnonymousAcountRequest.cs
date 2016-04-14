
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using com.b2mi.dc.Network.Core;
using com.b2mi.dc.Security;
using MiniJSON;

namespace com.b2mi.dc.Network.RequestAPI
{
	public class GenerateAnonymousAcountRequest: BaseRequest
	{
		public GenerateAnonymousAcountRequest (String url, string deviceId, SuccessCallback successCallback, ErrorCallback errorCallback) : base(Method.POST, url, successCallback, errorCallback) 
		{
			Hashtable hashtable = new Hashtable ();
			hashtable.Add( GameConstants.REQUEST_KEY_DEVICE_ID, deviceId );
			
			string json = Json.Serialize(hashtable );
			
			this.form.AddField( GameConstants.REQUEST_KEY_PARAMS, Encryption.Encrypt(json) );
			this.AddWWWForm(this.form);
		}
		
		public override object parseDataRespone(object result) 
		{
			Dictionary<string, object> data = (Dictionary<string, object>)result;
			Dictionary<string, object> account = (Dictionary<string, object>) data[GameConstants.RESPONE_KEY_ACCOUNT];
			
			Dictionary<string, object> resultData = new Dictionary<string, object>();

			string userName = Helpers.GetStringNotNull (account [GameConstants.RESPONE_KEY_USER_NAME]);
			resultData.Add(GameConstants.RESPONE_KEY_USER_NAME, userName);

			string password = Helpers.GetStringNotNull (account [GameConstants.RESPONE_KEY_PASSWORD]);
			resultData.Add(GameConstants.RESPONE_KEY_PASSWORD, password);

			return resultData;
		}
	}
}
