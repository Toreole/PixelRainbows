using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

#if UNITY_EDITOR

#endif

namespace Minigame
{
    
    public class LoadingBar : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("GameObject/UI/Linear Progress Bar")]
        public static void AddLinearProgressBar()
        {
            GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Linear Progress Bar"));
            obj.transform.SetParent(Selection.activeGameObject.transform, false);
        }
#endif
    
        [SerializeField]
        private int _maximum;   
        public int Maximum => _maximum;

        [SerializeField]
        private int _minimum;

        public int Minimum => _minimum;

        [FormerlySerializedAs("_current")] [SerializeField]
        public int current;
    
        [SerializeField]
        private Image _mask; 
    
        [SerializeField]
        private Image _fill;
    
        [SerializeField]
        private Color _fillColor;

        void Update()
        {
            GetCurrentFill();
        }

        public void GetCurrentFill()
        {
            float currentOffset = current - _minimum;
            float maximumOffset = _maximum - _minimum;
            float fillAmount = currentOffset / maximumOffset;
            _mask.fillAmount = fillAmount;

            _fill.color = _fillColor;

        }
    }
}
