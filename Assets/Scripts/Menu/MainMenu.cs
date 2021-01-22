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
        [SerializeField]
        private Button[] chapterButtons;

        private void Start() 
        {
            //only make the continue button interactable if there is any progress. continuing from 0 doesnt make much sense.
            continueButton.interactable = GameProgress.Current > 0;
            //enabled the image that shows up when the player finished the game
            finishedImage.enabled = GameProgress.Furthest >= gameFinishedReq;
            titleImage.enabled = !finishedImage.enabled;
            //make the chapters interactable if furthest progress is more than the chapter.
            for(int i = 0; i < chapterButtons.Length; i++)
                chapterButtons[i].interactable = i * 100 <= GameProgress.Furthest;
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

        //loads the chapter at the set index. if in order, then index 0 should lead to chapter 1 and so on.
        public void StartGameFromChapter(int chapterIndex)
        {
            SceneManager.LoadScene(chapters[chapterIndex]);
        }
    }
}