using UnityEngine;
using System.Collections;
using System;

namespace com.b2mi.dc.Database
{
	public class DBCallbackDispatcher : MonoBehaviour
	{
		private static DBCallbackDispatcher singleton = null;
		private static GameObject singletonGameObject = null;
		private static object singletonLock = new object();
		
		public static DBCallbackDispatcher Singleton {
			get {
				return singleton;
			}
		}
		
		public Queue requests = Queue.Synchronized( new Queue() );
		
		public static void Init()
		{
			if ( singleton != null )
			{
				return;
			}
			
			lock( singletonLock )
			{
				if ( singleton != null )
				{
					return;
				}
				
				singletonGameObject = new GameObject();
				singleton = singletonGameObject.AddComponent< DBCallbackDispatcher >();
				singletonGameObject.name = "DBCallbackDispatcher";
			}
		}
		
		public void Update()
		{
			while( requests.Count > 0 )
			{
				DBCallback request = (DBCallback)requests.Dequeue();
				request.completedCallback( request );
			}
		}
	}
	
	public class DBCallback
	{
		private object data;
		public Action<DBCallback> completedCallback;
		
		public object Data
		{
			get{ return data;  }
			set{ data = value; }
		}
	}
}
