#if UNITY_ANDROID
using System.Collections.Generic;
using UnityEngine;

namespace GoogleMobileAds.Android
{
	internal class Utils
	{
		public static AndroidJavaObject GetJavaListObject(List<string> csTypeList)
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.util.ArrayList", new object[0]);
			foreach (string current in csTypeList)
			{
				androidJavaObject.Call<bool>("add", new object[]
				{
					current
				});
			}
			return androidJavaObject;
		}

		public static List<string> GetCsTypeList(AndroidJavaObject javaTypeList)
		{
			List<string> list = new List<string>();
			int num = javaTypeList.Call<int>("size", new object[0]);
			for (int i = 0; i < num; i++)
			{
				list.Add(javaTypeList.Call<string>("get", new object[]
				{
					i
				}));
			}
			return list;
		}
	}
}
#endif