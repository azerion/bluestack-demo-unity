#if UNITY_IOS
using System;
using System.Runtime.InteropServices;

namespace GoogleMobileAds.iOS
{
	internal class Externs
	{
		// Request Configuration
		
		[DllImport("__Internal")]
		internal static extern IntPtr GADUCreateRequestConfiguration();

		[DllImport("__Internal")]
		internal static extern void GADUSetRequestConfiguration(IntPtr requestConfiguration);

		[DllImport("__Internal")]
		internal static extern void GADUSetRequestConfigurationTestDeviceIdentifiers(
			IntPtr requestConfiguration,
			string[] testDeviceIDs,
			int testDeviceIDLength);

		[DllImport("__Internal")]
		internal static extern IntPtr GADUGetTestDeviceIdentifiers(IntPtr request);

		[DllImport("__Internal")]
		internal static extern int GADUGetTestDeviceIdentifiersCount(IntPtr request);

		// Ad Inspector
		
		[DllImport("__Internal")]
		internal static extern void GADUPresentAdInspector(IntPtr mobileAdsClient, MobileAdsClient.GADUAdInspectorClosedCallback callback);

		
		// AdError Methods
		
		[DllImport("__Internal")]
		internal static extern int GADUGetAdErrorCode(IntPtr error);

		[DllImport("__Internal")]
		internal static extern string GADUGetAdErrorDomain(IntPtr error);

		[DllImport("__Internal")]
		internal static extern string GADUGetAdErrorMessage(IntPtr error);

		[DllImport("__Internal")]
		internal static extern IntPtr GADUGetAdErrorUnderLyingError(IntPtr error);

		[DllImport("__Internal")]
		internal static extern string GADUGetAdErrorDescription(IntPtr error);
		
	}
}
#endif