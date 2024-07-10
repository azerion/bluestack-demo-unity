#if UNITY_IOS
using GoogleMobileAds.Common;
using System;

namespace GoogleMobileAds.iOS
{
	internal class AdErrorClient : IAdErrorClient
	{
		private IntPtr error;

		public AdErrorClient(IntPtr error)
		{
			this.error = error;
		}

		public int GetCode()
		{
			return Externs.GADUGetAdErrorCode(this.error);
		}

		public string GetDomain()
		{
			return Externs.GADUGetAdErrorDomain(this.error);
		}

		public string GetMessage()
		{
			return Externs.GADUGetAdErrorMessage(this.error);
		}

		public IAdErrorClient GetCause()
		{
			return new AdErrorClient(Externs.GADUGetAdErrorUnderLyingError(this.error));
		}

		public override string ToString()
		{
			return Externs.GADUGetAdErrorDescription(this.error);
		}
	}
}
#endif