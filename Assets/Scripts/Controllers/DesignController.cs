using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class DesignController : MonoBehaviour
    {
        private static DesignController Current;
        [Header("Rainbow offset delta")][SerializeField] private Material _rainbowMaterial;

        [Header("Enviorment settings")] [SerializeField]
        private GameObject _envieromentGO;
        [SerializeField] private List<Material> _envieromentList;
        
        
        private Vector2 delta = Vector2.one * 0.01f;

        
        private void Awake()
        {
            Current = this;
        }

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