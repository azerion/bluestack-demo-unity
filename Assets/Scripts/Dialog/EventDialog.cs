using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Text;

public class EventDialog : Dialog
{
    public Image panelImage = null;
    public Text messageBody = null;
    public float fadeSpeed = 5.0f;
    private float waitTime = -1.0f;

    public void Awake()
    {
    }

    void Update()
    {
        if (waitTime > 0.0f)
        {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0.0f)
            {
                waitTime = 0.0f;
                //Destroy(this.gameObject);
                StartCoroutine(FadeOutDialog());
            }
        }
    }

    public IEnumerator FadeOutDialog()
    {
        while (panelImage.color.a > 0)
        {
            float fadeAmount = panelImage.color.a - (fadeSpeed * Time.deltaTime);
            panelImage.color = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, fadeAmount);
            messageBody.color = new Color(messageBody.color.r, messageBody.color.g, messageBody.color.b, fadeAmount);
            yield return null;
        }

        Destroy(this.gameObject);
        yield return null;
    }

    public void SetMessage(string message)
    {
        if (message != null)
        {
            messageBody.text = message;
        }
    }

    public void SetWaitTime(float _wait)
    {
        waitTime = _wait;
    }
}