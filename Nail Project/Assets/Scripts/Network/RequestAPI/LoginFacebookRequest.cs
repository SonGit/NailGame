using System;
using System.Collections;
using com.b2mi.dc.Network.Core;
using com.b2mi.dc.Security;
using MiniJSON;

namespace com.b2mi.dc.Network.RequestAPI
{
	public class LoginFacebookRequest: BaseRequest
	{
		public LoginFacebookRequest (String url, string token, SuccessCallback successCallback, ErrorCallback errorCallback) : base(Method.POST, url, successCallback, errorCallback) 
		{
			Hashtable hashtable = new Hashtable ();
			hashtable.Add( GameConstants.REQUEST_KEY_TOKEN, token );

			string json = Json.Serialize(hashtable );
			
			this.form.AddField( GameConstants.REQUEST_KEY_PARAMS, Encryption.Encrypt(json) );
			this.AddWWWForm(this.form);
		}
		
		public override object parseDataRespone(object result) 
		{
			return result;
		}
	}
}


