using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using com.b2mi.dc.Network.Core;
using com.b2mi.dc.Security;
using MiniJSON;

namespace com.b2mi.dc.Network.RequestAPI
{
	public class BaseRequest : Request
	{			
		private const string TAG = "BaseRequest";
		protected String jsonData = "";
		protected WWWForm form;
		protected SuccessCallback successCallback;
		protected ErrorCallback errorCallback;

		public BaseRequest (String method, String url, SuccessCallback successCallback, ErrorCallback errorCallback):base(method, url)
		{
			this.completedCallback = delegate(Request request) { onCompleteCallback(request); };;
			this.successCallback = successCallback;
			this.errorCallback = errorCallback;

			form = AddParams();
		}

		private WWWForm AddParams()
		{
			WWWForm form = new WWWForm();
			form.AddField( GameConstants.REQUEST_KEY_API_KEY, Encryption.API_KEY);
			form.AddField( GameConstants.REQUEST_KEY_TIMESTAMP, "" + (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond));
			return form;
		}

		public void SendRequest()
		{
			Debug.Log(TAG + ": " + " url = "+ this.uri + ", params = " + jsonData);
			this.Send(completedCallback);
			//TestScript.instance.Send ("http://giftalk.com.vn/user/anonymous",form);
		}
			
		private void onCompleteCallback(Request request)
		{
			try
			{
				Debug.Log(request.response.Text);
				Dictionary<string, object> responseData = (Dictionary<string, object>) Json.Deserialize(request.response.Text);
				if (responseData!= null)
				{
					int err = Helpers.checkErrorCode(responseData);
					object resultData = null;
					switch(err)
					{
					case ErrorCode.SUCCESS:
						if (responseData.TryGetValue(GameConstants.RESPONE_KEY_DATA, out resultData))
						{
							object data = parseDataRespone(resultData);
							if (successCallback != null) 
							{
								successCallback(data, ErrorCode.SUCCESS);
							}
						} else {
							if (errorCallback != null) 
							{
								errorCallback("", err);
							}
						}
						break;
					default:
						if (errorCallback != null) 
						{
							errorCallback("", err);
						}
						break;
					}
				}
			} 
			catch (Exception ex)
			{
				Debug.LogWarning(ex.ToString());
				if (errorCallback != null)
				{
					errorCallback(ex.ToString(), ErrorCode.UNKNOWN);
				}
			}
		}

		public virtual object parseDataRespone(object result) 
		{
			return result;
		}
	}
}

