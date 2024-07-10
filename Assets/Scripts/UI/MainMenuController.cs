using System.Collections.Generic;
using UnityEngine.UI;

public class MainMenuController : PanelSlider
{
    #region Fields
    
    private Button[] _menuButtons;
    private EventManager _eventManager; // manages communication with ad sdks
    private List<Subscription> _subscriptions;

    #endregion

    #region Methods

    public void OnEnable()
    {
        _eventManager = EventManager.Instance;
        _subscriptions = new List<Subscription>();
    }

    protected override void OnSetup()
    {
        // Close panel on menu buttons click
        _menuButtons = Content.GetComponentsInChildren<Button>();
        foreach (Button menuButton in _menuButtons)
        {
            menuButton.onClick.AddListener(delegate { Close(); });
        }
    }

    protected override void OnOpen()
    {
        AdManager.Instance.DestroyBanner();
        AdsMenuController.DeactivateAll();
    }

    #endregion
}