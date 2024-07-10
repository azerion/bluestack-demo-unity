using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Text;

public class SystemDialog : Dialog
{
    public enum Result
    {
        Ok = 0,
        Yes,
        No,
    }

    public enum ButtonType
    {
        NotSet = -1,
        Ok = 0,
        YesNo,
    }

    public enum AppearanceType
    {
        NotSet = -1,
        Default = 0,
        Confirm,
        Information,
    }

    public Button buttonOk = null;
    public Button buttonYes = null;
    public Button buttonNo = null;
    public Text messageTitle = null;
    public Text messageBody = null;

    private AppearanceType appearanceType = AppearanceType.NotSet;
    private float waitTime = -1.0f;

    private string[] ButtonLabels = null;

    public void Awake()
    {
        ButtonLabels = new string[Enum.GetNames(typeof(Result)).Length];
    }

    void Update()
    {
        if (waitTime > 0.0f)
        {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0.0f)
            {
                waitTime = 0.0f;
                OnButtonClickOk();
            }
        }
    }

    public void OnButtonClickOk()
    {
        //Debug.Log( "SystemDialog OnButtonClickOk" );
        Finish((int)Result.Ok);
    }

    public void OnButtonClickYes()
    {
        //Debug.Log( "SystemDialog OnButtonClickYes" );
        Finish((int)Result.Yes);
    }

    public void OnButtonClickNo()
    {
        //Debug.Log( "SystemDialog OnButtonClickNo" );
        Finish((int)Result.No);
    }

    public void SetMessage(string message)
    {
        if (message != null)
        {
            messageBody.text = message;
        }
    }

    public void SetMessageTitle(string title)
    {
        if (title != null)
        {
            messageTitle.text = title;
        }
    }

    public void SetAppearanceType(AppearanceType type)
    {
        appearanceType = type;
    }

    private Button GetButton(Result idnt)
    {
        switch (idnt)
        {
            case Result.Ok:
                return buttonOk;
            case Result.Yes:
                return buttonYes;
            case Result.No:
                return buttonNo;
            default:
                return buttonOk;
        }
    }

    public void SetButtonType(ButtonType type)
    {
        buttonOk.gameObject.SetActive(false);
        buttonYes.gameObject.SetActive(false);
        buttonNo.gameObject.SetActive(false);
        switch (type)
        {
            case ButtonType.NotSet:
                break;

            case ButtonType.Ok:
                GetButton(Result.Ok).gameObject.SetActive(true);
                break;

            case ButtonType.YesNo:
                GetButton(Result.Yes).gameObject.SetActive(true);
                GetButton(Result.No).gameObject.SetActive(true);
                break;

            default:
                break;
        }
    }

    public void SetWaitTime(float _wait)
    {
        waitTime = _wait;
    }
}