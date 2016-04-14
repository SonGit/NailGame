using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using com.b2mi.dc.Network.Core;
using com.b2mi.dc.Security;
using MiniJSON;

namespace com.b2mi.dc.Network.RequestAPI
{
	public class SubmitDeviceRequest: BaseRequest
	{
		public SubmitDeviceRequest (String url, string platform, string model, string imei, string version, string appVersion, SuccessCallback successCallback, ErrorCallback errorCallback) : base(Method.POST, url, successCallback, errorCallback) 
		{
			Hashtable hashtable = new Hashtable ();
			hashtable.Add( GameConstants.REQUEST_KEY_PLATFORM, platform );
			hashtable.Add( GameConstants.REQUEST_KEY_MODEL, model );
			hashtable.Add( GameConstants.REQUEST_KEY_IMEI, imei );
			hashtable.Add( GameConstants.REQUEST_KEY_VERSION, version );
			hashtable.Add( GameConstants.REQUEST_KEY_APP_VERSION, appVersion );

			string json = Json.Serialize(hashtable );
			
			this.form.AddField( GameConstants.REQUEST_KEY_PARAMS, Encryption.Encrypt(json) );
			this.AddWWWForm(this.form);
		}
		
		public override object parseDataRespone(object result) 
		{
			Dictionary<string, object> data = (Dictionary<string, object>)result;
			Dictionary<string, object> resultData = new Dictionary<string, object>();
			string deviceId = Helpers.GetStringNotNull (data [GameConstants.RESPONE_KEY_DEVICE_ID]);
			resultData.Add(GameConstants.RESPONE_KEY_DEVICE_ID, deviceId);
			return resultData;
		}
	}
}
