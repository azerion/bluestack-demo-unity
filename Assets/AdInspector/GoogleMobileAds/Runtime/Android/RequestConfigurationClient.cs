#if UNITY_ANDROID
using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoogleMobileAds.Android
{
	public class RequestConfigurationClient
	{
		public static AndroidJavaObject BuildRequestConfiguration(RequestConfiguration requestConfiguration)
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.google.android.gms.ads.RequestConfiguration$Builder", new object[0]);

			if (requestConfiguration.TestDeviceIds.Count > 0)
			{
				AndroidJavaObject javaListObject = Utils.GetJavaListObject(requestConfiguration.TestDeviceIds);
				androidJavaObject = androidJavaObject.Call<AndroidJavaObject>("setTestDeviceIds", new object[]
				{
					javaListObject
				});
			}

			return androidJavaObject.Call<AndroidJavaObject>("build", new object[0]);
		}

		public static RequestConfiguration GetRequestConfiguration(AndroidJavaObject androidRequestConfiguration)
		{
			List<string> csTypeList = Utils.GetCsTypeList(androidRequestConfiguration.Call<AndroidJavaObject>("getTestDeviceIds", new object[0]));
			RequestConfiguration.Builder builder = new RequestConfiguration.Builder();
			builder = builder.SetTestDeviceIds(csTypeList);
			return builder.build();
		}
	}
}
#endif