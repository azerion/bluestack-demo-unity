
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IO.Didomi.SDK;
using IO.Didomi.SDK.Events;

public class ConsentManager : MonoBehaviour
{
    [SerializeField] private bool enableDidomiInEditor = false;
    private DidomiEventListener eventListener = null;
    private EventManager _eventManager;
    private void Awake()
    {
#if !UNITY_EDITOR 
        RegisterDidomiEventHandlers();
#endif
        _eventManager = EventManager.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Loading starts
        if (PlayerPrefs.GetInt("didomi-local-config-available", 0) != 1 && Application.internetReachability == NetworkReachability.NotReachable)
        {
            DialogManager.Instance.CallNetworkAccessDialog(true,
                "Please connect to the internet...",
                1.0f);
        }
        else
        {
            DialogManager.Instance.CallNetworkAccessDialog(true,
                "Loading...",
                1.0f);
        }

#if UNITY_EDITOR
        if (!enableDidomiInEditor)
        {
            InitializeApp();
            return;
        }
#endif        
        
        // Didomi initialization
        InitializeDidomi();
    }
    
    void OnApplicationFocus(bool focusStatus)
    {
        // Debug.Log("OnApplicationFocus: " + focusStatus);
    }
    
    void OnApplicationPause(bool pause)
    {
        // Debug.Log("OnApplicationPause: " + pause);
    }

    #region didomi_user_consent
    
    private void RegisterDidomiEventHandlers()
    {
        eventListener = new DidomiEventListener();
        eventListener.Ready += OnReady;
        eventListener.ConsentChanged += OnConsentChanged;
        eventListener.Error += OnErrorEvent;
        Didomi.GetInstance().AddEventListener(eventListener);
    }

    private void UnRegisterDidomiEventHandlers()
    {
        if(eventListener != null){
            eventListener.Ready -= OnReady;
            eventListener.ConsentChanged -= OnConsentChanged;
            eventListener.Error -= OnErrorEvent;
        }
    }

    private void OnReady(object sender, ReadyEvent e)
    {
        Debug.Log("Didomi OnReady");
        EnqueueAction(() =>
            {
                DialogManager.Instance.CallNetworkAccessDialog(true,
                    "Loading...",
                    1.0f);
                PlayerPrefs.SetInt("didomi-local-config-available", 1);
            }
        );
    }
    
    private void OnErrorEvent(object sender, ErrorEvent e)
    {
        Debug.Log("Didomi OnErrorEvent : " + e.getErrorMessage());
    }

    private void OnConsentChanged(object sender, ConsentChangedEvent e)
    {
        UserStatus userStatus = Didomi.GetInstance().GetUserStatus();
        Debug.Log("Didomi OnConsentChanged, Consent String: " + userStatus.GetConsentString());
        
        InitializeApp();
    }
    
    private void InitializeApp()
    {
        //Loading ends, Hide Loading panel
        EnqueueAction(() =>
            {
                DialogManager.Instance.CallNetworkAccessDialog(false);
                _eventManager.Publish(EventTopic.InitializeAdSDK, new Message());
            }
        );
    }
    
    private void InitializeDidomi()
    {
        if (Didomi.GetInstance().IsReady())
        {
            return;
        }
        
        Didomi.GetInstance().OnReady(
            () =>
            {
                if (!CheckUserConsent())
                {
                    InitializeApp();
                }
            }
        );

        // iPhone or Android phone / Android TV
        Didomi.GetInstance().Initialize(
            new DidomiInitializeParameters(
                // Didomi example keys
                apiKey: "9bf8a7e4-db9a-4ff2-a45c-ab7d2b6eadba",
                noticeId: "Ar7NPQ72",
                tvNoticeId: "DirGCFKy",
                androidTvEnabled: true
            )
        );

#if !UNITY_EDITOR
        Didomi.GetInstance().OnError( () => {
            Debug.Log("ConsentManager: OnError : Failed to Initialize Didomi!");
            InitializeApp();
        });
#endif
    }
    
    private bool CheckUserConsent()
    {
#if UNITY_EDITOR
        Didomi.GetInstance().SetupUI();
        return false;
#else
        Debug.Log("Didomi IsConsentRequired: " + Didomi.GetInstance().IsConsentRequired());
        Debug.Log("Didomi ShouldConsentBeCollected: " + Didomi.GetInstance().ShouldUserStatusBeCollected());
        if (Didomi.GetInstance().ShouldUserStatusBeCollected())
        {
            Didomi.GetInstance().SetupUI();
            return true;
        }
        else
        {
            return false;
        }
#endif
    }

    public void GetUserConsent()
    {
        Didomi.GetInstance().SetupUI();
    }
    
    #endregion    
    
    
    private void OnDestroy()
    {
#if !UNITY_EDITOR
        UnRegisterDidomiEventHandlers();
#endif
    }
    
    // A queue with actions to execute on the next Update() method.
    // It can be used to make calls to the main thread for a thread-safe approach
    private static readonly Queue<Action> ExecutionQueue = new Queue<Action>();

    // Update is called once per frame
    void Update()
    {
        lock (ExecutionQueue)
        {
            while (ExecutionQueue.Count > 0)
            {
                ExecutionQueue.Dequeue().Invoke();
            }
        }
    }
    
    void Enqueue(IEnumerator action)
    {
        lock (ExecutionQueue)
        {
            ExecutionQueue.Enqueue(() => {
                StartCoroutine(action);
            });
        }
    }

    private void EnqueueAction(Action action)
    {
        Enqueue(ActionWrapper(action));
    }
    IEnumerator ActionWrapper(Action action)
    {
        action();
        yield return null;
    }
}

