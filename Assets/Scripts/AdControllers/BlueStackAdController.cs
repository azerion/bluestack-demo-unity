using System;
using System.Collections.Generic;
using Azerion.BlueStack.API.Rewarded;
using Azerion.BlueStack.API;
using Azerion.BlueStack.API.Banner;
using UnityEngine;

namespace AdControllers
{
    public class BlueStackAdController : AdController<BlueStackAdController>
    {
        protected override AdSDK adSDK => AdSDK.Bluestack;
        public override bool isInitialized { get; protected set; }

        private BannerAd _bannerAd;
        private InterstitialAd _interstitialAd;
        private RewardedVideoAd _rewardedVideo;
        private NativeAdLoader _nativeAdLoader;
        private NativeAd _nativeAd;
        private bool _isRewardSuccess;
        private Preference _preference;
        private bool _shouldLoadWithPreference = true;
        private bool _isBannerShowing = false;

        private BannerSize _displayedBannerSize = BannerSize.NULL;
        private BannerPosition _displayedBannerPosition = BannerPosition.NULL;

        private GameObject _nativeAdPrefab = null;
        private static readonly string _largeNativeAdPrefabsPath = "NativeAds/LargeNativeAd";
        private static readonly string _smallNativeAdPrefabsPath = "NativeAds/SmallNativeAd";

        private string appId
        {
            get
            {
#if UNITY_IOS
                return "3180317"; 
#elif UNITY_ANDROID
                return "5180317"; 
#else
                return "unexpected_platform";
#endif
            }
        }

        private string bannerId
        {
            get
            {
#if UNITY_IOS
                return "/" + appId + "/banner";
#elif UNITY_ANDROID
                return "/" + appId + "/banner";
#else
                return "unexpected_platform";
#endif
            }
        }

        private string nativeId
        {
            get
            {
#if UNITY_IOS
                return "/" + appId + "/nativead";
#elif UNITY_ANDROID
                return "/" + appId + "/nativead";
#else
                return "unexpected_platform";
#endif
            }
        }

        private string interstitialId
        {
            get
            {
#if UNITY_IOS
                return "/" + appId + "/interstitial";
#elif UNITY_ANDROID
                return "/" + appId + "/interstitial";
#else
                return "unexpected_platform";
#endif
            }
        }

        private string rewardedId
        {
            get
            {
#if UNITY_IOS
                return "/" + appId + "/rewardvideo"; 
#elif UNITY_ANDROID
                return "/" + appId + "/videoRewarded";
#else
                return "unexpected_platform";
#endif
            }
        }

#region SDK Initialization
        public override void InitializeAdSDK()
        {
            Debug.Log("BlueStackAdController: InitializeAdSDK");
            Settings settings = new Settings(isDebugModeEnabled: true);
            _preference = createBSPreference();
            BlueStackAds.Initialize(appId, settings, HandleSDKInitCompleteAction, HandleAdaptersInitCompleteAction);
        }

        private void HandleSDKInitCompleteAction(SDKInitializationStatus sdkInitializationStatus)
        {
            bool isSuccess = sdkInitializationStatus.IsSuccess;
            string description = sdkInitializationStatus.Description;

            if (isSuccess)
            {
                Debug.Log("BlueStack SDK initialized: " + description);
                isInitialized = true;
                FireOnAdSDKInitializationComplete();
                
                ShowBanner(BannerSize.BANNER, BannerPosition.BOTTOM);
                CreateAd(AdType.Interstitial);
                CreateAd(AdType.RewardedVideo);
                
                // Preload
                // PreloadAd(AdType.RewardedVideo);
                // PreloadAd(AdType.Interstitial);
            }
            else
            {
                isInitialized = false;
                Debug.Log("BlueStack SDK initialization failed: " + description);
            }
        }

        private void HandleAdaptersInitCompleteAction(AdaptersInitializationStatus adaptersInitializationStatus)
        {
            foreach (KeyValuePair<string, AdapterStatus> adapterStateEntry in adaptersInitializationStatus
                         .GetAdapterStatusMap())
            {
                Debug.Log("Adapter Name: " + adapterStateEntry.Value.Name + ", " +
                          "Adapter State: " + adapterStateEntry.Value.InitializationState + ", " +
                          "Adapter Description: " + adapterStateEntry.Value.Description);
            }
        }
#endregion

