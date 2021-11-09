using System;
using System.Collections.Generic;
using Dreamteck.Splines;
using TMPro;
using UnityEngine;

namespace Core
{
    public class BallView : MonoBehaviour
    {
        [SerializeField] private SplineTracer _splineUser;
        [SerializeField] private int _ballPower;
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private List<Material> _colorMaterials;
        [SerializeField] private GameObject _textSphere;
        [SerializeField] private List<TextMeshProUGUI> _textComponents;
        private Rigidbody _rigidbody;
        private Vector3 temp;

        private float _clampingVelocityWindow;
        [Header("ClampingWindow")][Tooltip("Time when clamping not affect on rigidbody")][SerializeField][Range(0.5f,5f)]private float _clampingWindowDuration;

        public int BallPower => _ballPower;


        public Rigidbody RigidBody => _rigidbody;


        private void OnCollisionEnter(Collision other)
        {
            if (gameObject.layer == 6) //layer6 = ball
            {
                if (other.gameObject.layer == 7)
                {
                    LevelController.Current.SetBallOnSpline(this);
                }
            }
            else if (gameObject.layer == 7)
            {
                if (other.gameObject.layer == 7)
                {
                    if (other.gameObject.CompareTag(tag))
                    {
                        CollapseBalls(other.gameObject.GetComponent<BallView>());
                    }
                }
            }
        }

        private void OnCollisionStay(Collision other)
        {
            if (gameObject.layer == 6) //layer6 = ball
            {
                if (other.gameObject.layer == 7)
                {
                    LevelController.Current.SetBallOnSpline(this);


                    temp = _splineUser.result.forward * -1;
                    _rigidbody.AddForce(temp);
                    other.gameObject.GetComponent<Rigidbody>().AddForce(temp);
                }
            }
            else if (gameObject.layer == 7)
            {
                if (other.gameObject.layer == 7)
                {
                    if (other.gameObject.CompareTag(tag))
                    {
                        CollapseBalls(other.gameObject.GetComponent<BallView>());
                        OpenClampingWindow();
                        temp = _splineUser.result.forward * -1;
                        _rigidbody.AddForce(temp * (_ballPower * 3 + 10));
                    }
                }
            }
        }

        public void CollapseBalls(BallView view)
        {
            if (this.gameObject.activeSelf)
            {
                
                LevelController.Current.BallCollapsed(view);
                ChangeBallPower(_ballPower + 1);
            }
        }

        public void UpdateTextSpherePosition(Transform camera)
        {
            _textSphere.transform.LookAt(camera);
        }

        public void PushForward(float ballCount)
        {
            _rigidbody.velocity = _splineUser.result.forward * ballCount;
        }

        public double GetSplineProgressPercent()
        {
            return _splineUser.result.percent;
        }

        public void Execute(float clampValue)
        {
            transform.position = Vector3.Lerp(transform.position, _splineUser.result.position, 0.6f);
            //var magnitude = _rigidbody.velocity.magnitude;
            //_rigidbody.velocity = _splineUser.result.forward * magnitude*0.95f;
            if (_clampingVelocityWindow == 0)
            {
                _rigidbody.velocity =
                    Vector3.ClampMagnitude(Vector3.Project(_rigidbody.velocity, _splineUser.result.forward),
                        clampValue);
            }
            else if(_clampingVelocityWindow > 0)
            {
                _clampingVelocityWindow -= Time.deltaTime;
            }else if (_clampingVelocityWindow < 0)
            {
                _clampingVelocityWindow = 0;
            }
        }

        private void OpenClampingWindow()
        {
            _clampingVelocityWindow = _clampingWindowDuration;
        }

        public void ChangeBallPower(int power)
        {
            if (power < 12)
            {
                _renderer.sharedMaterial = _colorMaterials[power];
                for (int i = 0; i < _textComponents.Count; i++)
                {
                    _textComponents[i].text = $"{Math.Pow(2, power)}";
                }

                gameObject.tag = power.ToString();
            }

            _ballPower = power;
        }

        private void Awake()
        {
            gameObject.SetActive(false);
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void SetSpline(SplineComputer spline)
        {
            gameObject.layer = 7;
            _splineUser.spline = spline;
        }
    }
}