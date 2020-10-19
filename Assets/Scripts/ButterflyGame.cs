using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelRainbows.Prototyping
{
    public class ButterflyGame : MonoBehaviour
    {
        [SerializeField]
        protected Butterfly[] butterflies;
        [SerializeField]
        protected float requiredProgress = 100;
        [SerializeField]
        protected float progressPerSecond = 8;
        [SerializeField]
        protected float butterflySpeed = 2;
        [SerializeField]
        protected Transform progressBar;
        [SerializeField]
        protected new Camera camera;


        float progress;
        bool isActive = false;
        int butterfliesInside = 0;

        //Custom message invoked via the PanelController.
        //This acts like a custom Start() method.
        public void WakeUp()
        {
            foreach(var bfly in butterflies)
                bfly.Init(this, butterflySpeed, Random.Range(0, 360), camera);
            isActive = true;
            butterfliesInside = butterflies.Length;
            StartCoroutine(GameLoop());
        }

        IEnumerator GameLoop()
        {
            while(progress < requiredProgress)
            {
                if(butterfliesInside >= butterflies.Length)
                {
                    progress += progressPerSecond * Time.deltaTime;  
                    progressBar.localScale = new Vector3(progress / requiredProgress, 1, 1);
                }  
                yield return null;
            }
            //game has ended successfully in here.
            foreach(var b in butterflies)
                b.Stop();
        }

        public void EnterButtfly()
        {
            butterfliesInside++;
        }

        public void LeaveButtfly()
        {
            butterfliesInside--;
        }
    }
}
