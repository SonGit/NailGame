using UnityEngine;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;

namespace com.b2mi.dc.Security
{
	public class Encryption
	{
		public const string API_KEY = "11f7749fe668f2334acb67690cf30697";
		public const string PRIVATE_KEY = "4c496e7d91d1fe632eecaa33044d3706";
		private static byte[] key = UTF8Encoding.UTF8.GetBytes(PRIVATE_KEY);
		private static byte[] salt = UTF8Encoding.UTF8.GetBytes("bamboo@#dm3%&123");	
		
		/// <summary>
		/// Encrypt a string using dual encryption method. Return a encrypted cipher Text
		/// </summary>
		/// <param name="text">string to be encrypted</param>
		/// <param name="useHashing">use hashing? send to for extra secirity</param>
		/// <returns></returns>
		public static string Encrypt(string text)
		{
			byte[] textBytes = UTF8Encoding.UTF8.GetBytes(text);
			
			SHA256 sha256 = SHA256.Create();
			byte[] keyEncoded = sha256.ComputeHash(key);
			sha256.Clear();
			
			RijndaelManaged SymmetricKey = new RijndaelManaged();
			
			SymmetricKey.Mode = CipherMode.CBC;
			SymmetricKey.Padding = PaddingMode.Zeros;
			
			byte[] CipherTextBytes = null;
			
			using (ICryptoTransform Encryptor = SymmetricKey.CreateEncryptor(keyEncoded, salt))
			{
				using (MemoryStream MemStream = new MemoryStream())
				{
					using (CryptoStream CryptoStream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write))
					{
						CryptoStream.Write(textBytes, 0, textBytes.Length);
						CryptoStream.FlushFinalBlock();
						CipherTextBytes = MemStream.ToArray();
						MemStream.Close();
						CryptoStream.Close();
					}
				}
			}
			
			SymmetricKey.Clear();
			
			return Convert.ToBase64String(CipherTextBytes);
		}
		
		/// <summary>
		/// DeCrypt a string using dual encryption method. Return a DeCrypted clear string
		/// </summary>
		/// <param name="cipherString">encrypted string</param>
		/// <param name="useHashing">Did you use hashing to encrypt this data? pass true is yes</param>
		/// <returns></returns>
		public static string Decrypt(string CipherText)
		{
			byte[] CipherTextBytes = Convert.FromBase64String(CipherText);
			
			SHA256 sha256 = SHA256.Create();
			byte[] keyEncoded = sha256.ComputeHash(key);
			sha256.Clear();
			
			RijndaelManaged SymmetricKey = new RijndaelManaged();
			
			SymmetricKey.Mode = CipherMode.CBC;
			SymmetricKey.Padding = PaddingMode.Zeros;
			
			byte[] PlainTextBytes = new byte[CipherTextBytes.Length];

			int count = 0;


			int ByteCount = 0;			
			using (ICryptoTransform Decryptor = SymmetricKey.CreateDecryptor(keyEncoded, salt))
			{
				using (MemoryStream MemStream = new MemoryStream(CipherTextBytes))
				{
					using (CryptoStream CryptoStream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read))
					{
						ByteCount = CryptoStream.Read(PlainTextBytes, 0, PlainTextBytes.Length);
						MemStream.Close();
						CryptoStream.Close();
					}
				}
			}
			
			SymmetricKey.Clear();

			for (int i = PlainTextBytes.Length - 1; i >= 0; i--) {
				if (PlainTextBytes[i] == 0) {
					count++;
				}
			}
			byte[] oldData = PlainTextBytes;
			ByteCount = PlainTextBytes.Length - count;
			PlainTextBytes = new byte[ByteCount];
			Buffer.BlockCopy (oldData, 0, PlainTextBytes, 0, ByteCount);

			return Encoding.UTF8.GetString(PlainTextBytes, 0, ByteCount);
		}
	}
}
