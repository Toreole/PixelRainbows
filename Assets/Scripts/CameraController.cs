using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using PixelRainbows.Panels;
using TMPro;

namespace PixelRainbows
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        protected PanelManager panelSource;
        [SerializeField]
        protected Button forwardButton, backwardButton;
        
        [Header("Chapter Transitions"), SerializeField]
        protected string nextScene;
        [SerializeField]
        protected int chapterNumber;
        [SerializeField]
        protected TextMeshProUGUI chapterTitle;
        [SerializeField]
        protected CanvasGroup uiFade;
        [SerializeField, Range(0.1f, 5)]
        protected float whiteFadeInOutTime = 2;
        [SerializeField, Range(0.1f, 5)]
        protected float titleFadeInOutTime = 1;
        [SerializeField, Range(1, 20)]
        protected float titleStayTime = 3;

        private int panelIndex = 0;
        private PanelData lastPanel;

        private void Start() 
        {

            backwardButton.interactable = false;
            backwardButton.onClick.AddListener(Back);
            forwardButton.interactable = true;    
            forwardButton.onClick.AddListener(Continue);
            //Load from the furthest game progress.
            if(GameProgress.LoadFromCurrent)
            {
                //use up the load flag.
                GameProgress.LoadFromCurrent = false;
                panelIndex = GameProgress.Current - (chapterNumber * 100);
                lastPanel = panelSource.GetPanel(panelIndex);
                lastPanel.Minigame?.WakeUp();
            }
            else
            {
                //Set current progress
                GameProgress.Current = chapterNumber*100;
                panelIndex = 0;
                lastPanel = panelSource.GetPanel(panelIndex);
            }
            //set the position to be at the panel, just in case.
            Vector3 position = lastPanel.transform.position.WithZ(transform.position.z);
            
            transform.position = position;

            chapterTitle.alpha = 0; //start with invisible title.
            StartCoroutine(DoIntroFade());
        }

        public void BackToMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

#if UNITY_EDITOR
        //Utility for editor: go to next minigame panel.
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.F7))
            {
                for(int i = panelIndex+1; i < panelSource.PanelCount-1; i++)
                {
                    if(panelSource.GetPanel(i+1).Minigame)
                    {
                        Debug.Log($"Found minigame on panel {i}");
                        lastPanel = panelSource.GetPanel(i);
                        panelIndex = i;
                        StartCoroutine(DoTransition());
                        break;
                    }
                }
            }
        }
