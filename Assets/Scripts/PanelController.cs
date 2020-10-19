﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PixelRainbows.Prototyping
{
    public class PanelController : MonoBehaviour
    {
        [SerializeField]
        protected Transform target;
        [SerializeField]
        protected float panelOffset, transitionTime;
        [SerializeField]
        protected Button continueButton;
        [SerializeField]
        protected AnimationCurve transitionCurve;

        int panelCount;
        int currentPanel = 1; //index starts at 1 KEKW

        void Start()
        {
            panelCount = target.childCount;
        }

        public void NextPanel()
        {
            if(currentPanel >= panelCount)
                return;
            continueButton.interactable = false;
            StartCoroutine(GotoNextPanel());
        }

        IEnumerator GotoNextPanel()
        {
            Vector2 startPos = target.position;
            Vector2 endPos = new Vector2(startPos.x + panelOffset, startPos.y);
            for(float t = 0; t < transitionTime; t += Time.deltaTime)
            {
                float ct = transitionCurve.Evaluate(t/transitionTime);
                target.position = Vector2.Lerp(startPos, endPos, ct);
                yield return null;
            }
            currentPanel ++;
            continueButton.interactable = true;
        }

    }
}