using UnityEngine;
using System.Collections;
using Exception = System.Exception;

public class Dialog : MonoBehaviour
{
    public delegate void Callback(Dialog dialog, int result);

    private Callback callback = null;

    protected void Finish(int result)
    {
        Debug.Log("Dialog Finish result: " + result);
        if (callback != null)
        {
            callback(this, result);
        }

        Destroy(this.gameObject);
    }

    public void SetCallback(Callback callback)
    {
        this.callback = callback;
    }
}