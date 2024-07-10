using UnityEngine;
using UnityEngine.UI;

public class DebugPanelController : PanelSlider
{
    
    [SerializeField] private Text logText;
    private string _log;
    
    #region Methods

    protected override void OnSetup()
    {
         RectTransform rectTransform = GetComponent<RectTransform>();
         var localPosition = rectTransform.localPosition;
         ClosedPosition = new Vector2(0, localPosition.y);
         OpenPosition = new Vector2(-rectTransform.rect.width, localPosition.y);
         
         // Placement
         Vector2 anchorMin = new Vector2(1, 0.5f);
         Vector2 anchorMax = new Vector2(1, 0.5f);
         Vector2 pivot = new Vector2(0, 0.5f);
         
         rectTransform.anchorMin = anchorMin;
         rectTransform.anchorMax = anchorMax;
         rectTransform.pivot = pivot;
    }

    protected override void OnOpen()
    {
        
    }
    
    void OnEnable()
    {
        Application.logMessageReceivedThreaded += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceivedThreaded -= HandleLog;
    }
    
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        _log += "\n" + "[" + type + "] : " + logString;
        if (type == LogType.Exception)
            _log += "\n" + "[StackTrace] : " + stackTrace;
    }

    public void ClearLog()
    {
        _log = "";
    }
    
    protected override void Update()
    {
        base.Update();
        logText.text = _log + "\n\n";
    }

    #endregion
}