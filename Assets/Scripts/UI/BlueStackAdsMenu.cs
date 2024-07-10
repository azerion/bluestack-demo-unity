using System;
using System.Collections.Generic;
using Azerion.BlueStack.API;
using UnityEngine;
using UnityEngine.UI;

public class BlueStackAdsMenu : MonoBehaviour
{
    private GameObject adButtons;
    private GameObject blueStackSettings;
    private Toggle adPreferenceToggle;
    private GameObject bannerAdOptions;
    private Dropdown bannerAdPositionDropdown;
    private GameObject nativeAdOptions;
    private GameObject showLrgNativeAdButton;
    private GameObject showSmlNativeAdButton;
    private GameObject removeNativeAdButton;

    private GameObject showInterstitialAdButton;
    private GameObject showRewardedAdButton;

    private Text interstitialTitleText;
    private Text rewardedTitleText;
    
    private EventManager _eventManager;
    private List<Subscription> _subscriptions;

    private void Awake()
    {
        bannerAdPositionDropdown = GameObject.Find("BannerPositionDropdown").GetComponent<Dropdown>();
        bannerAdPositionDropdown.value = 1;
        bannerAdPositionDropdown.onValueChanged.AddListener(delegate { OnAdPositionChange(); });

        blueStackSettings = GameObject.Find("BlueStackSettings");
        adPreferenceToggle = blueStackSettings.transform.Find("Preference").GetComponent<Toggle>();

        adButtons = GameObject.Find("AdButtons");
        bannerAdOptions = GameObject.Find("BannerAdOptions");
        
        nativeAdOptions = GameObject.Find("NativeAdOptions");
        showLrgNativeAdButton = GameObject.Find("ShowLrgNativeAdButton"); 
        showSmlNativeAdButton = GameObject.Find("ShowSmlNativeAdButton"); 
        removeNativeAdButton = GameObject.Find("RemoveNativeAdButton"); 
        
        interstitialTitleText = GameObject.Find("InterstitialTitle/Text").GetComponent<Text>();
        rewardedTitleText = GameObject.Find("RewardedTitle/Text").GetComponent<Text>();
        
        SetupShowInterstitialAdButton();
        SetupShowRewardedAdButton();
    }

    private void Start()
    {
        ShowBlueStackSettings(false);
        ShowBannerAdOptions(false);
        ShowNativeDisplayOptions(false);
        UpdateInterstitialPreLoadStatus(false);
        UpdateRewardedPreLoadStatus(false);
    }

    public void ShowBlueStackSettings(bool value)
    {
        blueStackSettings.SetActive(value);
        adPreferenceToggle.isOn = PlayerPrefs.GetInt("LoadWithPreference") == 1 ? true : false;
        ContentFitterRefresh.Instance.RefreshContentFitters();
    }
    
    public void ShowAdsMenu()
    {
        gameObject.SetActive(true);
        ContentFitterRefresh.Instance.RefreshContentFitters();
    }

#region Banner Ad
    
    private void ShowBannerAdOptions(bool value)
    {
        ResetPositionDropdown();
        bannerAdOptions.SetActive(value);
        ContentFitterRefresh.Instance.RefreshContentFitters();
    }
    
    public void ShowBanner()
    {
        AdManager.Instance.ShowBanner(AdSDK.Bluestack);
    }

    public void RemoveBanner()
    {
        AdManager.Instance.DestroyBanner();
    }

    public void ShowDynamicBanner()
    {
        AdManager.Instance.ShowBanner(AdSDK.Bluestack, BannerSize.SMART, BannerPosition.BOTTOM);
    }

    public void ShowLargeBanner()
    {
        AdManager.Instance.ShowBanner(AdSDK.Bluestack, BannerSize.LARGE, BannerPosition.BOTTOM);
    }

    public void ShowFullBanner()
    {
        AdManager.Instance.ShowBanner(AdSDK.Bluestack, BannerSize.FULL, BannerPosition.BOTTOM);
    }

    public void ShowLeaderboardBanner()
    {
        AdManager.Instance.ShowBanner(AdSDK.Bluestack, BannerSize.LEADERBOARD, BannerPosition.BOTTOM);
    }

