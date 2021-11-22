using System;
using UnityEngine;

namespace Core
{
    public class CastleView : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == 7)
            {
                LevelController.Current.LevelFailed();
            }
        }
    }
}