using UnityEngine;

namespace PixelRainbows.Panels
{
    ///<summary>All available Data on a given Panel</summary>
    [System.Serializable]
    internal class PanelData 
    {
        ///<summary>The transform base of the panel.</summary>
        internal Transform transform;

        ///<summary>The time in seconds it takes to transition to this panel.</summary>
        internal float transitionTime = 1f;

        ///<summary>How the panel is eased in.</summary>
        internal AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    }
}