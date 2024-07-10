#if UNITY_IOS

using GoogleMobileAds.Common;
using System;

namespace GoogleMobileAds.iOS
{
	internal class AdInspectorErrorClient : AdErrorClient, IAdInspectorErrorClient, IAdErrorClient
	{
		public AdInspectorErrorClient(IntPtr error) : base(error)
		{
		}
	}
}
#endif