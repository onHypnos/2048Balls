using System;
using UnityEngine;

namespace Core
{
    public class DesignController : MonoBehaviour
    {
        [SerializeField] private Material _rainbowMaterial;
        private Vector2 delta = Vector2.one * 0.01f;

        private void FixedUpdate()
        {
            _rainbowMaterial.mainTextureOffset += delta;
            if (_rainbowMaterial.mainTextureOffset.y >= 1)
            {
                _rainbowMaterial.mainTextureOffset = Vector2.zero;
            }
        }
    }
}