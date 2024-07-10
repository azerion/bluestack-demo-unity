using System;
using UnityEngine;
using System.Collections.Generic;
using Azerion.BlueStack.API;

public enum AdType
{
    RewardedVideo = 0,
    Interstitial = 1,
    MoreApps = 2,
    Banner = 3,
    Native = 4,
    Unknown = 5
}

/// <summary>
/// Class which combines Ad SDKs and UI
/// </summary>
public class AdManager : MonoBehaviour
{
    public static AdManager Instance;
    public static event Action<AdSDK, AdType> OnAdLoaded;
    public static event Action<AdSDK, AdType> OnAdLoadFailed;
    public static event Action<AdSDK, NativeAd> OnNativeAdAvailable;

    private EventManager _eventManager; // manages communication with ad sdks

    private List<Subscription> _subscriptions;

    //private int sdkCount = 0;
    private Queue<Action> _queuedThreadAction;

    public void OnEnable()
    {
        _eventManager = EventManager.Instance;
        _subscriptions = new List<Subscription>();

        _subscriptions.Add(_eventManager.Subscribe(EventTopic.AdSDKInitializationStatus,
            (AdSDKInitializationMessage message) =>
            {
                if (message.Subject == AdSDK.Bluestack && message.Status == "Complete")
                {
                    // ShowBanner(AdSDK.Bluestack);
                }
            }
        ));

        _subscriptions.Add(_eventManager.Subscribe(EventTopic.AdLoaded, (AdEventMessage message) =>
            {
                // Debug.Log("AdManager AdLoaded - " + message.Type.ToString());
                OnAdLoaded?.Invoke(message.Subject, message.Type);
            }
        ));

        _subscriptions.Add(_eventManager.Subscribe(EventTopic.AdFailedToLoad,
            (AdEventMessage message) => { OnAdLoadFailed?.Invoke(message.Subject, message.Type); }
        ));

        _subscriptions.Add(_eventManager.Subscribe(EventTopic.NativeAdLoaded, (NativeAdEventMessage message) =>
            {
                // Debug.Log("AdController: NativeAdLoaded - " + message.NativeAd.GetTitle());
                OnNativeAdAvailable?.Invoke(message.Subject, message.NativeAd);
            }
        ));
    }

    public void OnDisable()
    {
        foreach (var subscription in _subscriptions)
        {
            subscription.Remove();
        }

        _subscriptions = new List<Subscription>();
    }

