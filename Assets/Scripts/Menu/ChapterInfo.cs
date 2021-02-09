using UnityEngine.EventSystems;
using UnityEngine;

namespace PixelRainbows
{
    public class ChapterInfo : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField]
        protected GameObject info;
        [SerializeField]
        protected GameObject[] others;

        private UnityEngine.UI.Button button;

        private void Start() 
        {
            button = GetComponent<UnityEngine.UI.Button>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(!button.interactable)
                return;
            info.SetActive(true);
            for(int i = 0; i < others.Length; i++)
                others[i].SetActive(false);
        }

    }
}