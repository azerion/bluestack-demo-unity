using GoogleMobileAds.Api;
using System;

namespace GoogleMobileAds.Common
{
	public interface IMobileAdsClient
	{
		void SetRequestConfiguration(RequestConfiguration requestConfiguration);

		RequestConfiguration GetRequestConfiguration();
		
		void OpenAdInspector(System.Action<AdInspectorErrorClientEventArgs> adInspectorClosedAction);
	}
}
