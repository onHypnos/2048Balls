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
        
        
        
        
        public Rigidbody RigidBody => _rigidbody;
        
        public void UpdateTextSpherePosition(Transform camera)
        {
            _textSphere.transform.LookAt(camera);
        }

        public void PushForward()
        {
            _rigidbody.velocity = _splineUser.result.forward*3f;
        }

        public double GetSplineProgressPercent()
        {
            return _splineUser.modifiedResult.percent;
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, _splineUser.result.position, 0.3f);
            var magnitude = _rigidbody.velocity.magnitude;
            _rigidbody.velocity = _splineUser.result.forward * magnitude*0.95f;
        }


        public void ChangeBallPower(int power)
        {
            if (power < 10)
            {
                if (_colorMaterials.Count < power)
                {
                    //_renderer.sharedMaterial = _colorMaterials[power];
                    for (int i = 0; i < _textComponents.Count; i++)
                    {
                        _textComponents[i].text = $"{Math.Pow(2, power)}";
                    }
                }
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
            _splineUser.spline = spline;

        }
    }
}