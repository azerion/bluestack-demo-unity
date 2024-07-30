#if UNITY_ANDROID
using GoogleMobileAds.Common;
using System;
using UnityEngine;

namespace GoogleMobileAds.Android
{
	internal class AdErrorClient : IAdErrorClient
	{
		private AndroidJavaObject error;

		public AdErrorClient(AndroidJavaObject error)
		{
			this.error = error;
		}

		public int GetCode()
		{
			return this.error.Call<int>("getCode", new object[0]);
		}

		public string GetDomain()
		{
			return this.error.Call<string>("getDomain", new object[0]);
		}

		public string GetMessage()
		{
			return this.error.Call<string>("getMessage", new object[0]);
		}

		public IAdErrorClient GetCause()
		{
			return new AdErrorClient(this.error.Call<AndroidJavaObject>("getCause", new object[0]));
		}

		public override string ToString()
		{
			return this.error.Call<string>("toString", new object[0]);
		}
	}
}
#endif