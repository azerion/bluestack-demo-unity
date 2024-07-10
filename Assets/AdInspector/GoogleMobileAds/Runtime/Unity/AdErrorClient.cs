#if UNITY_EDITOR
using GoogleMobileAds.Common;
using System;

namespace GoogleMobileAds.Unity
{
	internal class AdErrorClient : IAdErrorClient
	{
		public int GetCode()
		{
			return -1;
		}

		public string GetDomain()
		{
			return "Google Mobile Ads";
		}

		public string GetMessage()
		{
			return "Prefab Ad is Null";
		}

		public IAdErrorClient GetCause()
		{
			return null;
		}

		public override string ToString()
		{
			return "Prefab Ad is Null";
		}
	}
}
#endif