using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = System.Random;

namespace Core
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private List<CannonView> _playerCannon;
        [SerializeField] private PlayerState _state;
        [SerializeField] private Camera _cameraMain;
        [SerializeField] private LevelController _controller;
        private int tempInt;
        
        private BallView _currentBall;
        private BallView _alternateBall;

        private Vector3 _currentBallBasePosition;
        private Vector3 _alternateBallBasePosition;
        
        private Vector2 _mousePosition = Vector2.zero;
        private Ray _cameraRayContainer;
        private RaycastHit _hitInfoContainter;
        private Vector3 _targetPosition = Vector3.zero;

        private bool _attackStarted = false;
        
        private void Start()
        {
            FindCannons();
            SubscribeEvents();
            _cameraMain = Camera.main;
            /*_currentBallBasePosition = _playerCannon[0].transform.position + Vector3.up * 3f + Vector3.back;
            _alternateBallBasePosition = _playerCannon[0].transform.position + Vector3.up + Vector3.left * 2f + Vector3.back;
            */
            _currentBallBasePosition = _cameraMain.transform.position + _cameraMain.transform.forward * 7f + _cameraMain.transform.up * -4.5f;
            _alternateBallBasePosition = _currentBallBasePosition +  _cameraMain.transform.forward * 3f + _cameraMain.transform.right * -1.3f + _cameraMain.transform.up * -2f;
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            InputEvents.Current.OnTouchBeganEvent += StartAttack;
            InputEvents.Current.OnTouchBeganEvent += UpdateMousePosition;
            InputEvents.Current.OnTouchMovedEvent += UpdateMousePosition;
            InputEvents.Current.OnTouchEndedEvent += EndAttack;
            UIEvents.Current.OnButtonStartGame += FindCannons;
            GameEvents.Current.OnLevelEnd += () => {Destroy(this);};
        }

        private void UnsubscribeEvents()
        {
            InputEvents.Current.OnTouchBeganEvent -= StartAttack;
            InputEvents.Current.OnTouchBeganEvent -= UpdateMousePosition;
            InputEvents.Current.OnTouchMovedEvent -= UpdateMousePosition;
            InputEvents.Current.OnTouchEndedEvent -= EndAttack;
            UIEvents.Current.OnButtonStartGame -= FindCannons;
        }

        private void Update()
        {
            if (_state == PlayerState.Attack)
            {
                _cameraRayContainer = _cameraMain.ScreenPointToRay(_mousePosition);
                if (Physics.Raycast(_cameraRayContainer, out _hitInfoContainter, float.PositiveInfinity, 1 << 20))
                {
                    _targetPosition = _hitInfoContainter.point;
                    for (tempInt = 0; tempInt < _playerCannon.Count; tempInt++)
                    {
                        _playerCannon[tempInt].UpdateRotation(_targetPosition);
                    }
                }
            }
        }

        private int GetRandomBallPower()
        {
            return UnityEngine.Random.Range(-1,5);
        }

        public void SetLevelController(LevelController controller)
        {
            _controller = controller;
            SetNewCurrentBall(_controller.GetBallFromPool(GetRandomBallPower()), false);
            SetNewAlternateBall(_controller.GetBallFromPool(GetRandomBallPower()));

        }

        public void SetNewCurrentBall(BallView view, bool moveInterpolate) 
        {
            _currentBall = view;
            if (moveInterpolate)
            {
                view.transform.DOMove(_currentBallBasePosition,0.5f);
            }
            else
            {
                _currentBall.transform.position = _currentBallBasePosition;
            }
            //_currentBall.transform.LookAt(_cameraMain.transform);
            _currentBall.transform.rotation = Quaternion.LookRotation(_cameraMain.transform.position - _currentBall.transform.position, _cameraMain.transform.up);
            _currentBall.gameObject.SetActive(true);
            
        }

        public void SetNewAlternateBall(BallView view)
        {
            _alternateBall = view;
            _alternateBall.transform.position = _alternateBallBasePosition;
            //_alternateBall.transform.LookAt(_cameraMain.transform);
            _alternateBall.transform.rotation = Quaternion.LookRotation(_cameraMain.transform.position - _currentBall.transform.position, _cameraMain.transform.up);
            _alternateBall.gameObject.SetActive(true);
        }


        public async void StartAttack(Vector2 pos)
        {
            if (_state == PlayerState.CanAttack)
            {
                SetState(PlayerState.Attack);
                await AwaitShootPosition();
            }
        }

        public void UpdateMousePosition(Vector2 pos)
        {
            _mousePosition = pos;
        }

        public Task<Vector3> AwaitShootPosition()
        {
            var checkPosition = new TaskCompletionSource<Vector3>();
            ActivateAttackingLine();
            OnEndAttack += (Vector2 pos) =>
            {
                for (tempInt = 0; tempInt < _playerCannon.Count; tempInt++)
                {
                    _playerCannon[tempInt].Attack(_controller.GetBallFromPool(_currentBall.BallPower));
                }
                _controller.ReturnBallInPool(_currentBall);
                SetNewCurrentBall(_alternateBall,true);
                SetNewAlternateBall(_controller.GetBallFromPool(GetRandomBallPower()));
                DeactivateAttackingLine();
                checkPosition.SetResult(pos);
                //ShootCurrentBall(pos);
                SetState(PlayerState.CanAttack);
            };
            return checkPosition.Task;
        }

        public event Action<Vector2> OnEndAttack;

        public void EndAttack(Vector2 pos)
        {
            OnEndAttack?.Invoke(pos);
            OnEndAttack = null;
        }

        private void ShootCurrentBall(Vector3 pos)
        {
            if (_currentBall != null)
            {
                
            }
        }

        private void ActivateAttackingLine()
        {
            for (int i = 0; i < _playerCannon.Count; i++)
            {
                _playerCannon[i].ActivateAimLine();
            }
        }

        private void DeactivateAttackingLine()
        {
            for (int i = 0; i < _playerCannon.Count; i++)
            {
                _playerCannon[i].DeactivateAimLine();
            }
        }

        private void FindCannons()
        {
            _playerCannon = new List<CannonView>();
            var Cannons = FindObjectsOfType<CannonView>();
            for (tempInt = 0; tempInt < Cannons.Length; tempInt++)
            {
                if (Cannons[tempInt].PlayerControl)
                {
                    _playerCannon.Add(Cannons[tempInt]);
                }
            }

            if (_playerCannon.Count < 1)
            {
                Debug.LogWarning("Cannons not found in scene");
            }
        }

        public void SetState(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.Menu:
                {
                    break;
                }
                case PlayerState.CanAttack:
                {
                    if (_state == PlayerState.Attack || _state == PlayerState.Menu)
                    {
                        _state = state;
                    }
                    else
                    {
                        return;
                    }
                    break;
                }
                case PlayerState.Attack:
                {
                    break;
                }
                case PlayerState.LevelEnd:
                {
                    break;
                }
                default:
                    break;
            }

            _state = state;
        }
    }

    public enum PlayerState
    {
        Menu,
        CanAttack,
        Attack,
        LevelEnd
    }
}