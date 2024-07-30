#if UNITY_ANDROID
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using UnityEngine;

namespace GoogleMobileAds.Android
{
	public class MobileAdsClient : AndroidJavaProxy, IMobileAdsClient
	{
		private static MobileAdsClient instance = new MobileAdsClient();
		
		public static MobileAdsClient Instance
		{
			get
			{
				return MobileAdsClient.instance;
			}
		}

		private MobileAdsClient() : base("com.google.android.gms.ads.initialization.OnInitializationCompleteListener")
		{
		}

		public void SetRequestConfiguration(RequestConfiguration requestConfiguration)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.google.android.gms.ads.MobileAds");
			AndroidJavaObject androidJavaObject = RequestConfigurationClient.BuildRequestConfiguration(requestConfiguration);
			androidJavaClass.CallStatic("setRequestConfiguration", new object[]
			{
				androidJavaObject
			});
		}

		public RequestConfiguration GetRequestConfiguration()
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.google.android.gms.ads.MobileAds");
			AndroidJavaObject androidRequestConfiguration = androidJavaClass.CallStatic<AndroidJavaObject>("getRequestConfiguration", new object[0]);
			return RequestConfigurationClient.GetRequestConfiguration(androidRequestConfiguration);
		}

		public void OpenAdInspector(Action<AdInspectorErrorClientEventArgs> onAdInspectorClosed)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.google.unity.ads.UnityAdInspector");
			AdInspectorListener adInspectorListener = new AdInspectorListener(onAdInspectorClosed);
			androidJavaClass2.CallStatic("openAdInspector", new object[]
			{
				@static,
				adInspectorListener
			});
		}
	}
}
#endif
