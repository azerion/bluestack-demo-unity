#if UNITY_EDITOR
using GoogleMobileAds.Common;
using System;
using UnityEngine.Scripting;

namespace GoogleMobileAds.Unity
{
	[Preserve]
	public class GoogleMobileAdsClientFactory : IClientFactory
	{
		public IMobileAdsClient MobileAdsInstance()
		{
			return new MobileAdsClient();
		}
	}
}
#endif