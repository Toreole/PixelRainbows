using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace PixelRainbows
{
    public class MainMenu : MonoBehaviour
    {
        public string sceneName; 

        //the chapters in order.
        [SerializeField]
        private string[] chapters;
        [SerializeField]
        private Button continueButton;
        [SerializeField]
        private Image titleImage, finishedImage;
        [SerializeField]
        private int gameFinishedReq = 619;

        private void Start() 
        {
            continueButton.interactable = GameProgress.Current > 0;
            finishedImage.enabled = GameProgress.Furthest >= gameFinishedReq;
            titleImage.enabled = !finishedImage.enabled;
        }

        public void PlayGame()
        {
            SceneManager.LoadScene(sceneName);
        }

        public void QuitGame()
        {
            Application.Quit();
            Debug.Log("Game Over");
        }

        public void ContinueGame()
        {
            //set the loading flag.
            GameProgress.LoadFromCurrent = true;
            //figure out the scene we have to load.
            int sceneIndex = GameProgress.Current / 100; //0-99 => 0 ; 100-199 => 1.
            SceneManager.LoadScene(chapters[sceneIndex]);
        }

        public void StartNewGame()
        {
            //reset progress.
            GameProgress.Current = 0;
            PlayGame();
        }
    }
}