using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Core
{
    public class StickmanView : MonoBehaviour
    {
        [SerializeField] private bool _isPlayer = false;
        [Header("Materials")][SerializeField] private Material _playerMaterial;
        [SerializeField] private Animator _animator;
        [SerializeField] private Material _enemyMaterial;
        [Header("Renderer")][SerializeField] private SkinnedMeshRenderer _renderer;

        [Header("Minion properties")][SerializeField] private bool _isMinion;
        [SerializeField] private BallView _minionBall;
        [SerializeField] private bool _delieverBall = false;

        private Material[] _tempMaterials;
        private void Start()
        {
            if (_isMinion)
            {
                if (_isPlayer)
                {
                    _isMinion = false;
                    
                }
                _delieverBall = true;
            }
        }

        private IEnumerator MinionMoving(Vector3 position)
        {
            if (this == null)
            {
                yield break;
            }
            
            while (_delieverBall)
            {
                transform.DOMove(position, 0.3f);

            }
        }

        public void SetMinion()
        {
            _isMinion = true;
        }

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
    }
}