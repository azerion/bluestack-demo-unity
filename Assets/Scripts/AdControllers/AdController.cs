using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum BannerPosition
{
    NULL,
    TOP,
    BOTTOM
}

public enum BannerSize
{
    NULL,
    BANNER,
    LEADERBOARD,
    FULL,
    LARGE,
    SMART,
    MEDIUM_RECTANGLE
}

public enum NativeAdSize
{
    NULL,
    LARGE,
    SMALL
}

public abstract class AdController<T> : MonoBehaviour where T : Component
{
    protected abstract AdSDK adSDK { get; }
    public abstract bool isInitialized { get; protected set; }

    protected bool _bannerAdLoaded = false;
    protected bool _interstitialAdLoaded = false;
    protected bool _rewardedAdLoaded = false;
    protected bool _nativeAdLoaded = false;

    protected bool _bannerAdDisplaying = false;
    protected bool _interstitialAdDisplaying = false;
    protected bool _rewardedAdDisplaying = false;
    protected bool _nativeAdDisplaying = false;

    protected EventManager _eventManager;
    protected List<Subscription> _subscriptions;

    private static T _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    protected void OnEnable()
    {
        _eventManager = EventManager.Instance;
        _subscriptions = new List<Subscription>();

        _subscriptions.Add(_eventManager.Subscribe(EventTopic.InitializeAdSDK,
            (Message message) => { InitializeAdSDK(); }
        ));

        _subscriptions.Add(_eventManager.Subscribe(EventTopic.CloseAd, (Message message) =>
            {
                DestroyBanner();
            }
        ));

        _subscriptions.Add(_eventManager.Subscribe(EventTopic.LoadAd, (LoadAdMessage message) =>
            {
                if (message.Subject != adSDK)
                {
                    return;
                }
                PreloadAd(message.Type, message.OnLoadSuccess, message.OnLoadFail);
            }
        ));

        _subscriptions.Add(_eventManager.Subscribe(EventTopic.ShowNativeAd, (ShowNativeAdMessage message) =>
            {
                if (message.Subject != adSDK)
                {
                    return;
                }
                
                ShowNativeAd(message.NativeAdSize, message.OnDisplaySuccess, message.OnDisplayFail,
                    message.ParentTransform);
            }
        ));

        _subscriptions.Add(_eventManager.Subscribe(EventTopic.ShowInterstitial, (ShowInterstitialMessage message) =>
            {
                if (message.Subject != adSDK)
                {
                    return;
                }

                ShowInterstitial(message.OnClose, message.OnDisplayFail);
            }
        ));

        _subscriptions.Add(_eventManager.Subscribe(EventTopic.ShowRewarded, (ShowRewardedMessage message) =>
            {
                if (message.Subject != adSDK)
                {
                    return;
                }

                ShowRewarded(message.OnSuccess, message.OnFail, message.OnClose, message.OnDisplayFail);
            }
        ));

        _subscriptions.Add(_eventManager.Subscribe(EventTopic.ShowBanner, (ShowBannerMessage message) =>
            {
                if (message.Subject == adSDK)
                    ShowBanner(message.BannerSize, message.BannerPosition, message.OnLoadFail, message.OnDisplayFail);
            }
        ));

        _subscriptions.Add(_eventManager.Subscribe(EventTopic.ChangeBannerPosition, (BannerAdEventMessage message) =>
            {
                if (message.Subject == adSDK)
                    ChangeBannerPosition(message.BannerPosition);
            }
        ));

        _subscriptions.Add(_eventManager.Subscribe(EventTopic.HideBanner, (Message message) => { HideBanner(); }
        ));

        _subscriptions.Add(_eventManager.Subscribe(EventTopic.DestroyBanner, (Message message) => { DestroyBanner(); }
        ));

        _subscriptions.Add(_eventManager.Subscribe(EventTopic.DestroyNativeAd,
            (Message message) => { DestroyNativeAd(); }
        ));
    }

    protected void OnDisable()
    {
        if (_subscriptions == null)
            return;

        foreach (var subscription in _subscriptions)
        {
            subscription.Remove();
        }

        _subscriptions = new List<Subscription>();
    }

    // Initialize the Ads SDK.
    public abstract void InitializeAdSDK();

    /// <summary>
    /// Method which is called after Ad SDK initialization has been started.
    /// </summary>
    protected void FireOnAdSDKInitializationStart()
    {
        _eventManager.Publish(EventTopic.AdSDKInitializationStatus,
            new AdSDKInitializationMessage { Subject = adSDK, Status = "Start" });
        AdManager.Instance.AddJob(() => DialogManager.Instance.CallEventDialog(adSDK + " initialization started"));
    }

    /// <summary>
    /// Method which is called after Ad SDK initialization has been completed.
    /// </summary>
    protected void FireOnAdSDKInitializationComplete()
    {
        Debug.Log("Fire On AdSDK Initialization Complete");
        _eventManager.Publish(EventTopic.AdSDKInitializationStatus,
            new AdSDKInitializationMessage { Subject = adSDK, Status = "Complete" });
        AdManager.Instance.AddJob(() => DialogManager.Instance.CallEventDialog(adSDK + " initialization completed"));
    }

