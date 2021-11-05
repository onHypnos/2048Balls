using System;
using UnityEngine;

public class InputEvents
{
    public static InputEvents Current = new InputEvents();

    #region TouchBeganEvents

    public Action<Vector2> OnTouchBeganEvent;

    public void TouchBeganEvent(Vector2 position)
    {
        OnTouchBeganEvent?.Invoke(position);
    }

    #endregion

    #region TouchMovedEvents

    public Action<Vector2> OnTouchMovedEvent;

    public void TouchMovedEvent(Vector2 mousePos)
    {
        OnTouchMovedEvent?.Invoke(mousePos);
    }

    #endregion

    #region TouchEndedEvents

    public Action<Vector2> OnTouchEndedEvent;

    public void TouchEndedEvent(Vector2 mousePos)
    {
        OnTouchEndedEvent?.Invoke(mousePos);
    }

    #endregion

    #region TouchStationaryEvents

    public Action<Vector2> OnTouchStationaryEvent;

    public void TouchStationaryEvent(Vector2 mousePos)
    {
        OnTouchStationaryEvent?.Invoke(mousePos);
    }

    #endregion

    #region TouchCancelledEvents

    public Action OnTouchCancelledEvent;

    public void TouchCancelledEvent()
    {
        OnTouchCancelledEvent?.Invoke();
    }

    #endregion
}