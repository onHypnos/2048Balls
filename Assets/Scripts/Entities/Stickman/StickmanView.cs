using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Core
{
    public class StickmanView : MonoBehaviour
    {
        [SerializeField] protected bool _isPlayer = false;
        [Header("Materials")][SerializeField] protected Material _playerMaterial;
        [SerializeField] protected Animator _animator;
        [SerializeField] protected Material _enemyMaterial;
        [Header("Renderer")][SerializeField] protected SkinnedMeshRenderer _renderer;

        

        protected Material[] _tempMaterials;
        protected Transform _targetTransform;
        

        public void Initialize()
        {
            Initialize(_isPlayer);
        }

        public void Initialize(bool isPlayer)
        {
            _renderer.materials = _tempMaterials;
            if (isPlayer)
            {
                _tempMaterials[0] = _playerMaterial;
            }
            else
            {
                _tempMaterials[0] = _enemyMaterial;
            }
            _tempMaterials = _renderer.materials;
            _isPlayer = isPlayer;
            
        }

        protected void OnDestroy()
        {
            
        }
    }
}