        protected override void CreateAd(AdType type)
        {
            switch (type)
            {
                case AdType.RewardedVideo:
                    _rewardedVideo = new RewardedVideoAd(rewardedId);
                    break;

                case AdType.Interstitial:
                    _interstitialAd = new InterstitialAd(interstitialId);
                    break;

                case AdType.Native:
                    _nativeAdLoader = new NativeAdLoader(nativeId);
                    break;

                default:
                    Debug.LogError("Unable to create ad with type: " + type);
                    break;
            }
        }

        public override void PreloadAd(AdType type, Action OnLoadSuccess = null, Action OnLoadFail = null)
        {
            if (!isInitialized)
            {
                OnLoadFail?.Invoke();
                return;
            }

            switch (type)
            {
                case AdType.RewardedVideo:
                    if (_rewardedVideo == null)
                    {
                        CreateAd(AdType.RewardedVideo);
                    }

                    LoadRewarded(OnLoadSuccess, OnLoadFail);
                    break;

                case AdType.Interstitial:
                    if (_interstitialAd == null)
                    {
                        CreateAd(AdType.Interstitial);
                    }

                    LoadInterstitial(OnLoadSuccess, OnLoadFail);
                    break;

                case AdType.Native:
                    if (_nativeAd == null)
                    {
                        CreateAd(AdType.Native);
                    }
                    else
                    {
                        DestroyNativeAd();
                        CreateAd(AdType.Native);
                    }

                    LoadNativeAd(OnLoadSuccess, OnLoadFail);
                    break;

                case AdType.Banner:
                    break;

                default:
                    Debug.LogError("Unable to request ad with type: " + type);
                    break;
            }
        }

#region Native Ad        
        protected override void LoadNativeAd(Action OnLoadSuccess = null, Action OnLoadFail = null)
        {
            EventHandler<NativeAdEventArgs> onAdLoaded = null;
            EventHandler<BlueStackError> onAdFailedToLoad = null;

            onAdLoaded = (sender, args) =>
            {
                // Debug.Log("BlueStack NativeAd successfully loaded");
                _nativeAd = args.nativeAd;
                _nativeAdLoaded = true;
                _nativeAdLoader.OnNativeAdLoaded -= onAdLoaded;
                _nativeAdLoader.OnNativeAdFailedToLoad -= onAdFailedToLoad;
                
                OnLoadSuccess?.Invoke();
                FireOnAdLoaded(AdType.Native);
            };

            onAdFailedToLoad = (sender, args) =>
            {
                Debug.Log("BlueStack NativeAd: " + "errorCode: " + args.ErrorCode + " message: " + args.Message);
                _nativeAdLoaded = false;
                _nativeAdLoader.OnNativeAdLoaded -= onAdLoaded;
                _nativeAdLoader.OnNativeAdFailedToLoad -= onAdFailedToLoad;
                
                OnLoadFail?.Invoke();
                FireOnAdFailedToLoad(AdType.Native);
            };

            _nativeAdLoader.OnNativeAdLoaded += onAdLoaded;
            _nativeAdLoader.OnNativeAdFailedToLoad += onAdFailedToLoad;
            if (_shouldLoadWithPreference)
            {
                _nativeAdLoader.Load(_preference);
            }
            else
            {
                _nativeAdLoader.Load();
            }
        }

        protected override void ShowNativeAd(NativeAdSize nativeAdSize, Action OnAdDisplayed = null,
            Action OnDisplayFail = null, Transform parentTransform = null)
        {
            if (nativeAdSize == NativeAdSize.LARGE)
            {
                if (parentTransform != null)
                {
                    _nativeAdPrefab = Instantiate(Resources.Load(_largeNativeAdPrefabsPath, typeof(GameObject)),
                        parentTransform) as GameObject;
                }
                else
                {
                    _nativeAdPrefab =
                        Instantiate(Resources.Load(_largeNativeAdPrefabsPath, typeof(GameObject))) as GameObject;
                }

                if (_nativeAdPrefab != null)
                {
                    LargeNativeAdViewer nativeAdViewer = _nativeAdPrefab.GetComponent<LargeNativeAdViewer>();
                    nativeAdViewer.Initialize(_nativeAd);
                    
                    OnAdDisplayed?.Invoke();
                    FireOnAdDisplayed(AdType.Native);
                }
                else
                {
                    OnDisplayFail?.Invoke();
                }
            }
            else if (nativeAdSize == NativeAdSize.SMALL)
            {
                if (parentTransform != null)
                {
                    _nativeAdPrefab = Instantiate(Resources.Load(_smallNativeAdPrefabsPath, typeof(GameObject)),
                        parentTransform) as GameObject;
                }
                else
                {
                    _nativeAdPrefab =
                        Instantiate(Resources.Load(_smallNativeAdPrefabsPath, typeof(GameObject))) as GameObject;
                }

                if (_nativeAdPrefab != null)
                {
                    SmallNativeAdViewer nativeAdViewer = _nativeAdPrefab.GetComponent<SmallNativeAdViewer>();
                    nativeAdViewer.Initialize(_nativeAd);
                    
                    OnAdDisplayed?.Invoke();
                    FireOnAdDisplayed(AdType.Native);
                }
                else
                {
                    OnDisplayFail?.Invoke();
                }
            }
            else
            {
                Debug.LogWarning("Wrong NativeAdSize!");
                OnDisplayFail?.Invoke();
            }
        }

