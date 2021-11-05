using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class SceneController : MonoBehaviour
    {
        [SerializeField] private List<string> _scenes;

        [SerializeField] private LevelController _currentLevelController;
        
        private void Start()
        {
            UIEvents.Current.OnButtonNextLevel += LoadNextScene;


            PlayerPrefs.GetInt("PLayerLevel", 0);
        }
        
        public void LoadNextScene()
        {
            
        }

        public void LoadScene(string SceneName)
        {
            
        }
    }
}