    /// <summary>
    /// Method which is called after an Ad has been loaded.
    /// </summary>
    protected void FireOnAdLoaded(AdType adType)
    {
        _eventManager.Publish(EventTopic.AdLoaded, new AdEventMessage { Subject = adSDK, Type = adType });
        AdManager.Instance.AddJob(() => DialogManager.Instance.CallEventDialog(adSDK + " " + adType + " ad loaded"));
    }

    /// <summary>
    /// Method which is called after any Ad has been failed to load.
    /// </summary>
    protected void FireOnAdFailedToLoad(AdType adType)
    {
        _eventManager.Publish(EventTopic.AdFailedToLoad, new AdEventMessage { Subject = adSDK, Type = adType });
        AdManager.Instance.AddJob(() =>
            DialogManager.Instance.CallEventDialog(adSDK + " " + adType + " ad failed to load"));
    }

    /// <summary>
    /// Method which is called after the Ad has been displayed.
    /// </summary>
    protected void FireOnAdDisplayed(AdType adType, BannerPosition bannerPosition = BannerPosition.BOTTOM)
    {
        _eventManager.Publish(EventTopic.AdDisplayed, new AdEventMessage { Subject = adSDK, Type = adType });
        AdManager.Instance.AddJob(() => DialogManager.Instance.CallEventDialog(adSDK + " " + adType + " ad displayed"));
    }

    /// <summary>
    /// Method which is called after the banner Ad has been displayed.
    /// </summary>
    protected void FireOnBannerAdDisplayed(AdType adType, BannerPosition bannerPosition = BannerPosition.BOTTOM,
        int height = 0, int width = 0)
    {
        _eventManager.Publish(EventTopic.BannerAdDisplayed,
            new BannerAdEventMessage
                { Subject = adSDK, BannerPosition = bannerPosition, Height = height, Width = width });
        AdManager.Instance.AddJob(() => DialogManager.Instance.CallEventDialog(adSDK + " " + adType + " ad displayed"));
    }

    /// <summary>
    /// Method which is called after the banner Ad position been changed.
    /// </summary>
    protected void FireOnBannerAdPositionChanged(AdType adType, BannerPosition bannerPosition = BannerPosition.BOTTOM,
        int height = 0, int width = 0)
    {
        _eventManager.Publish(EventTopic.BannerAdPositionChanged,
            new BannerAdEventMessage
                { Subject = adSDK, BannerPosition = bannerPosition, Height = height, Width = width });
        AdManager.Instance.AddJob(() =>
            DialogManager.Instance.CallEventDialog(adSDK + " " + adType + " ad position changed"));
    }

    /// <summary>
    /// Method which is called when the ad leaving the app.
    /// </summary>
    protected void FireOnAdLeavingApplication(AdType adType)
    {
        _eventManager.Publish(EventTopic.AdLeavingApplication, new AdEventMessage { Subject = adSDK, Type = adType });
        AdManager.Instance.AddJob(
            () => DialogManager.Instance.CallEventDialog(adSDK + " " + adType + " ad leaving App"));
    }

    /// <summary>
    /// Method which is called after the Ad has been closed.
    /// </summary>
    protected void FireOnAdClosed(AdType adType)
    {
        _eventManager.Publish(EventTopic.AdClosed, new AdEventMessage { Subject = adSDK, Type = adType });
        AdManager.Instance.AddJob(() => DialogManager.Instance.CallEventDialog(adSDK + " " + adType + " ad closed"));
    }

    /// <summary>
    /// Method which is called after the Ad has been failed to display.
    /// </summary>
    protected void FireOnAdFailedToDisplay(AdType adType)
    {
        _eventManager.Publish(EventTopic.AdFailedToDisplay, new AdEventMessage { Subject = adSDK, Type = adType });
        AdManager.Instance.AddJob(() =>
            DialogManager.Instance.CallEventDialog(adSDK + " " + adType + " ad failed to display"));
    }

    /// <summary>
    /// Method which is called after the Ad Reward Earned.
    /// </summary>
    protected void FireOnAdRewardSuccess(AdType adType)
    {
        _eventManager.Publish(EventTopic.AdRewardEarned, new AdEventMessage { Subject = adSDK, Type = adType });
        AdManager.Instance.AddJob(() =>
            DialogManager.Instance.CallEventDialog(adSDK + " " + adType + " ad reward success"));
    }

    /// <summary>
    /// Method which is called after the Ad Reward Earned.
    /// </summary>
    protected void FireOnAdRewardFail(AdType adType)
    {
        _eventManager.Publish(EventTopic.AdRewardEarned, new AdEventMessage { Subject = adSDK, Type = adType });
        AdManager.Instance.AddJob(
            () => DialogManager.Instance.CallEventDialog(adSDK + " " + adType + " ad reward fail"));
    }

