#if UNITY_EDITOR
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace GoogleMobileAds.Unity
{
	public class MobileAdsClient : IMobileAdsClient
	{
		public MobileAdsClient()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void SetRequestConfiguration(RequestConfiguration requestConfiguration)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public RequestConfiguration GetRequestConfiguration()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			return null;
		}
		
		public void OpenAdInspector(Action<AdInspectorErrorClientEventArgs> onAdInspectorClosed)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			onAdInspectorClosed(null);
		}

		public void DestroyAdInspector()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}
	}
}
#endif
