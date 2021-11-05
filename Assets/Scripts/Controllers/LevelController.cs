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
        private BallView _tempBall;

        [SerializeField] private SplineComputer _levelSpline;
        [SerializeField] private Transform _starterNode;
        [SerializeField] private List<BallView> _ballSnakeList;
        [SerializeField] private Queue<BallView> _ballPool = new Queue<BallView>();
        [SerializeField] private LineState _lineState;
        [Header("Example")]
        [SerializeField] private GameObject _ballExample;
        [Header("Properties")] [SerializeField]
        private List<int> _starterBalls;



        private BallView _lastCalledBall;

        [SerializeField] [Range(0.1f, 2f)] private float _deployTime;

        private void Awake()
        {
            if (_levelSpline == null)
            {
                Debug.LogWarning("SplineNotInstalled");
            }

            if (_starterNode)
            {
                Debug.LogWarning("StarterNode not setted");
            }

            if (_ballPool == null)
            {
                _ballPool = new Queue<BallView>();
                InitializePool<BallView>(_ballPool, _ballExample, 50, _starterNode.position + Vector3.right);
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
            double tempDistance = 1.01;
            
            for (int i = 0; i < _ballSnakeList.Count; i++)
            {
                if (_ballSnakeList[i].GetSplineProgressPercent() < tempDistance)
                {
                    tempDistance = _ballSnakeList[i].GetSplineProgressPercent();
                    _tempBall = _ballSnakeList[i];
                }
            }
            _lastCalledBall = _tempBall;
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < _ballSnakeList.Count; i++)
            {
                //_ballSnakeList[i].
            }

            
            switch (_lineState)
            {
                case LineState.Await:
                {
                    break;
                }
                case LineState.Moving:
                {
                    _lastCalledBall.PushForward();
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
            for (int i = 0; i < ballAmount; i++)
            {
                _tempBall = GetBallFromPool(ballsPower[i]);
                //_TempBall going to line
                //_tempBall.SetSpline(_levelSpline);
                SetBallOnSpline(_tempBall, _levelSpline);
                _tempBall.transform.position = _starterNode.position;
                _tempBall.gameObject.SetActive(true);
                FindBackBall();
                yield return new WaitForSeconds(timeDelay);
            }
            _lineState = LineState.Moving;
        }

        private BallView GetBallFromPool(int ballPower)
        {
            
            if (_ballPool == null || _ballPool.Count == 0)
            {
                Debug.LogWarning("Pool is null"); //TODO разобраться с порядком вызова
                InitializePool<BallView>(_ballPool, _ballExample, 50, _starterNode.position+Vector3.right);
            }
            _tempBall = _ballPool.Dequeue();
            _tempBall.ChangeBallPower(ballPower);
            return _tempBall;
        }

        private BallView SetBallOnSpline(BallView ball, SplineComputer spline)
        {
            ball.SetSpline(spline);
            if (!_ballSnakeList.Contains(ball))
            {
                _ballSnakeList.Add(ball);
            }

            
            return ball;
        }


        public void LevelStart()
        {
            StartCoroutine(DeployStarterBalls(_starterBalls.Count, _starterBalls));
            
            Debug.Log("Start level");
            GameEvents.Current.LevelStart();
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