using System;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

namespace Core
{
    public class BallView : MonoBehaviour
    {
        [SerializeField] private SplinePositioner _positioner;
        [SerializeField] private int _ballPower;
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private List<Material> _colorMaterials;
        
        
        public void ChangeBallPower(int power)
        {
            if (power < 10)
            {
                if (_colorMaterials.Count < power)
                {
                    _renderer.sharedMaterial = _colorMaterials[power];
                }
            }
            
            _ballPower = power;
        }

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void SetPositionerSpline(SplineComputer spline)
        {
            _positioner.spline = spline;

        }
    }
}