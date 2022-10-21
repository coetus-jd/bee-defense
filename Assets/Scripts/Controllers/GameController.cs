using Bee.Defenses;
using Bee.Enums;
using Bee.Interfaces;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Bee.Controllers
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private GameState State;

        [SerializeField]
        private int CurrentLevel;

        [Header("UI")]
        [SerializeField]
        private TMP_Text LevelText;

        [SerializeField]
        private GameObject LevelUp;

        [SerializeField]
        private GameObject LostPanel;

        [SerializeField]
        private GameObject PausePanel;

        [Header("Controllers")]
        private PunctuationController PunctuationController;
        private EnemiesController EnemiesController;
        private DefenseController DefenseController;
        private HiveController HiveController;

        void Start()
        {
            Time.timeScale = 1;
            LevelText.text = $"Level {CurrentLevel}";

            PunctuationController = GameObject.FindGameObjectWithTag(Tags.PunctuationController)
                .GetComponent<PunctuationController>();
            EnemiesController = GameObject.FindGameObjectWithTag(Tags.EnemiesController)
                .GetComponent<EnemiesController>();
            DefenseController = GameObject.FindGameObjectWithTag(Tags.DefenseController)
                .GetComponent<DefenseController>();
            HiveController = GameObject.FindGameObjectWithTag(Tags.Hive)
                .GetComponent<HiveController>();

            HandleLevel();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                TogglePauseGame();
            
            OnLost();
        }

        public void TogglePauseGame()
        {
            PausePanel.SetActive(!PausePanel.activeSelf);
            Time.timeScale = PausePanel.activeSelf ? 0 : 1;
        }

        public void PlaySameLevelAgain()
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.PreviousLevel, CurrentLevel);
            SceneLoaderController.ReloadCurrentScene();
        }

        public void NextLevel(bool ignoreLevelUp = false)
        {
            CurrentLevel++;
            print($"Level up: {CurrentLevel}");
            LevelText.text = $"Level {CurrentLevel}";
            EnemiesController.OnNextLevel(CurrentLevel);
            DefenseController.OnNextLevel(CurrentLevel);
            HiveController.OnNextLevel(CurrentLevel);

            if (!ignoreLevelUp)
                StartCoroutine(HandleLevelUp());    
        }

        public void OnLost()
        {
            if (!HiveController.EnemyAttacked)
                return;

            CleanGameObjects();
            LostPanel.SetActive(true);
            Time.timeScale = 0;
        }

        private IEnumerator HandleLevelUp()
        {
            LevelUp.SetActive(true);
            LevelUp.GetComponent<Animator>().SetBool("Start", true);

            yield return new WaitForSeconds(1f);

            LevelUp.GetComponent<Animator>().SetBool("Start", false);
            LevelUp.SetActive(false);
        }

        private void CleanGameObjects()
        {
            foreach (var gameObject in GameObject.FindGameObjectsWithTag(Tags.BeeQueen))
                Destroy(gameObject);
        }

        private void HandleLevel()
        {   
            var savedLevel = PlayerPrefs.GetInt(PlayerPrefsKeys.PreviousLevel);

            print($"Save level: {savedLevel}");

            if (savedLevel == 0)
            {
                CurrentLevel = 1;
                PlayerPrefs.DeleteKey(PlayerPrefsKeys.PreviousLevel);
                return;
            }

            CurrentLevel = 0;

            for (int index = 0; index < savedLevel; index++)
                NextLevel(ignoreLevelUp: true);

            PlayerPrefs.DeleteKey(PlayerPrefsKeys.PreviousLevel);
        }

        public static void QuitGame() => Application.Quit();
    }
}
