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

        [SerializeField] private SplineComputer _levelSpline; //Расширить до levelSplines
        [SerializeField] private Transform _starterNode; //Расширить до StarterNodes
        [SerializeField] private List<BallView> _ballSnakeList; //Расширить до List<List<BallView>>
        [SerializeField] private Queue<BallView> _ballPool = new Queue<BallView>();
        [SerializeField] private LineState _lineState; //LinesState
        [SerializeField] private PlayerView _currentPlayer;
        [Header("Example")] [SerializeField] private GameObject _ballExample;

        [SerializeField] private GameObject _playerPrefab;

        [Header("Properties")] [SerializeField]
        private List<int> _starterBalls;

        [SerializeField] [Range(0.1f, 3f)] private float _ballsClampValue;


        private int _iterator;
        private double _tempDistance;
        private BallView _lastBallOnSpline;
        private float _regroupWindow = 0;
        [SerializeField] [Range(0.1f, 1f)] private float _baseRegroupWindowDuration;
        [SerializeField] [Range(0.1f, 2f)] private float _deployTime;
        [SerializeField] [Range(0.01f, 2f)] private float _pushPowerModifier;
        [SerializeField] private int _scoreCurrent;
        [SerializeField] private int _scoreMax;

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
                tempGO.name = $"Ball {i}";
                queue.Enqueue(tempGO.GetComponent<T>());
            }
        }

        public void FindBackBall()
        {
            _tempBall = null;
            _tempDistance = 1;
            for (_iterator = 0; _iterator < _ballSnakeList.Count; _iterator++)
            {
                if (_ballSnakeList[_iterator].GetSplineProgressPercent() < _tempDistance)
                {
                    _tempDistance = _ballSnakeList[_iterator].GetSplineProgressPercent();
                    _tempBall = _ballSnakeList[_iterator];
                }
            }

            _lastBallOnSpline = _tempBall;
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
                    for (_iterator = 0; _iterator < _ballSnakeList.Count; _iterator++)
                    {
                        _ballSnakeList[_iterator].Execute(_ballsClampValue);
                    }

                    if (_regroupWindow > 0)
                    {
                        _regroupWindow -= Time.deltaTime;
                    }
                    else
                    {
                        _regroupWindow = 0;
                        SetLineState(LineState.Moving);
                    }

                    break;
                }
                default: break;
            }
        }

        private void SetLineState(LineState state)
        {
            switch (state)
            {
                case LineState.Await:
                {
                    break;
                }
                case LineState.Moving:
                {
                    break;
                }
                case LineState.Regroup:
                {
                    _regroupWindow = _baseRegroupWindowDuration;
                    break;
                }
                default:
                {
                    break;
                }
            }

            _lineState = state;
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
                    _lastBallOnSpline.PushForward(_ballSnakeList.Count * _pushPowerModifier);
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
            int iiii = 0;
            for (int i = 0; i < ballAmount; i++)
            {
                iiii++;
                _tempBall = GetBallFromPool(ballsPower[i]);
                //_TempBall going to line
                //_tempBall.SetSpline(_levelSpline);
                SetBallOnSpline(_tempBall, _levelSpline);
                _tempBall.transform.position = _starterNode.position;
                _tempBall.gameObject.SetActive(true);
                UpdateScore(CountScore(), _scoreMax);
                FindBackBall();
                yield return new WaitForSeconds(timeDelay);
            }
            Debug.Log($"Охуел?{iiii}, BIGBALLSAMOUNT{ballAmount}");
            
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
            _tempBall.gameObject.layer = 6;
            return _tempBall;
        }

        public void ReturnBallInPool(BallView view)
        {
            if (_ballSnakeList.Contains(view))
            {
                _ballSnakeList.Remove(view);
            }

            view.gameObject.SetActive(false);
            view.RigidBody.velocity = Vector3.zero;
            view.gameObject.layer = 6;
            view.SetSpline(null);
            _ballPool.Enqueue(view);
        }

        public void BallCollapsed(BallView view, int pow)
        {
            ReturnBallInPool(view);
            for (int i = 0; i < _ballSnakeList.Count; i++)
            {
                if (_ballSnakeList[i] != _lastBallOnSpline)
                {
                    _ballSnakeList[i].PushBack(_ballSnakeList.Count, pow);
                }
                else
                {
                    _ballSnakeList[i].PushBack(_ballSnakeList.Count, 1);
                }
                
                SetLineState(LineState.Regroup);
            }

            UpdateScore(CountScore(), _scoreMax);
        }


        private void SetBallOnSpline(BallView ball, SplineComputer spline)
        {
            ball.SetSpline(spline);
            if (!_ballSnakeList.Contains(ball))
            {
                _ballSnakeList.Add(ball);
                UpdateScore(CountScore(), _scoreMax);
            }
        }

        public void SetBallOnSpline(BallView ball)
        {
            SetBallOnSpline(ball, _levelSpline);
        }


        public void LevelStart()
        {
            UpdateScore(0, _scoreMax);
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

        
        private int _tempScore;
        private int CountScore()
        {
            _tempScore = 0;
            for (_iterator = 0; _iterator < _ballSnakeList.Count; _iterator++)
            {
                //_tempScore += (int)Math.Pow(2, _ballSnakeList[_iterator].BallPower) * _ballSnakeList[_iterator].BallPower;
                _tempScore += (int)Math.Pow(2, _ballSnakeList[_iterator].BallPower);
            }

            return _tempScore;
        }

        private void UpdateScore(int currentScore, int scoreMax)
        {
            _scoreCurrent = currentScore;
            GameEvents.Current.ScoreUpdate(_scoreCurrent, scoreMax);
        }
    }

    internal enum LineState
    {
        Await,
        Moving,
        Regroup
    }
}