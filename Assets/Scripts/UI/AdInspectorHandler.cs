using System;
using UnityEngine;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine.UIElements;

public class AdInspectorHandler : MonoBehaviour
{
    public GameObject inspectorButtonObj;

    void Awake()
    {
#if UNITY_IPHONE        
        Debug.Log("iOS advertisingIdentifier: " + UnityEngine.iOS.Device.advertisingIdentifier);
#endif
        
        List<String> deviceIds = new List<String>() { "SIMULATOR" };
        
        // Add some test device IDs (replace with your own device IDs). Check following logs to get test device ID.
        // Android: Info Ads Use RequestConfiguration.Builder().setTestDeviceIds(Arrays.asList("58214BFD445FE4A8EC2770E56CDFB47F")) to get test ads on this device.
        // iOS: <Google> To get test ads on this device, set: GADMobileAds.sharedInstance().requestConfiguration.testDeviceIdentifiers = [ "e813ed2fbe15bb51cf4c544f7fbd8d3b" ]
#if UNITY_IPHONE
        deviceIds.Add("9cdafb247d727b64a278204e475e694b");
#elif UNITY_ANDROID
        deviceIds.Add("58214BFD445FE4A8EC2770E56CDFB47F");
#endif

        // Configure TagForChildDirectedTreatment and test device IDs.
        RequestConfiguration requestConfiguration =
            new RequestConfiguration.Builder()
                .SetTestDeviceIds(deviceIds).build();
        MobileAds.SetRequestConfiguration(requestConfiguration);
    }

    public static void OpenAdInspector()
    {
        Debug.Log("Open AdInspector ->");
        MobileAds.OpenAdInspector(error => {
            // Error will be set if there was an issue and the inspector was not displayed.
            if (error != null) Debug.Log(error.GetMessage());
        });
    }

    public void SetInspectorButtonActive(bool value)
    {
        if(inspectorButtonObj!= null)
            inspectorButtonObj.SetActive(value);
    }
}