    private void Awake()
    {
        // Singleton
        if (AdManager.Instance == null)
            AdManager.Instance = this;
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this);
        _queuedThreadAction = new Queue<Action>();
    }

    private void Start()
    {
        //sdkCount = Enum.GetNames(typeof(AdSDK)).Length - 1;
        //_eventManager.Publish(EventTopic.InitializeAdSDK, new Message());
    }

    private void OnDestroy()
    {
        ClearJobs();
    }

    /// <summary>
    /// Method to show an native ad
    /// </summary>
    /// <param name="adSDK">Ad SDK to request</param> 
    /// <param name="OnDisplaySuccess">Callback once the ad has been loaded successfully</param>
    /// <param name="OnDisplayFail">Callback once the ad was unable to be displayed</param>
    /// <param name="parentTransform">Parent Transform of the Native ad</param>
    public void ShowNativeAd(AdSDK adSDK, NativeAdSize nativeAdSize = NativeAdSize.LARGE,
        Action OnDisplaySuccess = null, Action OnDisplayFail = null, Transform parentTransform = null)
    {
        _eventManager.Publish(EventTopic.ShowNativeAd, new ShowNativeAdMessage(adSDK, nativeAdSize,
            () => AddJob(OnDisplaySuccess),
            () => AddJob(OnDisplayFail),
            parentTransform
        ));
    }

    /// <summary>
    /// Preload the assigned ad type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="adSDK">Ad SDK to request</param> 
    /// <param name="OnLoadSuccess"></param>
    /// <param name="OnLoadFail"></param>
    public void LoadAd(AdType type, AdSDK adSDK = AdSDK.All, Action<AdSDK> OnLoadSuccess = null,
        Action OnLoadFail = null)
    {
        if (adSDK == AdSDK.All)
        {
            // Start from SDK no 1
            TryAllSDKToLoadAd(adSDK.Next(), type, OnLoadSuccess, OnLoadFail);
        }
        else
        {
            _eventManager.Publish(EventTopic.LoadAd, new LoadAdMessage(adSDK, type,
                () => AddJob(() => OnLoadSuccess(adSDK)),
                () => AddJob(OnLoadFail)
            ));
        }
    }

    private void TryAllSDKToLoadAd(AdSDK currentAdSDK, AdType type, Action<AdSDK> OnLoadSuccess = null,
        Action OnLoadFail = null)
    {
        // Debug.Log("AdManager.TryAllSDKToLoadAd");
        _eventManager.Publish(EventTopic.LoadAd, new LoadAdMessage(currentAdSDK, type,
            () => AddJob(() => OnLoadSuccess(currentAdSDK)),
            () =>
            {
                Debug.LogWarning("AdManager.TryAllSDKToLoadAd: " + currentAdSDK.ToString());
                AdSDK nextSDK = currentAdSDK.Next();
                if ((int)nextSDK != 0)
                    TryAllSDKToLoadAd(nextSDK, type, OnLoadSuccess, OnLoadFail);
                else
                    AddJob(OnLoadFail);
            }
        ));
    }

    /// <summary>
    /// Method to show an interstitial ad
    /// </summary>
    /// <param name="adSDK">Ad SDK to request</param>
    /// <param name="OnClose">Callback once the ad is closed</param>
    /// <param name="OnDisplayFail">Callback once the ad was unable to be displayed</param>
    public void ShowInterstitial(AdSDK adSDK, Action OnClose = null, Action OnDisplayFail = null)
    {
        // Debug.Log("AdManager.ShowInterstitial");
        _eventManager.Publish(EventTopic.ShowInterstitial, new ShowInterstitialMessage(adSDK,
            () => AddJob(OnClose),
            () => AddJob(OnDisplayFail)
        ));
    }

    /// <summary>
    /// Method to show a rewarded ad
    /// </summary>
    /// <param name="adSDK">Ad SDK to request</param> 
    /// <param name="OnSuccess">Callback once the add has been successfully shown</param>
    /// <param name="OnFail">Callback once the add has been closed before watching completely</param>
    /// <param name="OnClose">Callback once the ad is closed (Also gets triggered once the ad has been successfully shown)</param>
    /// <param name="OnDisplayFail">Callback once the ad was unable to be displayed</param>
    public void ShowRewarded(AdSDK adSDK, Action<string, double> OnSuccess = null, Action OnFail = null, Action OnClose = null,
        Action OnDisplayFail = null)
    {
        _eventManager.Publish(EventTopic.ShowRewarded, new ShowRewardedMessage(adSDK,
            (rewardType, rewardAmount) => AddJob(() =>OnSuccess?.Invoke(rewardType, rewardAmount)),
            () => AddJob(OnFail),
            () => AddJob(OnClose),
            () => AddJob(OnDisplayFail)
        ));
    }

    /// <summary>
    /// Method to show a banner at the defined position
    /// </summary>
    /// <param name="adSDK">Ad SDK to request the banner</param> 
    /// <param name="bannerSize">Size of the banner</param> 
    /// <param name="bannerPosition">Position of the banner</param>
    /// <param name="OnLoadFail">Callback once the ad was unable to load</param>
    /// <param name="OnDisplayFail">Callback once the ad was unable to be displayed</param>
    public void ShowBanner(AdSDK adSDK = AdSDK.All, BannerSize bannerSize = BannerSize.BANNER,
        BannerPosition bannerPosition = BannerPosition.BOTTOM, Action OnLoadFail = null, Action OnDisplayFail = null)
    {
        if (adSDK == AdSDK.All)
        {
            // Start from SDK no 1
            TryAllSDKToShowBanner(adSDK.Next(), bannerSize, bannerPosition, OnLoadFail, OnDisplayFail);
        }
        else
        {
            _eventManager.Publish(EventTopic.ShowBanner, new ShowBannerMessage(adSDK, bannerSize, bannerPosition,
                () => AddJob(OnLoadFail),
                () => AddJob(OnDisplayFail)
            ));
        }
    }

    /// <summary>
    /// Method to show a banner at the defined position using all available ad SDK based on priority
    /// </summary>
    /// <param name="currentAdSDK"></param>
    /// <param name="bannerSize">Size of the banner</param> 
    /// <param name="bannerPosition">Position of the banner</param>
    /// <param name="OnLoadFail">Callback once the ad was unable to load</param>
    /// <param name="OnDisplayFail">Callback once the ad was unable to be displayed</param>
    private void TryAllSDKToShowBanner(AdSDK currentAdSDK, BannerSize bannerSize = BannerSize.BANNER,
        BannerPosition bannerPosition = BannerPosition.BOTTOM, Action OnLoadFail = null, Action OnDisplayFail = null)
    {
        _eventManager.Publish(EventTopic.ShowBanner, new ShowBannerMessage(currentAdSDK, bannerSize, bannerPosition,
            () =>
            {
                AdSDK nextSDK = currentAdSDK.Next();
                if ((int)nextSDK != 0)
                    TryAllSDKToShowBanner(nextSDK, bannerSize, bannerPosition, OnLoadFail, OnDisplayFail);
                else
                    AddJob(OnLoadFail);
            },
            () => AddJob(OnDisplayFail)
        ));
    }

    public void ChangeBannerPosition(AdSDK adSDK, BannerPosition bannerPosition)
    {
        _eventManager.Publish(EventTopic.ChangeBannerPosition, new BannerAdEventMessage(adSDK, bannerPosition));
    }

    /// <summary>
    /// Method to hide the banner which is current active
    /// </summary>
    public void HideBanner()
    {
        _eventManager.Publish(EventTopic.HideBanner, new Message());
    }

    /// <summary>
    /// Method to hide the banner which is current active
    /// </summary>
    public void DestroyBanner()
    {
        _eventManager.Publish(EventTopic.DestroyBanner, new Message());
    }

    /// <summary>
    /// Method to hide the banner which is current active
    /// </summary>
    public void DestroyNativeAd()
    {
        _eventManager.Publish(EventTopic.DestroyNativeAd, new Message());
    }

    private void ClearJobs()
    {
        _queuedThreadAction?.Clear();
    }

    public void AddJob(Action action)
    {
        _queuedThreadAction?.Enqueue(action);
    }

    protected void Update()
    {
        if (_queuedThreadAction == null) return;
        while (_queuedThreadAction.Count > 0)
            _queuedThreadAction.Dequeue()?.Invoke();
    }
}