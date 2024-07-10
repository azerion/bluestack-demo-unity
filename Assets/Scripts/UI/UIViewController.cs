using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIViewController : MonoBehaviour
{
    private static VerticalLayoutGroup _rootLayoutGroup;
    private static SafeAreaHandler _safeAreaHandler;
    private static GameObject settings;
    private static Button settingsButton;

    private EventManager _eventManager;
    private List<Subscription> _subscriptions;

    void Awake()
    {
        // Debug.Log("Screen.dpi: " + Screen.dpi);
        // Fetch the RectTransform from the GameObject
        _rootLayoutGroup = GetComponent<VerticalLayoutGroup>();
        _safeAreaHandler = GetComponent<SafeAreaHandler>();

        settings = GameObject.Find("SettingsButton");
        settingsButton = settings.GetComponent<Button>();
        settings.SetActive(false);
    }

    public static void ActivateSettingsButton(System.Action ShowSettings)
    {
        if (settings == null) return;
        settings.SetActive(true);
        settingsButton.onClick.RemoveAllListeners();
        settingsButton.onClick.AddListener(() => ShowSettings());
    }

    public static void DeactivateSettingsButton()
    {
        if (settings == null) return;
        settings.SetActive(false);
        settingsButton.onClick.RemoveAllListeners();
    }

    private void MoveUp()
    {
        SafeAreaHandler.margin = 0f;
    }

    private void MoveDown(int adHeight)
    {
#if UNITY_EDITOR
        SafeAreaHandler.margin = adHeight * 3.5f;
#else
        SafeAreaHandler.margin = adHeight * Screen.dpi/160f;
#endif
    }

    public void OnEnable()
    {
        _eventManager = EventManager.Instance;
        _subscriptions = new List<Subscription>();

        _subscriptions.Add(_eventManager.Subscribe(EventTopic.BannerAdDisplayed,
            (BannerAdEventMessage message) =>
            {
                if (message.BannerPosition == BannerPosition.BOTTOM)
                {
                    MoveUp();
                }
                else
                {
                    MoveDown(message.Height);
                }
            }
        ));

        _subscriptions.Add(_eventManager.Subscribe(EventTopic.BannerAdPositionChanged, (BannerAdEventMessage message) =>
            {
                if (message.BannerPosition == BannerPosition.BOTTOM)
                {
                    MoveUp();
                }
                else
                {
                    MoveDown(message.Height);
                }
            }
        ));

        _subscriptions.Add(_eventManager.Subscribe(EventTopic.DestroyBanner, (Message message) =>
            {
                Debug.Log("Ads menu : DestroyBanner!!! ");
                MoveUp();
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
}