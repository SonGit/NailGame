using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using com.b2mi.dc.Network;
using MiniJSON;

public class Helpers 
{	
	private const string URL_FACEBOOK = "https://graph.facebook.com/v2.0";

	public static int checkErrorCode(Dictionary<string, object> errorData)
	{
		object errorCode;
		if (errorData.TryGetValue (GameConstants.RESPONE_KEY_ERROR_CODE, out errorCode))
		{
			try
			{
				int code = int.Parse(string.Format("{0}", errorCode));
				return code;
			}
			catch (Exception ex)
			{
				Debug.LogWarning (ex.ToString());
				return ErrorCode.UNKNOWN;
			}
		}
		return ErrorCode.UNKNOWN;
	}

	public static string GetPictureURL(string facebookID, int? width = null, int? height = null, string type = null, string accesstoken = null)
	{
		string url = string.Format("/{0}/picture", facebookID);
		string query = width != null ? "&width=" + width.ToString() : "";
		query += height != null ? "&height=" + height.ToString() : "";
		query += accesstoken != null ? "&access_token=" + accesstoken : "";
		if (query != "") url += ("?g" + query);
		url = URL_FACEBOOK + url; 
		return url;
	}

	public static Dictionary<string, string> DeserializeJSONProfile(string response)
	{
		var profile = new Dictionary<string, string>();
		try 
		{
			var responseObject = Json.Deserialize(response) as Dictionary<string, object>;
			object nameH;
			if (responseObject.TryGetValue("first_name", out nameH))
			{
				profile["first_name"] = (string)nameH;
			}
		} 
		catch (Exception ex)
		{
			Debug.LogWarning(ex.Message);
		}
		return profile;
	}

	public static string GetStringNotNull(object obj) 
	{
		if (obj != null)
		{
			return obj.ToString();
		}
		return "";
	}

	public static bool IsEmpty(string obj) 
	{
		if (obj != null && obj.Length > 0)
		{
			return false;
		}
		return true;
	}
}