    public void ShowMediumRectangleBanner()
    {
        AdManager.Instance.ShowBanner(AdSDK.Bluestack, BannerSize.MEDIUM_RECTANGLE, BannerPosition.BOTTOM);
    }
    
    private void OnAdPositionChange()
    {
        AdManager.Instance.ChangeBannerPosition(AdSDK.Bluestack,
            bannerAdPositionDropdown.value == 0 ? BannerPosition.TOP : BannerPosition.BOTTOM);
    }

    private void ResetPositionDropdown()
    {
        bannerAdPositionDropdown.value = 1;
    }
    
#endregion

#region Native Ad
    
    public void LoadNativeAd()
    {
        DialogManager.Instance.CallNetworkAccessDialog(true);
        AdManager.Instance.LoadAd(AdType.Native, AdSDK.Bluestack, OnNativeAdLoadSuccess, OnNativeAdLoadFail);
    }
    
    public void ShowLargeNativeAd()
    {
        AdManager.Instance.ShowNativeAd(AdSDK.Bluestack, NativeAdSize.LARGE,
            OnNativeDisplaySuccess, OnNativeDisplayFail, this.transform);
    }

    public void ShowSmallNativeAd()
    {
        AdManager.Instance.ShowNativeAd(AdSDK.Bluestack, NativeAdSize.SMALL,
            OnNativeDisplaySuccess, OnNativeDisplayFail, this.transform);
    }

    private void OnNativeAdLoadSuccess(AdSDK adSDK)
    {
        if (adSDK != AdSDK.Bluestack) return;
        DialogManager.Instance.CallNetworkAccessDialog(false);
        ShowNativeDisplayOptions(true);
        Debug.Log("Successfully Loaded Native Ad");
    }

    private void OnNativeAdLoadFail()
    {
        Debug.Log("Failed to Load Native Ad!");
        DialogManager.Instance.CallNetworkAccessDialog(false);
    }
    
    private void OnNativeDisplaySuccess()
    {
        Debug.Log("Successfully displayed Native Ad!");
    }

    private void OnNativeDisplayFail()
    {
        Debug.Log("Failed to display Native Ad");
    }

    public void RemoveNativeAd()
    {
        AdManager.Instance.DestroyNativeAd();
    }
    
    private void ShowNativeDisplayOptions(bool value)
    {
        nativeAdOptions.SetActive(value);
        showLrgNativeAdButton.SetActive(value);
        showSmlNativeAdButton.SetActive(value);
        removeNativeAdButton.SetActive(false);
        ContentFitterRefresh.Instance.RefreshContentFitters();
    }

    private void ShowNativeRemoveOption(bool value)
    {
        nativeAdOptions.SetActive(value);
        removeNativeAdButton.SetActive(value);
        showLrgNativeAdButton.SetActive(false);
        showSmlNativeAdButton.SetActive(false);
        ContentFitterRefresh.Instance.RefreshContentFitters();
    }
    
#endregion

#region Interstitial Ad
    
    public void LoadInterstitial()
    {
        RemoveBanner();
        DialogManager.Instance.CallNetworkAccessDialog(true);
        AdManager.Instance.LoadAd(AdType.Interstitial, AdSDK.Bluestack, OnInterstitialLoadSuccess,
            OnInterstitialLoadFail);
    }

    private void OnInterstitialLoadSuccess(AdSDK adSDK)
    {
        if (adSDK != AdSDK.Bluestack) return;
        DialogManager.Instance.CallNetworkAccessDialog(false);
        UpdateInterstitialPreLoadStatus(true);
    }

    private void OnInterstitialClose()
    {
        UpdateInterstitialPreLoadStatus(false);
    }

    private void OnInterstitialDisplayFail()
    {
        UpdateInterstitialPreLoadStatus(false);
    }

    private void OnInterstitialLoadFail()
    {
        UpdateInterstitialPreLoadStatus(false);
        DialogManager.Instance.CallNetworkAccessDialog(false);
    }
    
    private void SetupShowInterstitialAdButton()
    {
        showInterstitialAdButton = GameObject.Find("InterstitialAdButtons/DisplayAdButton");
        showInterstitialAdButton.GetComponent<Button>().onClick.RemoveAllListeners();
        showInterstitialAdButton.GetComponent<Button>().onClick.AddListener(() =>
            AdManager.Instance.ShowInterstitial(AdSDK.Bluestack, OnInterstitialClose, OnInterstitialDisplayFail)
        );
    }
    
