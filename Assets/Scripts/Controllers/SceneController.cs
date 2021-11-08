using System;
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

        private void Awake()
        {
            UIEvents.Current.OnButtonNextLevel += LoadNextScene;
        }

        private void Start()
        {
            if (!LevelDebug)
            {
                LoadLevelScene(_scenes[PlayerPrefs.GetInt("PLayerLevel", 0)]);
            }
            else
            {
            }
        }


        public void LoadNextScene()
        {
            Debug.Log(">???>>?");
            if (_currentLevelController != null)
            {
                UIEvents.Current.OnButtonStartGame -= _currentLevelController.LevelStart;
            }

            var currentLevelNumber = PlayerPrefs.GetInt("PlayerLevel");
            SceneManager.UnloadSceneAsync(_scenes[currentLevelNumber]);
            if (currentLevelNumber + 1 >= _scenes.Count)
            {
                currentLevelNumber = -1;
            }

            PlayerPrefs.SetInt("PlayerLevel", currentLevelNumber + 1);

            LoadLevelScene(_scenes[currentLevelNumber + 1]);
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
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        public bool InitializeLevelController(LevelController controller)
        {
            //_currentLevelController = LevelController.Current;
            _currentLevelController = controller;
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