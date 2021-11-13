using System.Collections.Generic;
using UnityEngine;


public class InputController : MonoBehaviour
{
    [SerializeField] private bool _isActive;
    [SerializeField] private bool _useMouse;
    private bool _countQueue = true;
    private Queue<Vector2> _queue = new Queue<Vector2>();
    private float _temporalMagnitude = 0;
    private Vector2 _mouseStartPosition;
    private bool _mouseCLickedPreviousFrame = false;
    private Vector2 _mouseOldPosition;
    private Vector2 _mousePosition = Vector2.zero;
    private int _counter = 0;
    private Touch _firstTouch;


    public float TemporalMagnitude => _temporalMagnitude;

    public void Update()
    {
        if (!_isActive)
        {
            return;
        }

        if (!_useMouse)
        {
            if (Input.touchCount > 0)
            {
                _firstTouch = Input.GetTouch(0);
                switch (_firstTouch.phase)
                {
                    case TouchPhase.Began:
                    {
                        InputEvents.Current.TouchBeganEvent(_firstTouch.position);
                        break;
                    }
                    case TouchPhase.Canceled:
                    {
                        InputEvents.Current.TouchCancelledEvent();
                        break;
                    }
                    case TouchPhase.Moved:
                    {
                        InputEvents.Current.TouchMovedEvent(_firstTouch.position);
                        break;
                    }
                    case TouchPhase.Ended:
                    {
                        InputEvents.Current.TouchEndedEvent(_firstTouch.position);
                        break;
                    }
                    case TouchPhase.Stationary:
                    {
                        InputEvents.Current.TouchStationaryEvent(_firstTouch.position);
                        break;
                    }
                }
            }
        }
        else
        {
            if (_mouseCLickedPreviousFrame)
            {
                if (Input.GetMouseButton(0))
                {
                    _mousePosition = Input.mousePosition;
                    if (_mousePosition == _mouseOldPosition)
                    {
                        InputEvents.Current.TouchStationaryEvent(_mousePosition);
                        //Debug.Log("Cтационарно");
                    }
                    else
                    {
                        InputEvents.Current.TouchMovedEvent(_mousePosition);
                        //Debug.Log("Мувд");
                    }
                }
                else
                {
                    _mousePosition = Input.mousePosition;
                    InputEvents.Current.TouchEndedEvent(_mousePosition);
                    //Debug.Log("Енд");
                    _mouseCLickedPreviousFrame = false;
                }
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    _mouseCLickedPreviousFrame = true;
                    InputEvents.Current.TouchBeganEvent(Input.mousePosition);
                    //Debug.Log("Старт");
                }
            }

            _mouseOldPosition = _mousePosition;
        }
    }
}