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

        public void CollapseBalls(BallView view)
        {
            if (this.gameObject.activeSelf)
            {
                LevelController.Current.ReturnBallInPool(view);
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
            _rigidbody.velocity = Vector3.ClampMagnitude(Vector3.Project(_rigidbody.velocity, _splineUser.result.forward), clampValue);
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