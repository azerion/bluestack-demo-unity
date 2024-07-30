#if UNITY_IOS

using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoogleMobileAds.iOS
{
	public class RequestConfigurationClient
	{
		private static IntPtr requestConfigurationPtr = Externs.GADUCreateRequestConfiguration();

		public static void SetRequestConfiguration(RequestConfiguration requestConfiguration)
		{
			if (requestConfiguration.TestDeviceIds.Count > 0)
			{
				string[] array = new string[requestConfiguration.TestDeviceIds.Count];
				requestConfiguration.TestDeviceIds.CopyTo(array);
				Externs.GADUSetRequestConfigurationTestDeviceIdentifiers(RequestConfigurationClient.requestConfigurationPtr, array, requestConfiguration.TestDeviceIds.Count);
			}

			Debug.Log("RequestConfigurationClient: SetRequestConfiguration: TestDeviceIds.Count: " + requestConfiguration.TestDeviceIds.Count);

			Externs.GADUSetRequestConfiguration(RequestConfigurationClient.requestConfigurationPtr);
		}

		public static RequestConfiguration GetRequestConfiguration()
		{
			RequestConfiguration.Builder builder = new RequestConfiguration.Builder();
			IntPtr arrayPtr = Externs.GADUGetTestDeviceIdentifiers(RequestConfigurationClient.requestConfigurationPtr);
			List<string> testDeviceIds = Utils.PtrArrayToManagedList(arrayPtr, Externs.GADUGetTestDeviceIdentifiersCount(RequestConfigurationClient.requestConfigurationPtr));
			builder.SetTestDeviceIds(testDeviceIds);
			return builder.build();
		}
	}
}

#endif
