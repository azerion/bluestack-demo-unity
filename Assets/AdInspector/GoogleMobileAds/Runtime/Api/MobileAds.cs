using GoogleMobileAds.Common;
using System;
using UnityEngine;

namespace GoogleMobileAds.Api
{
	public class MobileAds
	{
		private readonly IMobileAdsClient client = MobileAds.GetMobileAdsClient();

		private static IClientFactory clientFactory;

		private static MobileAds instance;

		public static MobileAds Instance
		{
			get
			{
				if (MobileAds.instance == null)
				{
					MobileAds.instance = new MobileAds();
				}
				return MobileAds.instance;
			}
		}

		public static void SetRequestConfiguration(RequestConfiguration requestConfiguration) => MobileAds.Instance.client.SetRequestConfiguration(requestConfiguration);

		public static RequestConfiguration GetRequestConfiguration() => MobileAds.Instance.client.GetRequestConfiguration();

		public static void OpenAdInspector(Action<AdInspectorError> adInspectorClosedAction)
		{
			MobileAds.Instance.client.OpenAdInspector(delegate(AdInspectorErrorClientEventArgs args)
			{
				if (adInspectorClosedAction != null)
				{
					AdInspectorError obj = null;
					if (args != null && args.AdErrorClient != null)
					{
						obj = new AdInspectorError(args.AdErrorClient);
					}
					adInspectorClosedAction(obj);
				}
			});
		}

		internal static IClientFactory GetClientFactory()
		{
			if (MobileAds.clientFactory == null)
			{
				string typeName;
				if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					typeName = "GoogleMobileAds.iOS.GoogleMobileAdsClientFactory";
				}
				else if (Application.platform == RuntimePlatform.Android)
				{
					typeName = "GoogleMobileAds.Android.GoogleMobileAdsClientFactory";
				}
				else
				{
					typeName = "GoogleMobileAds.Unity.GoogleMobileAdsClientFactory";
				}
				Type type = Type.GetType(typeName, true);
				// Debug.Log("Client Type: " + type.ToString());
				MobileAds.clientFactory = (IClientFactory)Activator.CreateInstance(type);
				
			}
			return MobileAds.clientFactory;
		}

		internal static void SetClientFactory(IClientFactory clientFactory)
		{
			MobileAds.clientFactory = clientFactory;
		}

		private static IMobileAdsClient GetMobileAdsClient()
		{
			return MobileAds.GetClientFactory().MobileAdsInstance();
		}
	}
}
