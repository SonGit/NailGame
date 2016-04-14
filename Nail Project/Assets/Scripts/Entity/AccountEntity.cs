using UnityEngine;
using System.Collections;
namespace com.b2mi.dc.Entity
{
	public class AccountEntity 
	{		
		private string userId;
		private string userName;
		private string password;
		private string accountType;

		public string UserId
		{
			get { return userId;}
			set { userId = value;}
		}
		
		public string UserName
		{
			get { return userName;}
			set { userName = value;}
		}
		
		public string Password
		{
			get { return password;}
			set { password = value;}
		}
		
		public string AccountType
		{
			get { return accountType;}
			set { accountType = value;}
		}
	}
}

