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
        [SerializeField] private Queue<BallView> _ballPool;
        [SerializeField] private LineState _lineState;

        [Header("Properties")] [SerializeField]
        private List<int> _starterBalls;

        [SerializeField] [Range(0.1f, 2f)] private float _deployTime;

        private void Start()
        {
            if (_levelSpline == null)
            {
                Debug.LogWarning("SplineNotInstalled");
            }

            if (_starterNode)
            {
                Debug.LogWarning("StarterNode not setted");
            }
        }

        private void InitializePool<T>(Queue<T> _list, GameObject example, int amount, Vector3 poolPosition)
        {
            GameObject tempGO;
            for (int i = 0; i < amount; i++)
            {
                tempGO = Instantiate(example, poolPosition, Quaternion.identity);
                tempGO.SetActive(false);
                _list.Enqueue(tempGO.GetComponent<T>());
            }
        }

        public IEnumerator DeployStarterBalls(int ballAmount, List<int> ballsPower)
        {
            var timeDelay = _deployTime / ballAmount;
            for (int i = 0; i < ballAmount; i++)
            {
                _tempBall = GetBallFromPool(ballsPower[i]);
                yield return new WaitForSeconds(timeDelay);
            }
        }

        private void Update()
        {
            switch (_lineState)
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
                    break;
                }
                default: break;
            }
        }


        private BallView GetBallFromPool(int ballPower)
        {
            _tempBall = _ballPool.Dequeue();
            _tempBall.ChangeBallPower(ballPower);
            return _tempBall;
        }

        private BallView SetBallOnSpline(BallView ball, SplineComputer spline)
        {
            ball.SetPositionerSpline(spline);
            if (!_ballSnakeList.Contains(ball))
            {
                _ballSnakeList.Add(ball);
            }
            return ball;
        }


        public void LevelStart()
        {
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