#endif
        
        private IEnumerator DoIntroFade()
        {
            //Fade in the title.
            for(float t = 0; t < titleFadeInOutTime; t += Time.deltaTime)
            {
                chapterTitle.alpha = t / titleFadeInOutTime;
                yield return null;
            }
            chapterTitle.alpha = 1;
            //let the title stay for a while.
            yield return new WaitForSeconds(titleStayTime);
            //Fade out the title
            for(float t = titleFadeInOutTime; t > 0; t -= Time.deltaTime)
            {
                chapterTitle.alpha = t / titleFadeInOutTime;
                yield return null;
            }
            chapterTitle.alpha = 0;
            //fade into the scene.
            for(float t = whiteFadeInOutTime; t > 0; t -= Time.deltaTime)
            {
                uiFade.alpha = t / whiteFadeInOutTime;
                yield return null;
            }
            uiFade.alpha = 0;
            uiFade.blocksRaycasts = false;
            if(lastPanel.Minigame)
            {
                forwardButton.interactable = false;
                lastPanel.Minigame.WakeUp(); //mainly for animations and the sort.
                yield return new WaitUntil(() => lastPanel.Minigame.IsDone);
                forwardButton.interactable = true;
            }
        }

        private IEnumerator DoOutroFade()
        {
            uiFade.blocksRaycasts = true;
            for(float t = 0; t < whiteFadeInOutTime; t += Time.deltaTime)
            {
                uiFade.alpha = t / whiteFadeInOutTime;
                yield return null;
            }
            uiFade.alpha = 1;
            yield return new WaitForSeconds(1);
            if(!string.IsNullOrEmpty(nextScene))
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
        }

        //Should be easy to fix this in case we add sub-panels or minigames.
        void Back()
        {
            panelIndex--;
            if(lastPanel.Minigame)
                lastPanel.Minigame.CancelMinigame();
            StartCoroutine(DoTransition());
        }

        void Continue()
        {
            //check if the panel has subpanels
            if(lastPanel.HasSubPanels)
            {
                //as long as we havent seen the last subpanel yet, we should show the next one.
                if(lastPanel.SubPanelIndex < lastPanel.SubPanelCount-1)
                {
                    lastPanel.SubPanelIndex++;
                    var sub = lastPanel.GetSubPanel(lastPanel.SubPanelIndex);
                    var color = sub.color;
                    color.a = 1;
                    sub.color = color;
                    //maybe a short delay between showing subpanels so you cant spam through them by accident?
                }
                else 
                {
                    panelIndex++;
                    StartCoroutine(DoTransition());
                }
            }
            else 
            { //copy pasting feels dirty, but really it needs fewer lines of code than if i were to make a method for this so screw it.
                panelIndex++;
                if(panelIndex < panelSource.PanelCount)
                    StartCoroutine(DoTransition());
                else if(!string.IsNullOrEmpty(nextScene))
                    StartCoroutine(DoOutroFade());
            }
        }

        IEnumerator DoTransition()
        {
            DisableButtons();
            PanelData targetPanel = panelSource.GetPanel(panelIndex);
            Vector2 startPos = lastPanel.transform.position;
            Vector2 targetPos = targetPanel.transform.position;
            var transitionStyle = (targetPanel.overrideTransition == TransitionMode.UseDefault) ? panelSource.DefaultTransition : targetPanel.overrideTransition;
            //transition handle:
            if(transitionStyle == TransitionMode.SmoothMove)
                yield return DoSmoothTransition();
            else if(transitionStyle == TransitionMode.LinearMove)
                yield return DoLinearTransition();
            else if(transitionStyle == TransitionMode.JumpCut)
                transform.position = targetPos.WithZ(transform.position.z);
            else //if(transitionStyle == TransitionMode.WhiteFade)
                yield return DoWhiteFadeTransition();

            lastPanel = targetPanel;
            //EnableButtons();
            backwardButton.interactable = true;
            //Update the progress.
            GameProgress.Current = chapterNumber*100 + panelIndex;
            if(lastPanel.Minigame)
            {
                lastPanel.Minigame.WakeUp();
                yield return new WaitUntil(() => lastPanel.Minigame.IsDone);
                forwardButton.interactable = panelIndex < panelSource.PanelCount-1;
                if(!forwardButton.interactable)
                    StartCoroutine(DoOutroFade());
            }
            else 
                forwardButton.interactable = true;
#region TransitionRoutines
            //local methods for handling the transition. 
            IEnumerator DoSmoothTransition() //smooth curved-based transition
            {
                for(float t = 0; t < targetPanel.transitionTime; t += Time.deltaTime)
                {
                    //normalized time, evaluated from the curve.
                    float nt = targetPanel.transitionCurve.Evaluate(t / targetPanel.transitionTime);
                    Vector3 pos = Vector2.Lerp(startPos, targetPos, nt);
                    pos.z = transform.position.z;
                    transform.position = pos;
                    yield return null;
                }
            }
            IEnumerator DoLinearTransition() //linear movement between panels.
            {
                for(float t = 0; t < targetPanel.transitionTime; t += Time.deltaTime)
                {
                    float nt = t / targetPanel.transitionTime;
                    Vector3 pos = Vector2.Lerp(startPos, targetPos, nt);
                    pos.z = transform.position.z;
                    transform.position = pos;
                    yield return null;
                }
            }
            IEnumerator DoWhiteFadeTransition()
            {
                uiFade.blocksRaycasts = true;
                chapterTitle.alpha = 0;
                float t;
                //Fade the ui to pure white
                for(t = 0; t < targetPanel.transitionTime; t+= Time.deltaTime)
                {
                    float nt = t / targetPanel.transitionTime;
                    uiFade.alpha = nt;
                    yield return null;
                }
                //relocate the camera.
                transform.position = targetPanel.transform.position.WithZ(transform.position.z);

                //fade out the white screen blocker
                for(; t > 0f; t -= Time.deltaTime)
                {
                    float nt = t / targetPanel.transitionTime;
                    uiFade.alpha = nt;
                    yield return null;
                }
                //reset values to normalized.
                uiFade.blocksRaycasts = false;
                //chapterTitle.alpha = 1; was not needed lol
                uiFade.alpha = 0;
            }
#endregion
        }


        void DisableButtons()
        {
            backwardButton.interactable = false;
            forwardButton.interactable = false;
        }

        //void EnableButtons()
        //{
        //    backwardButton.interactable = panelIndex > 0;
        //    forwardButton.interactable = panelIndex < panelSource.PanelCount-1;
        //}

    }  
}