using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PixelRainbows.Panels;

namespace PixelRainbows
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        protected PanelManager panelSource;
        [SerializeField]
        protected Button forwardButton, backwardButton;

        private int panelIndex = 0;
        private PanelData lastPanel;

        private void Start() 
        {
            backwardButton.interactable = false;
            backwardButton.onClick.AddListener(Back);
            forwardButton.interactable = true;    
            forwardButton.onClick.AddListener(Continue);
            lastPanel = panelSource.GetPanel(0);
        }

        //Should be easy to fix this in case we add sub-panels or minigames.
        void Back()
        {
            panelIndex--;
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
                StartCoroutine(DoTransition());
            }
        }

        IEnumerator DoTransition()
        {
            DisableButtons();
            PanelData targetPanel = panelSource.GetPanel(panelIndex);
            Vector2 startPos = lastPanel.transform.position;
            Vector2 targetPos = targetPanel.transform.position;
            //time based transition.
            for(float t = 0; t < targetPanel.transitionTime; t += Time.deltaTime)
            {
                //normalized time, evaluated from the curve.
                float nt = targetPanel.transitionCurve.Evaluate(t / targetPanel.transitionTime);
                Vector3 pos = Vector2.Lerp(startPos, targetPos, nt);
                pos.z = transform.position.z;
                transform.position = pos;
                yield return null;
            }
            lastPanel = targetPanel;
            EnableButtons();
            //Optional WakeUp message useful for starting minigames once we transition to their panel.
            targetPanel.transform.BroadcastMessage("WakeUp", SendMessageOptions.DontRequireReceiver);
        }

        void DisableButtons()
        {
            backwardButton.interactable = false;
            forwardButton.interactable = false;
        }

        void EnableButtons()
        {
            backwardButton.interactable = panelIndex > 0;
            forwardButton.interactable = panelIndex < panelSource.PanelCount-1;
        }
    }  
}