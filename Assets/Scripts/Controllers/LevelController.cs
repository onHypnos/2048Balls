using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Subsystems;

namespace Core
{
    public class LevelController : MonoBehaviour
    {
        public static LevelController Current;
        private BallView _tempBall;

        [SerializeField] private SplineComputer _levelSpline;
        [SerializeField] private Transform _starterNode;
        [SerializeField] private List<BallView> _ballSnakeList;
        [SerializeField] private Queue<BallView> _ballPool = new Queue<BallView>();
        [SerializeField] private LineState _lineState;
        [SerializeField] private PlayerView _currentPlayer;
        [Header("Example")]
        [SerializeField] private GameObject _ballExample;

        [SerializeField] private GameObject _playerPrefab;
        [Header("Properties")] [SerializeField]
        private List<int> _starterBalls;
        [SerializeField][Range(0.1f, 3f)] private float _ballsClampValue;


        private int _iterator;
        private double tempDistance;
        private BallView _lastCalledBall;

        [SerializeField] [Range(0.1f, 2f)] private float _deployTime;

        private void Awake()
        {
            Current = this;
            
            FindObjectOfType<SceneController>().InitializeLevelController(this);
            if (_levelSpline == null)
            {
                Debug.LogWarning("SplineNotInstalled");
            }

            if (_starterNode == null)
            {
                Debug.LogWarning("StarterNode is not setted");
            }

            if (_ballPool == null)
            {
                _ballPool = new Queue<BallView>();
                InitializePool<BallView>(_ballPool, _ballExample, 50, _starterNode.position + Vector3.right);
            }

            if (_playerPrefab != null)
            {
                _currentPlayer = Instantiate(_playerPrefab, transform).GetComponent<PlayerView>();
                if (_currentPlayer == null)
                {
                    Debug.LogWarning("Player initialization failed in LevelController", this.gameObject);
                }
                else
                {
                    
                    
                }
            }
            else
            {
                Debug.LogWarning("Player prefab is not set, failed in LevelController", this.gameObject);
            }
        }

        private void InitializePool<T>(Queue<T> queue, GameObject example, int amount, Vector3 poolPosition)
        {
            if (queue == null)
            {
                queue = new Queue<T>();
            }

            GameObject tempGO;
            for (int i = 0; i < amount; i++)
            {
                tempGO = Instantiate(example, poolPosition, Quaternion.identity, this.transform);
                tempGO.SetActive(false);
                queue.Enqueue(tempGO.GetComponent<T>());
            }
        }

        public void FindBackBall()
        {
            _tempBall = null;
            tempDistance = 1;
            for (_iterator = 0; _iterator < _ballSnakeList.Count; _iterator++)
            {
                if (_ballSnakeList[_iterator].GetSplineProgressPercent() < tempDistance)
                {
                    tempDistance = _ballSnakeList[_iterator].GetSplineProgressPercent();
                    _tempBall = _ballSnakeList[_iterator];
                }
            }
            _lastCalledBall = _tempBall;
        }

        private void LateUpdate()
        {
            
            
            switch (_lineState)
            {
                case LineState.Await:
                {
                    for (_iterator = 0; _iterator < _ballSnakeList.Count; _iterator++)
                    {
                        _ballSnakeList[_iterator].Execute(_ballsClampValue);
                    }
                    break;
                }
                case LineState.Moving:
                {
                    for (_iterator = 0; _iterator < _ballSnakeList.Count; _iterator++)
                    {
                        _ballSnakeList[_iterator].Execute(_ballsClampValue);
                    }
                    break;
                }
                case LineState.Regroup:
                {
                    break;
                }
                default: break;
            }
        }

        private void FixedUpdate()
        {
            FindBackBall();
            switch (_lineState)
            {
                case LineState.Await:
                {
                    break;
                }
                case LineState.Moving:
                {
                    _lastCalledBall.PushForward(_ballSnakeList.Count);
                    break;
                }
                case LineState.Regroup:
                {
                    break;
                }
                default: break;
            }
        }
        
        public IEnumerator DeployStarterBalls(int ballAmount, List<int> ballsPower)
        {
            var timeDelay = _deployTime / ballAmount;
            for (_iterator = 0; _iterator < ballAmount; _iterator++)
            {
                _tempBall = GetBallFromPool(ballsPower[_iterator]);
                //_TempBall going to line
                //_tempBall.SetSpline(_levelSpline);
                SetBallOnSpline(_tempBall, _levelSpline);
                _tempBall.transform.position = _starterNode.position;
                _tempBall.gameObject.SetActive(true);
                
                yield return new WaitForSeconds(timeDelay);
            }
            FindBackBall();
            _lineState = LineState.Moving;
        }

        public BallView GetBallFromPool(int ballPower)
        {
            if (_ballPool == null || _ballPool.Count == 0)
            {
                Debug.LogWarning("Pool is null"); //TODO разобраться с порядком вызова
                InitializePool<BallView>(_ballPool, _ballExample, 50, _starterNode.position + Vector3.right);
            }
            _tempBall = _ballPool.Dequeue();
            _tempBall.ChangeBallPower(ballPower);
            return _tempBall;
        }

        public void ReturnBallInPool(BallView view)
        {
            if (_ballSnakeList.Contains(view))
            {
                _ballSnakeList.Remove(view);
            }
            view.gameObject.SetActive(false);
            _ballPool.Enqueue(view);
        }

        private void SetBallOnSpline(BallView ball, SplineComputer spline)
        {
            ball.SetSpline(spline);
            if (!_ballSnakeList.Contains(ball))
            {
                _ballSnakeList.Add(ball);
            }
        }

        public void SetBallOnSpline(BallView ball)
        {
            SetBallOnSpline(ball, _levelSpline);
        }


        public void LevelStart()
        {
            StartCoroutine(DeployStarterBalls(_starterBalls.Count, _starterBalls));
            _currentPlayer.SetLevelController(this);
            Debug.Log("Start level");
            GameEvents.Current.LevelStart();
            _currentPlayer.SetState(PlayerState.CanAttack);
        }

        public void LevelVictory()
        {
            LevelEnd();
            GameEvents.Current.LevelVictory();
        }

        public void LevelFailed()
        {
            LevelEnd();
            GameEvents.Current.LevelFailed();
        }

        private void LevelEnd()
        {
            GameEvents.Current.LevelEnd();
        }
    }

    internal enum LineState
    {
        Await,
        Moving,
        Regroup
    }
}