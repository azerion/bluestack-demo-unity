using System;
using System.Collections.Generic;

namespace GoogleMobileAds.Api
{
	public class RequestConfiguration
	{
		public class Builder
		{
			internal List<string> TestDeviceIds
			{
				get;
				private set;
			}

			public Builder()
			{
				this.TestDeviceIds = new List<string>();
			}

			public RequestConfiguration.Builder SetTestDeviceIds(List<string> testDeviceIds)
			{
				this.TestDeviceIds = testDeviceIds;
				return this;
			}

			public RequestConfiguration build()
			{
				return new RequestConfiguration(this);
			}
		}

		public List<string> TestDeviceIds
		{
			get;
			private set;
		}

		private RequestConfiguration(RequestConfiguration.Builder builder)
		{
			this.TestDeviceIds = builder.TestDeviceIds;
		}

		public RequestConfiguration.Builder ToBuilder()
		{
			RequestConfiguration.Builder builder = new RequestConfiguration.Builder().SetTestDeviceIds(this.TestDeviceIds);
			return builder;
		}
	}
}
