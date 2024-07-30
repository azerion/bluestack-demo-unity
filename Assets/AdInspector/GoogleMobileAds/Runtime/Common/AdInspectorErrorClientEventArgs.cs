using System;

namespace GoogleMobileAds.Common
{
	public class AdInspectorErrorClientEventArgs : System.EventArgs
	{
		public IAdInspectorErrorClient AdErrorClient
		{
			get;
			set;
		}
	}
}
