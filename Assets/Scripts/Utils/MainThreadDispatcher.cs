using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

/// <summary>
/// A thread-safe class which holds a queue with actions to execute on the next Update() method. It can be used to make calls to the main thread for
/// things such as UI Manipulation in Unity. It was developed for use in combination with any Unity plugin, which uses separate threads for event handling
/// </summary>
public class MainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

    public void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }

    /// <summary>
    /// Locks the queue and adds the IEnumerator to the queue
    /// </summary>
    /// <param name="action">IEnumerator function that will be executed from the main thread.</param>
    private void Enqueue(IEnumerator action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(() => { StartCoroutine(action); });
        }
    }

    /// <summary>
    /// Locks the queue and adds the Action to the queue
    /// </summary>
    /// <param name="action">function that will be executed from the main thread.</param>
    public void Enqueue(Action action)
    {
        Enqueue(ActionWrapper(action));
    }

    /// <summary>
    /// Locks the queue and adds the Action to the queue, returning a Task which is completed when the action completes
    /// </summary>
    /// <param name="action">function that will be executed from the main thread.</param>
    /// <returns>A Task that can be awaited until the action completes</returns>
    public Task EnqueueAsync(Action action)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();

        void WrappedAction()
        {
            try
            {
                action();
                taskCompletionSource.TrySetResult(true);
            }
            catch (Exception exception)
            {
                taskCompletionSource.TrySetException(exception);
            }
        }

        Enqueue(ActionWrapper(WrappedAction));
        return taskCompletionSource.Task;
    }

    IEnumerator ActionWrapper(Action action)
    {
        action();
        yield return null;
    }

    private static MainThreadDispatcher _instance = null;

    public static bool Exists()
    {
        return _instance != null;
    }

    public static MainThreadDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject();
                go.name = "MainThreadDispatcher";
                _instance = go.AddComponent<MainThreadDispatcher>();
                DontDestroyOnLoad(go);
                Debug.Log(">> MainThreadDispatcher Instance!");
            }

            return _instance;
        }
    }

    void Awake()
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
    }

    void OnDestroy()
    {
        _instance = null;
    }
}