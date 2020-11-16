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
            panelIndex++;
            StartCoroutine(DoTransition());
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