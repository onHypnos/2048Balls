using System;
using DG.Tweening;
using UnityEngine;

namespace Core
{
    public class WarriorView : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        public WarriorState State;

        
        
        public void RunTo(Vector3 position)
        {
            SetState(WarriorState.Run);
            transform.DOMove(position, 3f).onComplete += Dance;
            transform.LookAt(position, Vector3.up);
        }
        

        public void SetState(WarriorState state)
        {
            switch (state)
            {
                case WarriorState.Await:
                {
                    break;
                }
                case WarriorState.Run:
                {
                    _animator.SetBool("Run", true);
                    _animator.SetBool("Fight", false);
                    _animator.SetBool("Dance", false);
                    break;
                }
                case WarriorState.Fight:
                {
                    _animator.SetBool("Run", false);
                    _animator.SetBool("Fight", true);
                    _animator.SetBool("Dance", false);
                    break;
                };
                case WarriorState.Dance:
                {
                    _animator.SetBool("Run", false);
                    _animator.SetBool("Fight", false);
                    _animator.SetBool("Dance", true);
                    
                    break;
                }
                default:
                    break;
            }
            State = state;
        }

        public void Dance()
        {
            SetState(WarriorState.Dance);
        }
    }
    
    

    public enum WarriorState
    {
        Await,
        Run,
        Fight,
        Dance,
    }
}