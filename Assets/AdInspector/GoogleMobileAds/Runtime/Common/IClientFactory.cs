using GoogleMobileAds.Common;
using System;

namespace GoogleMobileAds
{
	public interface IClientFactory
	{
		// IAppStateEventClient BuildAppStateEventClient();
		//
		// IAppOpenAdClient BuildAppOpenAdClient();
		//
		// IBannerClient BuildBannerClient();
		//
		// IInterstitialClient BuildInterstitialClient();
		//
		// IRewardedAdClient BuildRewardedAdClient();
		//
		// IRewardedInterstitialAdClient BuildRewardedInterstitialAdClient();

		IMobileAdsClient MobileAdsInstance();
	}
}
