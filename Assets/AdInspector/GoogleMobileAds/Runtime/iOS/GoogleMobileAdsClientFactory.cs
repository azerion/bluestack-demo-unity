#if UNITY_IOS
using GoogleMobileAds.Common;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;

namespace GoogleMobileAds.iOS
{
	[Preserve]
	public class GoogleMobileAdsClientFactory : IClientFactory
	{

		public IMobileAdsClient MobileAdsInstance()
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				return MobileAdsClient.Instance;
			}
			throw new InvalidOperationException("Called " + MethodBase.GetCurrentMethod().Name + " on non-iOS runtime");
		}
	}
}
#endif