        public override void DestroyNativeAd()
        {
            _nativeAdLoader?.Destroy();
            _nativeAdDisplaying = false;
            _nativeAdLoader = null;
            _nativeAd = null;
            if (_nativeAdPrefab != null) Destroy(_nativeAdPrefab);
        }
#endregion

#region Interstitial Ad
        protected override void LoadInterstitial(Action OnLoadSuccess = null, Action OnLoadFail = null)
        {
            EventHandler<EventArgs> onAdLoaded = null;
            EventHandler<BlueStackError> onAdFailedToLoad = null;

            onAdLoaded = (sender, args) =>
            {
                Debug.Log("BlueStack Interstitial successfully loaded");
                _interstitialAd.OnInterstitialDidLoaded -= onAdLoaded;
                _interstitialAd.OnInterstitialDidFail -= onAdFailedToLoad;
                _interstitialAdLoaded = true;
                
                OnLoadSuccess?.Invoke();
                FireOnAdLoaded(AdType.Interstitial);
            };

            onAdFailedToLoad = (sender, args) =>
            {
                Debug.Log("BlueStack Interstitial: " + "errorCode: " + args.ErrorCode + " message: " + args.Message);
                _interstitialAd.OnInterstitialDidLoaded -= onAdLoaded;
                _interstitialAd.OnInterstitialDidFail -= onAdFailedToLoad;
                _interstitialAdLoaded = false;
                
                OnLoadFail?.Invoke();
                FireOnAdFailedToLoad(AdType.Interstitial);
            };

            _interstitialAd.OnInterstitialDidLoaded += onAdLoaded;
            _interstitialAd.OnInterstitialDidFail += onAdFailedToLoad;
            if (_shouldLoadWithPreference)
            {
                _interstitialAd.Load(_preference);
            }
            else
            {
                _interstitialAd.Load();
            }
        }

