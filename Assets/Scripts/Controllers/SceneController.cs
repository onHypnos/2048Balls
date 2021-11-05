using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class SceneController : MonoBehaviour
    {
        [SerializeField] private List<string> _scenes;
        [SerializeField] private bool LevelDebug = false;
        [SerializeField] private LevelController _currentLevelController;
        
        
        private void Start()
        {
            UIEvents.Current.OnButtonNextLevel += LoadNextScene;

            if (!LevelDebug)
            {
                LoadLevelScene(_scenes[PlayerPrefs.GetInt("PLayerLevel", 0)]);
            }
            else
            {
                FindLevelController();
            }
        }

        
        public void LoadNextScene()
        {
            if (_currentLevelController != null)
            {
                UIEvents.Current.OnButtonStartGame -= _currentLevelController.LevelStart;
            }

            var currentLevelNumber = PlayerPrefs.GetInt("PlayerLevel");
            SceneManager.UnloadSceneAsync(currentLevelNumber);
            PlayerPrefs.SetInt("PlayerLevel", currentLevelNumber + 1);

            LoadLevelScene(_scenes[PlayerPrefs.GetInt("PlayerLevel")]);
        }

        public void ReloadScene()
        {
            if (_currentLevelController != null)
            {
                UIEvents.Current.OnButtonStartGame -= _currentLevelController.LevelStart;
            }
            var currentLevelNumber = PlayerPrefs.GetInt("PlayerLevel");
            SceneManager.UnloadSceneAsync(currentLevelNumber);
            LoadLevelScene(_scenes[currentLevelNumber]);
        }

        public void LoadLevelScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName,LoadSceneMode.Additive);
            FindLevelController();
        }

        public bool FindLevelController()
        {
            _currentLevelController =  FindObjectOfType<LevelController>();
            if (_currentLevelController != null)
            {
                UIEvents.Current.OnButtonStartGame += _currentLevelController.LevelStart;
                return true;
            }
            else
            {
                Debug.Log("LevelController not found");
                return false;
            }
        }
    }
}