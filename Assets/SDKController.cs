using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using GameAnalyticsSDK;
using Facebook.Unity;


public class SDKController : MonoBehaviour
{
    private int _currentLevelNumber;
    private int _previousLevelNumber = -1;
    private string _currentLevelNumberString;


    private void Start()
    {
        GameAnalyticsInitialize();
        FacebookInitialize();

        SubscribeGameAnalyticEvent();
    }


    #region GameAnalytics
    private void GameAnalyticsInitialize()
    {
        GameAnalytics.Initialize();
    }

    private void OnLevelStartEvent()
    {
        _currentLevelNumber = PlayerPrefs.GetInt("CurrentGlobalLevelNumber");
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, _currentLevelNumber.ToString());
    }

    private void OnLevelFailedEvent()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, _currentLevelNumber.ToString());
    }

    private void OnLevelCompleteEvent()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, _currentLevelNumber.ToString());
    }

    private void SubscribeGameAnalyticEvent()
    {
        GameEvents.Current.OnLevelStart += OnLevelStartEvent;
        GameEvents.Current.OnLevelVictory += OnLevelCompleteEvent;
        GameEvents.Current.OnLevelFailed += OnLevelFailedEvent;
    }
    #endregion
    #region FacebookSDK
    private void FacebookInitialize()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }
    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.LogWarning("Failed to Initialize the Facebook SDK");
        }
    }
    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }
    #endregion
}