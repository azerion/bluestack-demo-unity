using UnityEngine;
using UnityEngine.UI;

public class AdsMenuController : MonoBehaviour
{
    private static GameObject _adsMenu;
    
    void Awake()
    {
        Initialize();
    }

    void Start()
    {
        DeactivateAll();
    }

    private void Initialize()
    {
        _adsMenu = gameObject; //GameObject.Find("AdsMenu");
    }

    public static void DeactivateAll()
    {
        foreach (Transform trans in _adsMenu.transform)
        {
            trans.gameObject.SetActive(false);
        }
    }
}