    private void UpdateInterstitialPreLoadStatus(bool isLoaded)
    {
        interstitialTitleText.text = isLoaded ? "INTERSTITIAL AD (Loaded)" : "INTERSTITIAL AD";
    }

#endregion
    
#region Rewarded Ad
    
    public void LoadRewarded()
    {
        RemoveBanner();
        DialogManager.Instance.CallNetworkAccessDialog(true);
        AdManager.Instance.LoadAd(AdType.RewardedVideo, AdSDK.Bluestack, OnRewardedLoadSuccess, OnRewardedLoadFail);
    }

    private void SetupShowRewardedAdButton()
    {
        showRewardedAdButton = GameObject.Find("RewardedAdButtons/DisplayAdButton");
        showRewardedAdButton.GetComponent<Button>().onClick.RemoveAllListeners();
        showRewardedAdButton.GetComponent<Button>().onClick.AddListener(() =>
            AdManager.Instance.ShowRewarded(AdSDK.Bluestack, OnRewardedSuccess, OnRewardedFail, OnRewardedClose, OnRewardedDisplayFail)
        );
    }
    
    private void OnRewardedLoadSuccess(AdSDK adSDK)
    {
        if (adSDK != AdSDK.Bluestack) return;
        DialogManager.Instance.CallNetworkAccessDialog(false);
        UpdateRewardedPreLoadStatus(true);
    }

    private void OnRewardedLoadFail()
    {
        DialogManager.Instance.CallNetworkAccessDialog(false);
    }
    
    private void OnRewardedDisplayFail()
    {
        UpdateRewardedPreLoadStatus(false);
    }
    
    private void OnRewardedSuccess(string rewardType, double rewardAmount)
    {
        Debug.Log("received reward!");
        DialogManager.Instance.CallSystemDialog("Reward received!", "Successfully received reward!\n" + rewardType + " : " + rewardAmount, SystemDialog.AppearanceType.Information, null);    }

    private void OnRewardedFail()
    {
        Debug.Log("Failed to achieve reward!");
        DialogManager.Instance.CallSystemDialog("Reward failed!", "Failed to achieve reward!", SystemDialog.AppearanceType.Information, null);
    }

    private void OnRewardedClose()
    {
        UpdateRewardedPreLoadStatus(false);
    }

    private void UpdateRewardedPreLoadStatus(bool isLoaded)
    {
        rewardedTitleText.text = isLoaded ? "REWARDED AD (Loaded)" : "REWARDED AD";
    }
    
#endregion
    
    private void HandleAdLoaded(AdSDK adSDK, AdType adType)
    {
        if(adType == AdType.RewardedVideo)
            OnRewardedLoadSuccess(adSDK);
        else if(adType == AdType.Interstitial)
            OnInterstitialLoadSuccess(adSDK);
    }

    public void OnEnable()
    {
        AdManager.OnAdLoaded += HandleAdLoaded;
        
        _eventManager = EventManager.Instance;
        _subscriptions = new List<Subscription>();

        _subscriptions.Add(_eventManager.Subscribe(EventTopic.BannerAdDisplayed,
            (BannerAdEventMessage message) =>
            {
                ShowBannerAdOptions(true);
            }
        ));

        _subscriptions.Add(_eventManager.Subscribe(EventTopic.DestroyBanner, (Message message) =>
            {
                ShowBannerAdOptions(false);
            }
        ));

        _subscriptions.Add(_eventManager.Subscribe(EventTopic.AdDisplayed,
            (AdEventMessage message) =>
            {
                if (message.Type == AdType.Native)
                {
                    ShowNativeRemoveOption(true);
                }
            }
        ));
        
        _subscriptions.Add(_eventManager.Subscribe(EventTopic.DestroyNativeAd, (Message message) =>
            {
                ShowNativeRemoveOption(false);
            }
        ));

        UIViewController.ActivateSettingsButton(() => ShowBlueStackSettings(true));
    }

    public void OnDisable()
    {
        foreach (var subscription in _subscriptions)
        {
            subscription.Remove();
        }

        _subscriptions = new List<Subscription>();

        UIViewController.DeactivateSettingsButton();
    }
}