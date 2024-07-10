#if UNITY_ANDROID
using GoogleMobileAds.Common;
using System;
using UnityEngine;

namespace GoogleMobileAds.Android
{
	internal class AdInspectorErrorClient : AdErrorClient, IAdInspectorErrorClient, IAdErrorClient
	{
		public AdInspectorErrorClient(AndroidJavaObject error) : base(error)
		{
		}
	}
}
#endif