using UnityEngine;

namespace Core
{
    public class LevelController : MonoBehaviour
    {
        public void LevelStart()
        {
            Debug.Log("Start level");
            GameEvents.Current.LevelStart();
        }

        public void LevelVictory()
        {
            LevelEnd();
            GameEvents.Current.LevelVictory();
        }

        public void LevelFailed()
        {
            LevelEnd();
            GameEvents.Current.LevelFailed();
        }

        private void LevelEnd()
        {
            GameEvents.Current.LevelEnd();
        }
    }
}