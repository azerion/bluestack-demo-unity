#if UNITY_IOS

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GoogleMobileAds.iOS
{
  internal class Utils
  {
    public static string PtrToString(IntPtr stringPtr)
    {
      string stringAnsi = Marshal.PtrToStringAnsi(stringPtr);
      Marshal.FreeHGlobal(stringPtr);
      return stringAnsi;
    }

    public static List<string> PtrArrayToManagedList(IntPtr arrayPtr, int numOfAssets)
    {
      IntPtr[] destination = new IntPtr[numOfAssets];
      string[] collection = new string[numOfAssets];
      Marshal.Copy(arrayPtr, destination, 0, numOfAssets);
      for (int index = 0; index < numOfAssets; ++index)
      {
        collection[index] = Marshal.PtrToStringAuto(destination[index]);
        Marshal.FreeHGlobal(destination[index]);
      }
      Marshal.FreeHGlobal(arrayPtr);
      return new List<string>((IEnumerable<string>) collection);
    }
  }
}

#endif