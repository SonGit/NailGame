using UnityEngine;
using System.Collections;

public class FbRequestEntity {

	private int type;
	private int requestId;
	private string uidf ;
	private string uidt ;

	public int Type
	{
		get{ return type;  }
		set{ type = value; }
	}
	
	public int RequestId
	{
		get{ return requestId;  }
		set{ requestId = value; }
	}

	public string UserIdTo
	{
		get{ return uidt;  }
		set{ uidt = value; }
	}

	public string UserIdFrom
	{
		get{ return uidf;  }
		set{ uidf = value; }
	}
}
