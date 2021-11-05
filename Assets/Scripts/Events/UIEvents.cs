using System;


public class UIEvents
{
    public static UIEvents Current = new UIEvents();


    public event Action OnButtonStartGame;
    public void ButtonStartGame()
    {
        OnButtonStartGame?.Invoke();
    }

    public event Action OnButtonPauseGame;
    public void ButtonPauseGame()
    {
        OnButtonPauseGame?.Invoke();
    }

    public event Action OnButtonResumeGame;
    public void ButtonResumeGame()
    {
        OnButtonResumeGame?.Invoke();
    }

    public event Action OnButtonRestartGame;
    public void ButtonRestartGame()
    {
        OnButtonRestartGame?.Invoke();
    }

    public event Action OnButtonNextLevel;
    public void ButtonNextLevel()
    {
        OnButtonNextLevel?.Invoke();
    }
}