        public override void ShowInterstitial(Action OnClose = null, Action OnDisplayFail = null)
        {
            Action OnAdPreloaded = () =>
            {
                _interstitialAdDisplaying = true;

                EventHandler<EventArgs> onAdDisplayed = null;
                onAdDisplayed = (sender, args) =>
                {
                    Debug.Log("BlueStack Interstitial Ad Displayed");
                    FireOnAdDisplayed(AdType.Interstitial);
                };

                EventHandler<EventArgs> onAdClicked = null;
                onAdClicked = (sender, args) =>
                {
                    Debug.Log("BlueStack Interstitial Ad Clicked");
                    FireOnAdLeavingApplication(AdType.Interstitial);
                };

                EventHandler<EventArgs> onAdClosed = null;

                EventHandler<BlueStackError> onAdFailedToDisplay = null;
                onAdFailedToDisplay = (sender, args) =>
                {
                    Debug.LogError("BlueStack Interstitial: " + " errorCode: " + args.ErrorCode + " message: " +
                                   args.Message);
                    OnDisplayFail?.Invoke();
                    FireOnAdFailedToDisplay(AdType.Interstitial);

                    _interstitialAd.OnInterstitialDidShown -= onAdDisplayed;
                    _interstitialAd.OnInterstitialDidFail -= onAdFailedToDisplay;
                    _interstitialAd.OnInterstitialClicked -= onAdClicked;
                    _interstitialAd.OnInterstitialDisappear -= onAdClosed;

                    _interstitialAdDisplaying = false;

                    PreloadAd(AdType.Interstitial);
                };

                onAdClosed = (sender, args) =>
                {
                    Debug.Log("BlueStack Interstitial Ad Close!");
                    OnClose?.Invoke();
                    FireOnAdClosed(AdType.Interstitial);
                    _interstitialAd.OnInterstitialDidShown -= onAdDisplayed;
                    _interstitialAd.OnInterstitialDidFail -= onAdFailedToDisplay;
                    _interstitialAd.OnInterstitialClicked -= onAdClicked;
                    _interstitialAd.OnInterstitialDisappear -= onAdClosed;

                    _interstitialAdDisplaying = false;

                    PreloadAd(AdType.Interstitial);
                };

                // Add Event Handlers
                _interstitialAd.OnInterstitialDidShown += onAdDisplayed;
                _interstitialAd.OnInterstitialDidFail += onAdFailedToDisplay;
                _interstitialAd.OnInterstitialClicked += onAdClicked;
                _interstitialAd.OnInterstitialDisappear += onAdClosed;

                _interstitialAd?.Show();
            };
            
            if (!_interstitialAdLoaded)
            {
                LoadInterstitial(OnAdPreloaded);
            }
            else
            {
                OnAdPreloaded?.Invoke();
                //AddJob(OnAdPreloaded);
                _interstitialAdLoaded = false;
            }
        }
#endregion

#region Rewarded Ad
        protected override void LoadRewarded(Action OnLoadSuccess = null, Action OnLoadFail = null)
        {
            EventHandler<EventArgs> onAdLoaded = null;
            EventHandler<BlueStackError> onAdFailedToLoad = null;

            onAdLoaded = (sender, args) =>
            {
                Debug.Log("BlueStack Rewarded ad successfully preloaded");
                _rewardedVideo.OnRewardedVideoAdLoaded -= onAdLoaded;
                _rewardedVideo.OnRewardedVideoAdError -= onAdFailedToLoad;
                _rewardedAdLoaded = true;
                
                OnLoadSuccess?.Invoke();
                FireOnAdLoaded(AdType.RewardedVideo); 
            };

            onAdFailedToLoad = (sender, args) =>
            {
                Debug.Log("BlueStack Rewarded ad errorCode: " + args.ErrorCode + " message: " + args.Message);
                _rewardedVideo.OnRewardedVideoAdLoaded -= onAdLoaded;
                _rewardedVideo.OnRewardedVideoAdError -= onAdFailedToLoad;
                _rewardedAdLoaded = false;
                
                OnLoadFail?.Invoke();
                FireOnAdFailedToLoad(AdType.RewardedVideo);
            };

            // Add Event Handlers
            _rewardedVideo.OnRewardedVideoAdLoaded += onAdLoaded;
            _rewardedVideo.OnRewardedVideoAdError += onAdFailedToLoad;
            if (_shouldLoadWithPreference)
            {
                _rewardedVideo.Load(_preference);
            }
            else
            {
                _rewardedVideo.Load();
            }
        }

