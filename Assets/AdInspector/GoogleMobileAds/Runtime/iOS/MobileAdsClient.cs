#if UNITY_IOS
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GoogleMobileAds.iOS
{
	public class MobileAdsClient : IMobileAdsClient
	{
		internal delegate void GADUAdInspectorClosedCallback(IntPtr mobileAdsClient, IntPtr errorRef);
		
		private static MobileAdsClient instance = new MobileAdsClient();

		private Action<AdInspectorErrorClientEventArgs> adInspectorClosedAction;
		
		private IntPtr mobileAdsClientPtr;

		
		[CompilerGenerated]
		private static MobileAdsClient.GADUAdInspectorClosedCallback inspectorCloseCallback;

		public static MobileAdsClient Instance
		{
			get
			{
				return MobileAdsClient.instance;
			}
		}

		private MobileAdsClient()
		{
			this.mobileAdsClientPtr = (IntPtr)GCHandle.Alloc(this);
		}

		public void SetRequestConfiguration(RequestConfiguration requestConfiguration)
		{
			RequestConfigurationClient.SetRequestConfiguration(requestConfiguration);
		}

		public RequestConfiguration GetRequestConfiguration()
		{
			return RequestConfigurationClient.GetRequestConfiguration();
		}

		public void OpenAdInspector(Action<AdInspectorErrorClientEventArgs> onAdInspectorClosed)
		{
			Debug.Log("MobileAdsClient:OpenAdInspector");
			this.adInspectorClosedAction = onAdInspectorClosed;
			IntPtr iOSAdsClientPtr = this.mobileAdsClientPtr;
			if (MobileAdsClient.inspectorCloseCallback == null)
			{
				MobileAdsClient.inspectorCloseCallback = new MobileAdsClient.GADUAdInspectorClosedCallback(MobileAdsClient.AdInspectorClosedCallback);
			}
			Externs.GADUPresentAdInspector(iOSAdsClientPtr, MobileAdsClient.inspectorCloseCallback);
		}

		[AOT.MonoPInvokeCallback(typeof(MobileAdsClient.GADUAdInspectorClosedCallback))]
		private static void AdInspectorClosedCallback(IntPtr mobileAdsClientPtr, IntPtr errorRef)
		{
			Debug.Log("MobileAdsClient:AdInspectorClosedCallback");
			MobileAdsClient mobileAdsClient = MobileAdsClient.IntPtrToMobileAdsClient(mobileAdsClientPtr);
			if (mobileAdsClient.adInspectorClosedAction == null)
			{
				return;
			}
			AdInspectorErrorClientEventArgs obj = (!(errorRef == IntPtr.Zero)) ? new AdInspectorErrorClientEventArgs
			{
				AdErrorClient = new AdInspectorErrorClient(errorRef)
			} : null;
			mobileAdsClient.adInspectorClosedAction(obj);
			mobileAdsClient.adInspectorClosedAction = null;
		}

		private static MobileAdsClient IntPtrToMobileAdsClient(IntPtr mobileAdsClient)
		{
			return ((GCHandle)mobileAdsClient).Target as MobileAdsClient;
		}

		public void Dispose()
		{
			((GCHandle)this.mobileAdsClientPtr).Free();
		}

		~MobileAdsClient()
		{
			this.Dispose();
		}
	}
}
#endif