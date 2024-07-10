#if UNITY_ANDROID
using GoogleMobileAds.Common;
using System;
using UnityEngine;

namespace GoogleMobileAds.Android
{
	public class AdInspectorListener : AndroidJavaProxy
	{
		private Action<AdInspectorErrorClientEventArgs> adInspectorClosedAction;

		public AdInspectorListener(Action<AdInspectorErrorClientEventArgs> adInspectorClosedAction) : base("com.google.unity.ads.UnityAdInspectorListener")
		{
			this.adInspectorClosedAction = adInspectorClosedAction;
		}

		private void onAdInspectorClosed(AndroidJavaObject error)
		{
			if (this.adInspectorClosedAction == null)
			{
				return;
			}
			AdInspectorErrorClientEventArgs obj = (error != null) ? new AdInspectorErrorClientEventArgs
			{
				AdErrorClient = new AdInspectorErrorClient(error)
			} : null;
			this.adInspectorClosedAction(obj);
		}
	}
}
#endif