        public override void ShowRewarded(Action <string, double>OnSuccess = null, Action OnFail = null, Action OnClose = null,
            Action OnDisplayFail = null)
        {
            Action OnAdPreloaded = () =>
            {
                _rewardedAdDisplaying = true;

                EventHandler<EventArgs> onAdDisplayed = null;
                EventHandler<BlueStackError> onAdFailedToDisplay = null;
                EventHandler<EventArgs> onAdLeavingApplication = null;
                EventHandler<RewardedItem> onUserRewardEarned = null;
                EventHandler<EventArgs> onAdClosed = null;

                onAdLeavingApplication = (sender, args) =>
                {
                    Debug.Log("BlueStack rewarded ad Clicked");
                    FireOnAdLeavingApplication(AdType.RewardedVideo);
                };

                onAdDisplayed = (sender, args) =>
                {
                    Debug.Log("BlueStack rewarded ad Displayed");
                    _rewardedVideo.OnUserRewardEarned += onUserRewardEarned;
                    FireOnAdDisplayed(AdType.RewardedVideo);
                };

                onAdFailedToDisplay = (sender, args) =>
                {
                    Debug.Log("BlueStack rewarded ad errorCode: " + args.ErrorCode + " message: " + args.Message);
                    _rewardedVideo.OnRewardedVideoAdClosed -= onAdClosed;
                    _rewardedVideo.OnRewardedVideoAdAppeared -= onAdDisplayed;
                    _rewardedVideo.OnRewardedVideoAdError -= onAdFailedToDisplay;
                    _rewardedVideo.OnRewardedVideoAdClicked -= onAdLeavingApplication;
                    _rewardedAdDisplaying = false;

                    OnDisplayFail?.Invoke();
                    FireOnAdFailedToDisplay(AdType.RewardedVideo);
                    
                    PreloadAd(AdType.RewardedVideo);
                };

                string rewardType = "unknown";
                double rewardAmount = 0;
                onUserRewardEarned = (sender, rewardItem) =>
                {
                    Debug.Log("BlueStack rewarded ad reward '" + rewardItem?.Type + "' earned: " + rewardItem?.Amount);
                    _isRewardSuccess = true;
                    if (rewardItem?.Type != null) rewardType = rewardItem?.Type;
                    if (rewardItem?.Amount != null) rewardAmount = (double)rewardItem?.Amount;
                };

                onAdClosed = (sender, args) =>
                {
                    Debug.Log("BlueStack rewarded ad closed");

                    if (_isRewardSuccess)
                    {
                        OnSuccess?.Invoke(rewardType, rewardAmount);
                        FireOnAdRewardSuccess(AdType.RewardedVideo);
                    }
                    else
                    {
                        OnFail?.Invoke();
                        FireOnAdRewardFail(AdType.RewardedVideo);
                    }

                    _rewardedVideo.OnRewardedVideoAdClosed -= onAdClosed;
                    _rewardedVideo.OnRewardedVideoAdAppeared -= onAdDisplayed;
                    _rewardedVideo.OnRewardedVideoAdError -= onAdFailedToDisplay;
                    _rewardedVideo.OnRewardedVideoAdClicked -= onAdLeavingApplication;
                    _rewardedVideo.OnUserRewardEarned -= onUserRewardEarned;
                    _rewardedAdDisplaying = false;
                    
                    OnClose?.Invoke();
                    FireOnAdClosed(AdType.RewardedVideo);
                    
                    PreloadAd(AdType.RewardedVideo);
                };

                _rewardedVideo.OnRewardedVideoAdClosed += onAdClosed;
                _rewardedVideo.OnRewardedVideoAdAppeared += onAdDisplayed;
                _rewardedVideo.OnRewardedVideoAdError += onAdFailedToDisplay;
                _rewardedVideo.OnRewardedVideoAdClicked += onAdLeavingApplication;

                _isRewardSuccess = false;
                _rewardedVideo?.Show();
            };

            if (!_rewardedAdLoaded)
            {
                LoadRewarded(OnAdPreloaded);
            }
            else
            {
                OnAdPreloaded?.Invoke();
                _rewardedAdLoaded = false;
            }
        }
#endregion

#region Banner Ad
        EventHandler<EventArgs> onBannerAdLoaded = null;
        public override void ShowBanner(BannerSize bannerSize, BannerPosition bannerPosition, Action OnLoadFail = null,
            Action OnDisplayFail = null)
        {
            var adUnitId = bannerId;

#if UNITY_EDITOR

            if (_bannerAd != null)
            {
                DestroyBanner();
            }
#endif
            if (_bannerAd == null)
            {
                // Create a Smart banner at bottom of the screen
                AdPosition adPosition = bannerPosition.ConvertTo<AdPosition>();
                _bannerAd = new BannerAd(adUnitId, adPosition);

                onBannerAdLoaded = (sender, args) =>
                {
                    Debug.Log("BlueStack banner ad loaded");
                    _bannerAdLoaded = true;
                    FireOnAdLoaded(AdType.Banner);
                    _bannerAd?.Show();
                    _isBannerShowing = true;
                };

                // Add Event Handlers
                _bannerAd.OnBannerDidLoad += onBannerAdLoaded;

                _bannerAd.OnBannerDidFailed += (sender, args) =>
                {
                    Debug.LogError("BlueStack banner ad Failed: errorCode: " + args.ErrorCode + " message: " + args.Message);
                    _bannerAdLoaded = false;
                    FireOnAdFailedToLoad(AdType.Banner);
                    OnLoadFail?.Invoke();
                };

                _bannerAd.OnBannerDisplay += (sender, args) =>
                {
                    Debug.Log("BlueStack banner ad displayed");
                    _bannerAdDisplaying = true;
                    _displayedBannerSize = bannerSize;
                    _displayedBannerPosition = bannerPosition;
                    AdSize adSize = GetBannerAdSize(bannerSize);
                    FireOnBannerAdDisplayed(AdType.Banner, bannerPosition, adSize.Height, adSize.Width);
                };

                _bannerAd.OnBannerDidRefresh += (sender, args) => { Debug.Log("BlueStack banner ad refreshed"); };
                _bannerAd.OnBannerDidFailToRefresh += (sender, args) =>
                {
                    Debug.LogError("BlueStack banner ad failed to refresh: errorCode: " + args.ErrorCode + " message: " +
                                   args.Message);
                };

                _bannerAd.OnBannerHide += (sender, args) => { Debug.Log("Banner ad hidden"); };
                _bannerAd.OnAdClicked += (sender, args) =>
                {
                    Debug.Log("BlueStack banner ad clicked");
                    FireOnAdLeavingApplication(AdType.Banner);
                };
            }

            if (_shouldLoadWithPreference)
            {
                _bannerAd?.Load(GetBannerAdSize(bannerSize), _preference);
            }
            else
            {
                _bannerAd?.Load(GetBannerAdSize(bannerSize));
            }
        }

