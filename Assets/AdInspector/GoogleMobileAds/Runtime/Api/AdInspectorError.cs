using GoogleMobileAds.Common;
using System;

namespace GoogleMobileAds.Api
{
	public class AdInspectorError : AdError
	{
		public enum AdInspectorErrorCode
		{
			ERROR_CODE_INTERNAL_ERROR,
			ERROR_CODE_FAILED_TO_LOAD,
			ERROR_CODE_NOT_IN_TEST_MODE,
			ERROR_CODE_ALREADY_OPEN
		}

		public AdInspectorError(IAdInspectorErrorClient client) : base(client)
		{
		}

		public new AdInspectorError.AdInspectorErrorCode GetCode()
		{
			return (AdInspectorError.AdInspectorErrorCode)base.GetCode();
		}
	}
}
