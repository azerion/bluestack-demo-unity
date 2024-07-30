#if UNITY_ANDROID
using GoogleMobileAds.Common;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;

namespace GoogleMobileAds.Android
{
	[Preserve]
	public class GoogleMobileAdsClientFactory : IClientFactory
	{
		public IMobileAdsClient MobileAdsInstance()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return MobileAdsClient.Instance;
			}
			throw new InvalidOperationException("Called " + MethodBase.GetCurrentMethod().Name + " on non-Android runtime");
		}
	}
}
#endif