        public override void ChangeBannerPosition(BannerPosition bannerPosition)
        {
            _bannerAd?.SetPosition(GetAdPosition(bannerPosition));

            _displayedBannerPosition = bannerPosition;
            AdSize adSize = GetBannerAdSize(_displayedBannerSize);
            FireOnBannerAdPositionChanged(AdType.Banner, bannerPosition, adSize.Height, adSize.Width);
        }

        public override void HideBanner()
        {
            _bannerAd.Hide();
            _isBannerShowing = false;
        }

        public override void DestroyBanner()
        {
            _bannerAd?.Destroy();
            _isBannerShowing = false;
            _bannerAdDisplaying = false;
            _bannerAd = null;
        }

        private static AdSize GetBannerAdSize(BannerSize bannerAdSize)
        {
            switch (bannerAdSize)
            {
                case BannerSize.BANNER:
                    return AdSize.Banner;
                case BannerSize.LEADERBOARD:
                    return AdSize.Leaderboard;
                case BannerSize.FULL:
                    return AdSize.FullBanner;
                case BannerSize.LARGE:
                    return AdSize.LargeBanner;
                case BannerSize.SMART:
                    return AdSize.DynamicBanner;
                case BannerSize.MEDIUM_RECTANGLE:
                    return AdSize.MediumRectangle;
                default:
                    Debug.LogError("Unable to get BlueStack banner Ad size : " +
                                   bannerAdSize.ToString());
                    return AdSize.Banner;
            }
        }

        private static AdPosition GetAdPosition(BannerPosition bannerPosition)
        {
            switch (bannerPosition)
            {
                case BannerPosition.TOP:
                    return AdPosition.Top;
                case BannerPosition.BOTTOM:
                    return AdPosition.Bottom;
                default:
                    Debug.LogError("Unable to get BlueStack banner Ad position : " +
                                   bannerPosition.ToString());
                    return AdPosition.Bottom;
            }
        }
#endregion

        public void TogglePreference(bool value)
        {
            Debug.Log("Toggle Preference: " + value);
            _shouldLoadWithPreference = value;

            PlayerPrefs.SetInt("LoadWithPreference", value ? 1 : 0);
        }

        private Preference createBSPreference()
        {
            Preference bsPreference = new Preference();

            Location myLocation = new Location(Location.NONE_PROVIDER)
            {
                Latitude = 35.757866,
                Longitude = 10.810547
            };

            bsPreference.SetAge(25);
            bsPreference.SetLanguage("en");
            bsPreference.SetGender(Gender.Male);
            bsPreference.SetKeyword("brand=myBrand;category=sport");
            bsPreference.SetLocation(myLocation, 1);
            bsPreference.SetContentUrl("https://madvertise.com/en/");

            return bsPreference;
        }

        public void ToggleBannerDisplay()
        {
            if (_isBannerShowing)
            {
                _bannerAd?.Hide();
                _isBannerShowing = false;
            }
            else
            {
                _bannerAd?.Show();
                _isBannerShowing = true;
            }
        }
    }
}