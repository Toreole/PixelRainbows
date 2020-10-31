using UnityEngine;

namespace PixelRainbows.Panels
{
    ///<summary>All available Data on a given Panel</summary>
    [System.Serializable]
    public class PanelData 
    {
#pragma warning disable CS0649
        ///<summary>The transform base of the panel.</summary>
        public Transform transform;

        ///<summary>The main SpriteRenderer of this panel.</summary>
        public SpriteRenderer spriteRenderer;

        ///<summary>The time in seconds it takes to transition to this panel.</summary>
        public float transitionTime = 1f;

        ///<summary>How the panel is eased in.</summary>
        public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        ///<summary>Where the panel is located relative to the previous one.</summary>
        public PanelPlacement placement = PanelPlacement.NextTo;
#pragma warning restore CS0649 
    }

    public enum PanelPlacement
    {
        NextTo, 
        Above,
        Below,
        FreeForm
    }
}