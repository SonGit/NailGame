using System;
namespace com.b2mi.dc.Network.RequestAPI
{
	public delegate void SuccessCallback (object data, int errorCode);
	public delegate void ErrorCallback (object data, int errorCode);
}

