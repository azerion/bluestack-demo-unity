using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public GameObject prefabMarkNetworkAccess = null;
    public GameObject prefabSystemDialog = null;
    public GameObject prefabEventDialog = null;

    private GameObject objNetworkAccess = null;
    private GameObject objSystemDialog = null;
    private GameObject objEventDialog = null;
    
    private Text txtNetworkAccess = null;
    private Image networkAccessBg = null;
    private Dialog.Callback callbackDialog = null;
    
    private static readonly string MANAGER_PREFAB = "UI/DialogManager";
    private static DialogManager _instance = null;
    
    public static DialogManager Instance => GetInstance();

    private static DialogManager GetInstance()
    {
        if (_instance == null)
        {
            Object prefab = Resources.Load(MANAGER_PREFAB);
            Instantiate(prefab);
        }

        return _instance;
    }

    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeNetworkAccess();
    }

    #region SystemDialog

    /// <summary>
    /// Call System Dialog
    /// </summary>
    /// <param name="messageTitle">Dialog message title</param>
    /// <param name="message">Dialog message</param>
    /// <param name="appearanceType">Type of System Dialog</param>
    /// <param name="callback">Callback method</param>
    /// <param name="wait">Wait time before the message disappears </param>
    /// <returns></returns>
    public SystemDialog CallSystemDialog(string messageTitle, string message,
        SystemDialog.AppearanceType appearanceType, Dialog.Callback callback, float wait = 0.0f)
    {
        if (IsExistDialog())
        {
            Debug.Log("Dialog exists!");
            return null;
        }

        SystemDialog dialog = CreateSystemDialog(messageTitle, message, appearanceType, wait);
        StartDialog(dialog, callback);
        return dialog;
    }

    //	Create System Dialog
    private SystemDialog CreateSystemDialog(string messageTitle, string message,
        SystemDialog.AppearanceType appearanceType, float wait)
    {
        SystemDialog.ButtonType buttonType;
        if (appearanceType == SystemDialog.AppearanceType.Confirm)
        {
            buttonType = SystemDialog.ButtonType.YesNo;
        }
        else
        {
            buttonType = SystemDialog.ButtonType.Ok;
        }

        // Instantiate Dialog panel
        GameObject obj = Instantiate(prefabSystemDialog, this.transform, false) as GameObject;

        // Set message values
        SystemDialog dialog = obj.GetComponent<SystemDialog>();
        dialog.SetAppearanceType(appearanceType);
        dialog.SetMessageTitle(messageTitle);
        dialog.SetMessage(message);
        dialog.SetButtonType(buttonType);
        dialog.SetWaitTime(wait);

        return dialog;
    }

    private void StartDialog(Dialog dialog, Dialog.Callback callback)
    {
        objSystemDialog = dialog.gameObject;
        callbackDialog = callback;
        dialog.SetCallback(this.CallbackDialog);
    }

    private void CallbackDialog(Dialog dialog, int result)
    {
        objSystemDialog = null;

        if (callbackDialog != null)
        {
            Dialog.Callback callback = callbackDialog;
            callbackDialog = null;
            callback(dialog, result);
        }
    }

    #endregion

    #region EventDialog

    /// <summary>
    /// Call Event Dialog
    /// </summary>
    /// <param name="message">Event message</param>
    /// <param name="wait">Wait time before the message disappears </param>
    /// <returns>EventDialog</returns>
    public EventDialog CallEventDialog(string message, float wait = 2.0f)
    {
        if (objEventDialog != null)
        {
            // Debug.LogWarning("Event Dialog exists!");
            Destroy(objEventDialog);
        }

        EventDialog dialog = CreateEventDialog(message, wait);
        StartEventDialog(dialog);
        return dialog;
    }

    //	Create System Dialog
    private EventDialog CreateEventDialog(string message, float _wait)
    {
        // Instantiate Dialog panel
        GameObject obj = Instantiate(prefabEventDialog, this.transform, false) as GameObject;

        // Set message values
        EventDialog dialog = obj.GetComponent<EventDialog>();
        dialog.SetMessage(message);
        dialog.SetWaitTime(_wait);

        return dialog;
    }

    private void StartEventDialog(Dialog dialog)
    {
        objEventDialog = dialog.gameObject;
    }

    #endregion

    #region NetworkAccess

    // Initialize Network Access Dialog
    private void InitializeNetworkAccess()
    {
        objNetworkAccess = Instantiate(prefabMarkNetworkAccess, this.transform, false) as GameObject;
        if (objNetworkAccess != null)
        {
            txtNetworkAccess = objNetworkAccess.transform.Find("Loading/Text").GetComponent<Text>();
            networkAccessBg = objNetworkAccess.transform.Find("BG").GetComponent<Image>();
            objNetworkAccess.SetActive(false);
        }
    }

    /// <summary>
    /// Show/Hide Network Access Dialog
    /// </summary>
    /// <param name="active">Active status</param>
    /// <param name="loadingMsg">Loading message</param>
    /// <param name="bgAlpha">Background transparency (Alpha value 0-1) </param>
    public void CallNetworkAccessDialog(bool active, string loadingMsg = "Loading...", float bgAlpha = 0.5f )
    {
        if (objNetworkAccess != null)
        {
            txtNetworkAccess.text = loadingMsg;
            networkAccessBg.color = new Color(networkAccessBg.color.r, networkAccessBg.color.g, networkAccessBg.color.b, bgAlpha);
            objNetworkAccess.SetActive(active);
        }
    }

    #endregion

    // Check if any dialog is open
    private bool IsExistDialog()
    {
        return (objSystemDialog != null || objNetworkAccess.activeInHierarchy);
    }

    public void OnDestroy()
    {
        if (_instance == this)
        {
            Destroy(objNetworkAccess);
            if (objSystemDialog != null)
            {
                Destroy(objSystemDialog);
            }

            _instance = null;
        }
    }
}