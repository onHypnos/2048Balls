using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class ParticleController : MonoBehaviour
    {
        public static ParticleController Current;
        [SerializeField] private GameObject[] _particleExamples;
        [SerializeField] private List<ParticleSystem[]> _particles;
        
        private void Awake()
        {
            Current = this;
        }

        public void InitializeParticleGO()
        {
            
        }

        public void CallParticle(Vector3 position)
        {
            
        }
    }
}