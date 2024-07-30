using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ContentFitterRefresh : MonoBehaviour
{
    public static ContentFitterRefresh Instance;

    private void Start()
    {
        // Singleton
        if (Instance == null)
            Instance = this;

        RefreshContentFitters();
    }

    public void RefreshContentFitters()
    {
        var rectTransform = (RectTransform)transform;

        if (isActiveAndEnabled)
            StartCoroutine(RefreshContentFitter(rectTransform));
    }

    private IEnumerator RefreshContentFitter(RectTransform transform)
    {
        yield return new WaitForEndOfFrame();

        if (transform == null || !transform.gameObject.activeSelf)
        {
            yield return null;
        }

        foreach (RectTransform child in transform)
        {
            StartCoroutine(RefreshContentFitter(child));
        }

        var layoutGroup = transform.GetComponent<LayoutGroup>();
        var contentSizeFitter = transform.GetComponent<ContentSizeFitter>();
        if (layoutGroup != null)
        {
            layoutGroup.SetLayoutHorizontal();
            layoutGroup.SetLayoutVertical();
        }

        if (contentSizeFitter != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    void OnRectTransformDimensionsChange()
    {
        RefreshContentFitters();
    }
}