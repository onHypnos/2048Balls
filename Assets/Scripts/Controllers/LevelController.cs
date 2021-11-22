using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Subsystems;
using Random = System.Random;

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
        [SerializeField] private List<WarriorView> _warriorsPool;
        [SerializeField] private List<WarriorView> _activeWarriors;
        [SerializeField] private List<RiderView> _riders;
        [SerializeField] private LineState _lineState; //LinesState
        [SerializeField] private Transform _playerCastle;
        [SerializeField] private PlayerView _currentPlayer;
        [Header("Example")] [SerializeField] private GameObject _ballExample;
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _enemyMinion;
        [SerializeField] private GameObject _riderExample;
        [SerializeField] private GameObject _warriorExample;

        [Header("Properties")] [SerializeField]
        private List<int> _starterBalls;

        [SerializeField] private BallView _lastBallOnSpline;
        [SerializeField] [Range(0.1f, 3f)] private float _ballsClampValue;

        private int _iterator;
        private int _tempIndex;
        private double _tempDistance;

        private float _regroupWindow = 0;
        [SerializeField] [Range(0.1f, 1f)] private float _baseRegroupWindowDuration;
        [SerializeField] [Range(0.1f, 2f)] private float _deployTime;
        [SerializeField] [Range(0.1f, 3f)] private float _deployWindow = 1.5f;
        [SerializeField] [Range(0.01f, 2f)] private float _pushPowerModifier;
        [SerializeField] private int _scoreCurrent;
        [SerializeField] private int _scoreMax;

        [Header("SplineRoadMeshRenderer")] [SerializeField]
        private MeshRenderer _splineRoad;

        [Header("MMFeedbacks")] [SerializeField]
        private MMFeedbacks _mm;


        public BallView LastBallBall => _lastBallOnSpline;
        public LineState LineState => _lineState;

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

            if (_warriorExample != null)
            {
                _activeWarriors = new List<WarriorView>();
                InitializeWarriorPool();
            }
            else
            {
                Debug.LogWarning("Warrior prefab is not set, failed in LevelController", this.gameObject);
            }

            _splineRoad.material = DesignController.Current.GetRoadMaterial();
            GameEvents.Current.LevelLoaded();
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

        private int backBallIterator;

        public void FindBackBall()
        {
            if (_ballSnakeList.Count > 0)
            {
                _tempBall = null;
                _tempDistance = 1;
                _tempIndex = 0;
                for (backBallIterator = 0; backBallIterator < _ballSnakeList.Count; backBallIterator++)
                {
                    if (_ballSnakeList[backBallIterator].GetSplineProgressPercent() < _tempDistance)
                    {
                        _tempDistance = _ballSnakeList[backBallIterator].GetSplineProgressPercent();
                        _tempBall = _ballSnakeList[backBallIterator];
                        _tempIndex = backBallIterator;
                    }
                }

                _ballSnakeList[_tempIndex] = _ballSnakeList[0];
                _lastBallOnSpline = _tempBall;
                _ballSnakeList[0] = _tempBall;
            }
        }

        public void AddRider(RiderView riderView)
        {
            if (!_riders.Contains(riderView))
            {
                _riders.Add(riderView);
            }
        }

        public void KillRider(RiderView riderView)
        {
            _riders.Remove(riderView);
            Destroy(riderView.gameObject, 3f);
            if (_riders.Count.Equals(0))
            {
                StartCoroutine(DeployWarriors(_starterNode.position));
            }
        }

        [Button]
        public void TestDeployWarrirs()
        {
            StartCoroutine(DeployWarriors(_starterNode.position));
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
            switch (_lineState)
            {
                case LineState.Await:
                {
                    if (_ballSnakeList.Count > 0)
                    {
                        _ballSnakeList[0].Execute(_ballSnakeList.Count);
                        for (_iterator = 1; _iterator < _ballSnakeList.Count; _iterator++)
                        {
                            _ballSnakeList[_iterator].Execute(_ballsClampValue);
                        }

                        for (_iterator = 0; _iterator < _riders.Count; _iterator++)
                        {
                            _riders[_iterator].ExecuteMoving(_playerCastle.position);
                        }
                    }

                    break;
                }
                case LineState.Moving:
                {
                    FindBackBall();
                    _ballSnakeList[0].Execute(_ballSnakeList.Count);
                    for (_iterator = 1; _iterator < _ballSnakeList.Count; _iterator++)
                    {
                        _ballSnakeList[_iterator].Execute(_ballsClampValue);
                    }

                    _lastBallOnSpline.PushForward(_ballSnakeList.Count * _pushPowerModifier);
                    for (_iterator = 1; _iterator < _ballSnakeList.Count; _iterator++)
                    {
                        _ballSnakeList[_iterator].PushBack(_ballSnakeList.Count, Time.deltaTime * 0.5f);
                    }

                    for (_iterator = 0; _iterator < _riders.Count; _iterator++)
                    {
                        _riders[_iterator].ExecuteMoving(_playerCastle.position);
                    }

                    break;
                }
                case LineState.Regroup:
                {
                    _ballSnakeList[0].Execute(_ballSnakeList.Count);
                    for (_iterator = 1; _iterator < _ballSnakeList.Count; _iterator++)
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

                    for (_iterator = 0; _iterator < _riders.Count; _iterator++)
                    {
                        _riders[_iterator].ExecuteMoving(_playerCastle.position);
                    }

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
                for (int j = 0; j < UnityEngine.Random.Range(0, 6); j++)
                {
                    if (Instantiate(_riderExample, _tempBall.transform.position,
                        Quaternion.identity, transform).TryGetComponent<RiderView>(out RiderView newRider))
                    {
                        _tempBall.AddRiderEncounter(newRider);
                    }
                    else
                    {
                        Debug.LogWarning("ERROR");
                    }
                }

                _tempBall.gameObject.SetActive(true);
                UpdateScore(CountScore(), _scoreMax);
                FindBackBall();
                yield return new WaitForSeconds(timeDelay);
            }

            yield return new WaitForSeconds(_deployWindow);
            _lineState = LineState.Moving;
        }

        [Button]
        public void DebugDeployBalls()
        {
            StartCoroutine(DeployStarterBalls(4, _starterBalls));
        }

        public BallView GetBallFromPool(int ballPower)
        {
            if (_ballPool == null || _ballPool.Count == 0)
            {
                //Debug.LogWarning("Pool is null"); TODO разобраться с порядком вызова?в целом похуй
                InitializePool<BallView>(_ballPool, _ballExample, 50, _starterNode.position + Vector3.right);
            }

            _tempBall = _ballPool.Dequeue();
            _tempBall.gameObject.layer = 6;
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
            view.RigidBody.velocity = Vector3.zero;
            view.gameObject.layer = 6;
            view.SetSpline(null);
            _ballPool.Enqueue(view);
        }

        public void BallCollapsed(BallView collapsedView, BallView upgradedView, float pow)
        {
            //todo change values
            collapsedView.TransferRidersOnCollapse(upgradedView);
            if (collapsedView.GetRidersCount() > 0)
            {
                Debug.LogWarning("AXAAXAX", collapsedView.gameObject);
            }

            ReturnBallInPool(collapsedView);
            _ballSnakeList[0].StopBall();
            pow *= 0.2f;
            for (int i = 1; i < _ballSnakeList.Count; i++)
            {
                _ballSnakeList[i].PushBack(_ballSnakeList.Count, pow);
                SetLineState(LineState.Regroup);
            }

            FindBackBall();
            DesignController.Current.PulseBackgroundGradient();
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
            GameEvents.Current.LevelStart();

            _currentPlayer.SetState(PlayerState.CanAttack);
        }

        
        [Button]
        public void LevelVictory()
        {
            LevelEnd();
            _mm.PlayFeedbacks();
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
                _tempScore += (int) Math.Pow(2, _ballSnakeList[_iterator].BallPower);
            }

            return _tempScore;
        }

        private void UpdateScore(int currentScore, int scoreMax)
        {
            _scoreCurrent = currentScore;
            GameEvents.Current.ScoreUpdate(_scoreCurrent, scoreMax);
        }


        private void OnLastRiderKilledEncounter()
        {
            _lineState = LineState.WarriorsRun;
            for (int i = 0; i < _ballSnakeList.Count; i++)
            {
                _ballSnakeList[i].gameObject.SetActive(false);
            }
        }

        private WarriorView tempWarrior;

        private void InitializeWarriorPool()
        {
            InitializeWarriorPool(50);
        }

        private void InitializeWarriorPool(int count)
        {
            _warriorsPool = new List<WarriorView>();


            for (int i = 0; i < count; i++)
            {
                tempWarrior = Instantiate(_warriorExample, Vector3.down * 10f, Quaternion.identity, transform)
                    .GetComponent<WarriorView>();
                tempWarrior.gameObject.SetActive(false);
                _warriorsPool.Add(tempWarrior);
            }
        }

        private void SpawnWarrior(Vector3 position)
        {
            if (_warriorsPool.Count <= 0)
            {
                InitializeWarriorPool(10);
            }

            tempWarrior = _warriorsPool[_warriorsPool.Count - 1];
            _warriorsPool.Remove(tempWarrior);
            _activeWarriors.Add(tempWarrior);
            tempWarrior.transform.position = position;
            tempWarrior.gameObject.SetActive(true);
        }

        private void RemoveWarrior(WarriorView warriorView)
        {
            if (_activeWarriors.Contains(warriorView))
            {
                _activeWarriors.Remove(warriorView);
                _warriorsPool.Add(warriorView);
                warriorView.gameObject.SetActive(false);
                warriorView.transform.position = Vector3.down * 10f;
            }
        }

        private IEnumerator DeployWarriors(Vector3 position)
        {
            int i = 0;
            int j = 0;
            int tempCount;
            for (i = 0; i < _ballSnakeList.Count; i++)
            {
                tempCount = _ballSnakeList[i].BallPower;

                SpawnWarrior(
                    _ballSnakeList[i].transform.position /*+ Vector3.right * 0.5f *
                                                         Mathf.Sin((i * 360 / tempCount) * Mathf.Deg2Rad)
                                                         + Vector3.forward * 0.5f *
                                                         Mathf.Cos((i * 360 / tempCount) * Mathf.Deg2Rad)*/);


                _ballSnakeList[i].gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(1f);
            for (i = 0; i < _activeWarriors.Count; i++)
            {
                _activeWarriors[i].RunTo(position);
            }

            //Движение камеры
            yield return new WaitForSeconds(3f);
            LevelVictory();
        }
    }

    public enum LineState
    {
        Await,
        Moving,
        Regroup,
        WarriorsRun
    }
}