    /// <summary>
    /// Create the assigned ad type (fullscreen ads)
    /// </summary>
    /// <param name="type"></param>
    protected abstract void CreateAd(AdType type);

    /// <summary>
    /// Preload the assigned ad type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="OnLoadSuccess"></param>
    /// <param name="OnLoadFail"></param>
    public abstract void PreloadAd(AdType type, Action OnLoadSuccess = null, Action OnLoadFail = null);

    /// <summary>
    /// Method to load a native ad
    /// </summary>
    /// <param name="OnLoadSuccess"></param>
    /// <param name="OnLoadFail"></param>
    protected abstract void LoadNativeAd(Action OnLoadSuccess = null, Action OnLoadFail = null);

    /// <summary>
    /// Method to show an native ad
    /// </summary>
    /// <param name="OnDisplayFail">Callback once the ad was unable to be displayed</param>
    /// <param name="OnAdDisplayed">Callback once the ad is displayed</param>
    /// <param name="parentTransform">Parent Transform of the Native ad</param>
    protected abstract void ShowNativeAd(NativeAdSize size, Action OnAdDisplaySuccess = null,
        Action OnDisplayFail = null, Transform parentTransform = null);

    /// <summary>
    /// Method to load an interstitial ad
    /// </summary>
    /// <param name="OnLoadSuccess">Callback once the ad has been loaded succefully</param>
    /// <param name="OnLoadFail">Callback once the ad has the loading process has failed</param>
    protected abstract void LoadInterstitial(Action OnLoadSuccess = null, Action OnLoadFail = null);


    /// <summary>
    /// Method to show an interstitial ad
    /// </summary>
    /// <param name="OnDisplayFail">Callback once the ad was unable to be displayed</param>
    /// <param name="OnClose">Callback once the ad is closed</param>
    public abstract void ShowInterstitial(Action OnClose = null, Action OnDisplayFail = null);

    /// <summary>
    /// Method to load a rewarded ad
    /// </summary>
    /// <param name="OnLoadSuccess">Callback once the ad has been loaded succefully</param>
    /// <param name="OnLoadFail">Callback once the ad has the loading process has failed</param>
    protected abstract void LoadRewarded(Action OnLoadSuccess = null, Action OnLoadFail = null);

    /// <summary>
    /// Method to show a rewarded ad
    /// </summary>
    /// <param name="OnSuccess">Callback once the add has been succefully shown</param>
    /// <param name="OnFail">Callback once the add has been closed before watching completely</param>
    /// <param name="OnClose">Callback once the ad is closed (Also gets triggered once the ad has been succefully shown)</param>
    /// <param name="OnDisplayFail">Callback once the ad was unable to be displayed</param>
    public abstract void ShowRewarded(Action<string, double> OnSuccess = null, Action OnFail = null, Action OnClose = null,
        Action OnDisplayFail = null);

    /// <summary>
    /// Method to show a banner at the defined position
    /// </summary>
    /// <param name="bannerPosition">Size of the banner</param>
    /// <param name="bannerPosition">Position of the banner</param>
    public abstract void ShowBanner(BannerSize bannerSize, BannerPosition bannerPosition, Action OnLoadFail = null,
        Action OnDisplayFail = null);

    public abstract void ChangeBannerPosition(BannerPosition bannerPosition);

    /// <summary>
    /// Method to hide the banner which is current active
    /// </summary>
    public abstract void HideBanner();

    /// <summary>
    /// Method to destroy the banner which is current active
    /// </summary>
    public abstract void DestroyBanner();

    /// <summary>
    /// Method to destroy the Native ad which is current active
    /// </summary>
    public abstract void DestroyNativeAd();


    ///// <summary>
    ///// Method to check if a specific ad type is available
    ///// </summary>
    ///// <param name="type">Type of ad to check</param>
    ///// <returns>Availability of ad type</returns>
    public bool IsAdAvailable(AdType type)
    {
        switch (type)
        {
            case AdType.RewardedVideo:
                return _rewardedAdLoaded;
            case AdType.Interstitial:
                return _interstitialAdLoaded;
            case AdType.Banner:
                return _bannerAdLoaded;
            default:
                Debug.LogError("AdManager: Unable to check availability ad type: " + type);
                return false;
        }
    }

    /// <summary>
    /// Method to check if a specific ad type is currently displaying
    /// </summary>
    /// <param name="type">Type of ad to check</param>
    /// <returns>Display status of ad type</returns>
    public bool IsAdDisplaying(AdType type)
    {
        switch (type)
        {
            case AdType.RewardedVideo:
                return _rewardedAdDisplaying;
            case AdType.Interstitial:
                return _interstitialAdDisplaying;
            case AdType.Banner:
                return _bannerAdDisplaying;
            default:
                Debug.LogError("AdManager: Unable to check display status ad type: " + type);
                return false;
        }
    }
}