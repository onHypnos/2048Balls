using System.Collections.Generic;
using Core;
using UnityEngine;

public class UIController : MonoBehaviour
{
    private List<BaseMenuView> _menues = new List<BaseMenuView>();
    private InGameUIView _inGameUI;


    public void Awake()
    {
        UIEvents.Current.OnButtonStartGame += StartGame;
        UIEvents.Current.OnButtonPauseGame += PauseGame;
        UIEvents.Current.OnButtonResumeGame += StartGame;
        UIEvents.Current.OnButtonNextLevel += NextLevel;
        UIEvents.Current.OnButtonRestartGame += RestartGame;
        GameEvents.Current.OnLevelVictory += WinGame;
        GameEvents.Current.OnLevelFailed += LoseGame;
    }

    private void Start()
    {
        SwitchUI(UIState.MainMenu);
    }


    //TODO += to game events
    private void UpdateInGameSlider(int currentPoints, int maxPoints)
    {
        _inGameUI.SetSlider(currentPoints, maxPoints);
    }

    private void StartGame()
    {
        Time.timeScale = 1.0f;
        SwitchUI(UIState.InGame);
    }

    private void PauseGame()
    {
        Time.timeScale = 0.01f;
        SwitchUI(UIState.Pause);
    }

    private void WinGame()
    {
        Time.timeScale = 0.01f;
        SwitchUI(UIState.WinMenu);
    }

    private void LoseGame()
    {
        Time.timeScale = 0.01f;
        SwitchUI(UIState.LoseMenu);
    }

    private void NextLevel()
    {
        //TODO InGameMenu
        SwitchUI(UIState.MainMenu);
    }

    private void RestartGame()
    {
        //TODO SameThatNextLevel
        SwitchUI(UIState.MainMenu);
    }

    public void AddView(BaseMenuView view)
    {
        _menues.Add(view);

        if (view.GetType() == typeof(InGameUIView))
        {
            _inGameUI = view as InGameUIView;
        }
    }

    #region Switch
    private void SwitchUI(UIState state)
    {
        if (_menues.Count == 0)
        {
            Debug.LogWarning("There is no menues to switch.");
        }
        switch (state)
        {
            case UIState.MainMenu:
                SwitchMenu(typeof(MainMenuView));
                break;
            case UIState.InGame:
                SwitchMenu(typeof(InGameUIView));
                break;
            case UIState.Pause:
                SwitchMenu(typeof(PauseMenuView));
                break;
            case UIState.WinMenu:
                SwitchMenu(typeof(WinMenuView));
                break;
            case UIState.LoseMenu:
                SwitchMenu(typeof(LoseMenuView));
                break;
        }
    }
    private void SwitchMenu(System.Type type)
    {
        bool isFound = false;

        for (int i = 0; i < _menues.Count; i++)
        {
            if (_menues[i].GetType() == type)
            {
                _menues[i].Show();
                isFound = true;
            }
            else
            {
                _menues[i].Hide();
            }

            if (i == _menues.Count - 1f && !isFound)
            {
                Debug.LogWarning($"Oops! Menu {type} not found");
            }
        }
    }
    #endregion
}