#if UNITY_EDITOR
using GoogleMobileAds.Common;
using System;

namespace GoogleMobileAds.Unity
{
	internal class AdInspectorErrorClient : AdErrorClient, IAdInspectorErrorClient, IAdErrorClient
	{
	}
}
#endif