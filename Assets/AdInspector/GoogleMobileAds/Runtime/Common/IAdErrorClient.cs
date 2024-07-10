using System;

namespace GoogleMobileAds.Common
{
	public interface IAdErrorClient
	{
		int GetCode();

		string GetDomain();

		string GetMessage();

		IAdErrorClient GetCause();
	}
}
