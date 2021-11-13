using System;

namespace Core
{
    public class GameEvents
    {
        public static GameEvents Current = new GameEvents();
        
        public event Action OnLevelVictory;
        public void LevelVictory()
        {
            OnLevelVictory?.Invoke();
        }

        public event Action OnLevelFailed;
        public void LevelFailed()
        {
            OnLevelFailed?.Invoke();
        }

        public event Action OnLevelStart;
        public void LevelStart()
        {
            OnLevelStart?.Invoke();
        }

        public event Action OnLevelEnd;
        public void LevelEnd()
        {
            OnLevelEnd?.Invoke();
        }

        public event Action<int,int> OnScoreUpdate;
        public void ScoreUpdate(int current, int max)
        {
            OnScoreUpdate?.Invoke(current,max);
        }
    }
}