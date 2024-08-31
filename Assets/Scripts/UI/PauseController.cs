using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseController : MonoBehaviour
    {
        [SerializeField] private static bool _gamePaused;
        [SerializeField] private GameObject pauseMenu;

        // Start is called before the first frame update
        private void Start()
        {
            pauseMenu.SetActive(false);
            _gamePaused = false;
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public void OnMenu(InputValue value)
        {
            if (_gamePaused)
                ResumeGame();
            else
                PauseGame();
        }

        public void PauseGame()
        {
            _gamePaused = true;
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }

        public void ResumeGame()
        {
            _gamePaused = false;
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }

        public void MainMenu()
        {
            LoadMainMenu();
        }

        private void LoadMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}