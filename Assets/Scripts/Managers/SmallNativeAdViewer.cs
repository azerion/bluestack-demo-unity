using UnityEngine;
using Azerion.BlueStack.API;
using UnityEngine.UI;

public class SmallNativeAdViewer : MonoBehaviour
{
    private NativeAd _nativeAd;
    private bool _shouldDisplayNativeAd;
    private Preference _preference;

    // Below are the UI element that you need to assign from Unity Editor
    public GameObject badge;
    public GameObject appIcon;
    public GameObject title;
    public GameObject callToAction;

    // Initialization
    public void Initialize(NativeAd nativeAd)
    {
        _nativeAd = nativeAd;
        _shouldDisplayNativeAd = true;
    }

    void Start()
    {
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (this._shouldDisplayNativeAd)
        {
            gameObject.SetActive(true);
            _shouldDisplayNativeAd = false;

            Debug.Log("SmallNativeAdViewer: RegisterGameObjects");

            // badge
            string badgeText = this._nativeAd.GetBadge();
            if (badgeText != null)
            {
                Debug.Log("SmallNativeAdViewer: Register AdChoices");
                this.badge.GetComponent<Text>().text = badgeText;
                if (!this._nativeAd.RegisterBadgeTextGameObject(this.badge))
                {
                    Debug.Log("RegisterBadgeTextGameObject Unsuccessful");
                }
            }

            // Icon Texture
            Texture2D iconTexture = this._nativeAd.GetIconTexture();
            if (iconTexture != null)
            {
                Debug.Log("SmallNativeAdViewer: Register Icon Texture");
                appIcon.GetComponent<RawImage>().texture = iconTexture;
                if (!this._nativeAd.RegisterIconImageGameObject(appIcon))
                {
                    Debug.Log("RegisterIconImageGameObject Unsuccessful");
                }
            }

            // Headline/Title
            string titleText = this._nativeAd.GetTitle();
            if (titleText != null)
            {
                Debug.Log("SmallNativeAdViewer: Register Head line");
                title.GetComponent<Text>().text = titleText;
                if (!this._nativeAd.RegisterTitleTextGameObject(title))
                {
                    Debug.Log("RegisterHeadlineTextGameObject Unsuccessful");
                }
            }

            // CallToAction
            string callToActionText = this._nativeAd.GetCallToActionText();
            if (callToActionText != null)
            {
                Debug.Log("SmallNativeAdViewer: Register CallToAction");
                callToAction.GetComponent<Text>().text = callToActionText;
                if (!this._nativeAd.RegisterCallToActionGameObject(callToAction))
                {
                    Debug.Log("RegisterCallToActionGameObject Unsuccessful");
                }
            }

            // Debug.Log("Headline is " + titleText);
            // Debug.Log("GetBodyText is " + this.nativeAd.GetBodyText());
            // Debug.Log("GetCallToActionText is " + callToActionText);
        }
        // else
        // {
        //     gameObject.SetActive(false);
        // }
    }
}