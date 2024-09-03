using UnityEngine;
using Utility;

namespace UI
{
    public class MainMenuButtonController : MonoBehaviour
    {
        public void OnPlay()
        {
            SceneManager.LoadScene(EScene.PlayerAdd);
        }

        public void OnQuit()
        {
            Application.Quit();
        }

        public void OnSettings()
        {
            SceneManager.LoadScene(EScene.Settings);
        }
    }
}