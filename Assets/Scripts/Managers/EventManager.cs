using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Azerion.BlueStack.API;
using Azerion.BlueStack.API.Banner;
using UnityEngine;

public interface IMessage
{
}

public class Subscription
{
    public Action Remove;
}

public struct Message : IMessage
{
}

public struct StatusMessage : IMessage
{
    public StatusMessage(string subject, string status)
    {
        Subject = subject;
        Status = status;
    }

    public string Subject;
    public string Status;
}

public struct LoadAdMessage : IMessage
{
    public LoadAdMessage(AdSDK subject, AdType type, Action onLoadSuccess = null, Action onLoadFail = null)
    {
        Subject = subject;
        Type = type;
        OnLoadSuccess = onLoadSuccess;
        OnLoadFail = onLoadFail;
    }

    public AdSDK Subject;
    public AdType Type;
    public Action OnLoadSuccess;
    public Action OnLoadFail;
}

public struct ShowNativeAdMessage : IMessage
{
    public ShowNativeAdMessage(AdSDK subject, NativeAdSize nativeAdSize, Action onDisplaySuccess = null,
        Action onDisplayFail = null, Transform parentTransform = null)
    {
        Subject = subject;
        NativeAdSize = nativeAdSize;
        OnDisplaySuccess = onDisplaySuccess;
        OnDisplayFail = onDisplayFail;
        ParentTransform = parentTransform;
    }

    public AdSDK Subject;
    public NativeAdSize NativeAdSize;
    public Action OnDisplaySuccess;
    public Action OnDisplayFail;
    public Transform ParentTransform;
}

public struct ShowInterstitialMessage : IMessage
{
    public ShowInterstitialMessage(AdSDK subject, Action onClose = null, Action onDisplayFail = null)
    {
        Subject = subject;
        OnClose = onClose;
        OnDisplayFail = onDisplayFail;
    }

    public AdSDK Subject;
    public Action OnClose;
    public Action OnDisplayFail;
}

public struct ShowRewardedMessage : IMessage
{
    public ShowRewardedMessage(AdSDK subject, Action<string, double> onSuccess = null, Action onFail = null, Action onClose = null,
        Action onDisplayFail = null)
    {
        Subject = subject;
        OnSuccess = onSuccess;
        OnFail = onFail;
        OnClose = onClose;
        OnDisplayFail = onDisplayFail;
    }

    public AdSDK Subject;
    public Action<string, double> OnSuccess;
    public Action OnFail;
    public Action OnClose;
    public Action OnDisplayFail;
}

public struct ShowBannerMessage : IMessage
{
    public ShowBannerMessage(AdSDK subject, BannerSize bannerSize = BannerSize.BANNER,
        BannerPosition bannerPosition = BannerPosition.BOTTOM, Action onLoadFail = null, Action onDisplayFail = null)
    {
        Subject = subject;
        BannerSize = bannerSize;
        BannerPosition = bannerPosition;
        OnLoadFail = onLoadFail;
        OnDisplayFail = onDisplayFail;
    }

    public AdSDK Subject;
    public BannerSize BannerSize;
    public BannerPosition BannerPosition;
    public Action OnLoadFail;
    public Action OnDisplayFail;
}

public struct AdEventMessage : IMessage
{
    public AdEventMessage(AdSDK subject, AdType type)
    {
        Subject = subject;
        Type = type;
    }

    public AdSDK Subject;
    public AdType Type;
}

public struct BannerAdEventMessage : IMessage
{
    public BannerAdEventMessage(AdSDK subject, BannerPosition bannerPosition = BannerPosition.BOTTOM, int height = 0,
        int width = 0)
    {
        Subject = subject;
        BannerPosition = bannerPosition;
        Height = height;
        Width = width;
    }

    public AdSDK Subject;
    public BannerPosition BannerPosition;
    public int Height;
    public int Width;
}

public struct NativeAdEventMessage : IMessage
{
    public NativeAdEventMessage(AdSDK subject, NativeAd nativeAd = null)
    {
        Subject = subject;
        NativeAd = nativeAd;
    }

    public AdSDK Subject;
    public NativeAd NativeAd;
}

public struct AdSDKInitializationMessage : IMessage
{
    public AdSDKInitializationMessage(AdSDK subject, string status)
    {
        Subject = subject;
        Status = status;
    }

    public AdSDK Subject;
    public string Status;
}

public struct ErrorMessage : IMessage
{
    public Exception Exception;
}

public class EventManager
{
    private ConcurrentDictionary<string, List<Action<IMessage>>> _topics;
    public static EventManager Instance { get; } = new EventManager();

    public EventManager()
    {
        if (Instance != null)
        {
            throw new ApplicationException("Can only have one EventManager in this project");
        }

        _topics = new ConcurrentDictionary<string, List<Action<IMessage>>>(2, 128);
    }

    public Subscription Subscribe<TMessage>(EventTopic eventTopic, Action<TMessage> listener) where TMessage : IMessage
    {
        string topic = eventTopic.ToString();
        List<Action<IMessage>> topicList = null;
        _topics.TryGetValue(topic, out topicList);
        if (topicList == null)
        {
            topicList = new List<Action<IMessage>>();
        }

        // copy the topic list in case it is being iterated on a different thread (we are only allowed to modify from unity thread)
        List<Action<IMessage>> copiedTopicList = new List<Action<IMessage>>(topicList);
        Action<IMessage> action = (IMessage m) => listener((TMessage)m);
        copiedTopicList.Add(action);

        Subscription subscription = new Subscription();
        subscription.Remove = () =>
        {
            List<Action<IMessage>> newTopicList = new List<Action<IMessage>>();
            List<Action<IMessage>> oldTopicList = _topics[topic];
            foreach (Action<IMessage> act in oldTopicList)
            {
                if (action != act)
                {
                    newTopicList.Add(act);
                }
            }

            _topics[topic] = newTopicList;
            subscription.Remove = () => { };
        };

        _topics[topic] = copiedTopicList;
        return subscription;
    }

    public void Publish<TMessage>(EventTopic eventTopic, TMessage info) where TMessage : IMessage
    {
        string topic = eventTopic.ToString();
        if (!_topics.ContainsKey(topic))
        {
            return;
        }

        foreach (Action<IMessage> action in _topics[topic])
        {
            try
            {
                //action(info);
                // to make the action be called from the main thread.
                MainThreadDispatcher.Instance.Enqueue(() => action(info));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}