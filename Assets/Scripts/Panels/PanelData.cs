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

        ///<summary>The way the panel is transitioned in</summary> 
        [UnityEngine.Serialization.FormerlySerializedAs("transitionStyle")]
        public TransitionMode overrideTransition = TransitionMode.UseDefault;

        ///<summary>The time in seconds it takes to transition to this panel.</summary>
        public float transitionTime = 1f;

        ///<summary>How the panel is eased in.</summary>
        public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        ///<summary>Where the panel is located relative to the previous one.</summary>
        public PanelPlacement placement = PanelPlacement.NextTo;

        ///<summary>SubPanels are indivudual sprites that can be addressed later and enabled one by one.</summary>
        public SpriteRenderer[] subPanels; //A thing to consider might be Sound effects when showing different subpanels, but thats too much ig.
#pragma warning restore CS0649 

        //Does this panel have subpanels?
        public bool HasSubPanels => subPanels.Length != 0;
        public int SubPanelCount => subPanels.Length;
        public SpriteRenderer GetSubPanel(int index)
            => subPanels[index];

        ///<summary>The index of the currently shown subpanel. Increased at runtime. Used as condition to continue to next panel.</summary>
        public int SubPanelIndex { get; set; } = 0;

        public Minigame.MinigameBaseClass Minigame { get; set; }
    }

    public enum PanelPlacement
    {
        NextTo, 
        Above,
        Below,
        //FreeForm
    }
    public enum TransitionMode
    {
        SmoothMove,
        UseDefault = 1,
        LinearMove,
        JumpCut,
        WhiteFade

    }
}