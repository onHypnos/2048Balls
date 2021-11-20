using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Core
{
    public class MinionView : StickmanView
    {
        [Header("Minion properties")][SerializeField] protected bool _isMinion;
        [SerializeField] protected MinionBall _minionBall;
        [SerializeField] protected BallView _realBall;
        [SerializeField] protected bool _delieverBall = false;
        
        private void Start()
        {
            if (_isMinion)
            {
                if (_isPlayer)
                {
                    _isMinion = false;
                    
                }
                _delieverBall = true;
                _minionBall.gameObject.SetActive(true);
                _realBall.ChangeBallPower(1);
                _animator.SetBool("Pushing", true);
                StartCoroutine(MinionMoving());
            }
        }

        public void ActivateRealBall()
        {
            _realBall.gameObject.SetActive(true);
            _minionBall.gameObject.SetActive(false);
        }

        public void DefeatMinion()
        {
            _animator.SetTrigger("Dying");
            SetNotMinion();
            transform.DOMove(transform.position + Vector3.down, 6f);
            StopCoroutine(MinionMoving());
            Destroy(this.gameObject, 6.1f);
        }

        private IEnumerator MinionMoving()
        {
            while (_delieverBall)
            {
                if (LevelController.Current.LastBallBall != null)
                {
                    if (_isMinion)
                    {
                        _targetTransform = LevelController.Current.LastBallBall.transform;
                        transform.LookAt(_targetTransform);
                        transform.position = Vector3.MoveTowards(transform.position, _targetTransform.position, 0.005f);
                    }
                }
                yield return null;
            }
        }

        public void SetNotMinion()
        {
            _isMinion = false;
        }

        protected void OnDestroy()
        {
            StopCoroutine(MinionMoving());
        }
    }
}