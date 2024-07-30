using GoogleMobileAds.Common;
using System;

namespace GoogleMobileAds.Api
{
	public class AdError
	{
		private IAdErrorClient _client;

		public AdError(IAdErrorClient client)
		{
			this._client = client;
		}

		public int GetCode()
		{
			return this._client.GetCode();
		}

		public string GetDomain()
		{
			return this._client.GetDomain();
		}

		public string GetMessage()
		{
			return this._client.GetMessage();
		}

		public AdError GetCause()
		{
			return (this._client.GetCause() != null) ? new AdError(this._client.GetCause()) : null;
		}

		public override string ToString()
		{
			return this._client.ToString();
		}
	}
}
