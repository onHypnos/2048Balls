using System;
using UnityEngine;

namespace Core
{
    public class ParticleController : MonoBehaviour
    {
        public static ParticleController Current;
        
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