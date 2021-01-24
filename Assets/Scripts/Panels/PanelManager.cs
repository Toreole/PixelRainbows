using System.Collections.Generic;
using UnityEngine;

namespace PixelRainbows.Panels
{
    ///<summary>
    ///The PanelManager is the instance for editor and gameplay that manages the collection of the panels in-scene and any additional info on them.
    ///During edit mode, this is the main way of editing the flow of panels.
    ///</summary>
    public class PanelManager : MonoBehaviour
    {
#region SerializedSettings
        [SerializeField]
        private GameObject blankPanelPrefab;

        [SerializeField]
        private List<PanelData> panels = new List<PanelData>();
        
        [SerializeField] //the frame sprite is *required* for sub-panels.
        private Sprite frameSprite;
        
        [SerializeField]
        private new Camera camera;

        [SerializeField]
        private Transform panelParent;

        [SerializeField]
        private TransitionMode defaultTransitionStyle;

        public TransitionMode DefaultTransition => defaultTransitionStyle;
#endregion

#region Runtime_Vars
        private Vector2 cameraBounds;

        //The index of the currently selected panel
        int selectedPanel;
#endregion

#if UNITY_EDITOR
        //local static instance for the panel manager, just in case its needed. //Addendum: it is NOT needed.
        //if its not yet assigned, it will find it in the scene.
        //public static PanelManager Instance { get { return instance ?? (instance = FindObjectOfType<PanelManager>()); }}
        //protected static PanelManager instance;
        //Does a PanelManager exist in this scene?
        //public static bool Exists => Instance;

        [SerializeField, HideInInspector]
        public int lastEditedPanel = 0;

        //Draw gizmos for all the panels in the collection.
        private void OnDrawGizmos() 
        {
            if(!camera)
                return;
            //figure out the "real" size of the camera with the current screen bounds.
            var bounds = GetCameraBounds();
            Vector3 cubeSize = bounds * 2f;
            //print($"camera view: {cubeSize.x}, {cubeSize.y}");
            //draw the camera bounds around each panel so it's easy to see whether it stays within the bounds.
            for(int i = 0; i < panels.Count; i++)
            {
                var p = panels[i];
                Gizmos.color = (i == lastEditedPanel)? Color.green : Color.blue; //give a different colour to the panel we are currently editing.
                Gizmos.DrawWireCube(p.transform.position, cubeSize);
            }
            //???Draw Arrow from this to the next panel?
        }
#endif

        //Initialize PanelManager for runtime use. Prepare the panels.
        private void Start() 
        {
            //the camera bounds are vital for everything else.
            cameraBounds = GetCameraBounds();

            AutoArrangePanels();
            HideSubPanels();
        }

        ///<summary>
        ///Moves Panels defined by how they are placed in a way they dont overlap with the current screen ratio.
        ///if scalePanels is true, the panels will also be scaled down depending on screen width.
        ///</summary>
        public void AutoArrangePanels(bool scalePanels = true)
        {
            //original positions before adjusting, necessary for FreeForm panel placement to work.
            //Vector3[] prePositions = new Vector3[panels.Count];
#if UNITY_EDITOR
            //In the editor, Start() is never called so it needs to run the camera bounds calculation before doing this.
            cameraBounds = GetCameraBounds();
#endif
            //Adjust the size and position of all panels.
            for(int i = 0; i < panels.Count; i++)
            {
                //the current panel:
                PanelData current = panels[i];
                //change the size.
                if(scalePanels)
                    SetupPanel(current);
                //save the original editor position.
                //prePositions[i] = current.transform.position;
                if(i > 0)
                {
                    PanelData previous = panels[i-1];
                    switch(current.placement)
                    {
                        //place the panel to the right side of the last one.
                        case PanelPlacement.NextTo:
                            current.transform.position = previous.transform.position + new Vector3(cameraBounds.x*2f, 0f);
                            break;
                        case PanelPlacement.Below:
                            current.transform.position = previous.transform.position - new Vector3(0f, cameraBounds.y*2f);
                            break;
                        case PanelPlacement.Above:
                            current.transform.position = previous.transform.position + new Vector3(0f, cameraBounds.y*2f);
                            break;
                        //FreeForm uses the in-editor relative direction from the previous to this panel. 
                        //This may not be used, so might be free to just remove this, it causes some issues anyway.
                        //case PanelPlacement.FreeForm:
                        //    Vector3 relative = prePositions[i] - prePositions[i-1];
                        //    relative.Normalize();
                        //    relative *= cameraBounds.x / relative.x;
                        //    current.transform.position = previous.transform.position + relative;
                        //    break;
                    }
                }
            }
        }

        ///<summary>Hides all SubPanels besides the 0th. (makes them transparent with 0 alpha)</summary>
        void HideSubPanels() //!!! This could be made a lot easier by just disabling the renderer instead!
        {
            foreach(PanelData dat in panels)
            {
                if(dat.HasSubPanels)
                    for(int i = 1; i < dat.SubPanelCount; i++)
                    {
                        var rend = dat.GetSubPanel(i);
                        var color = rend.color; color.a = 0;
                        rend.color = color;
                    }
            }
        }


#if UNITY_EDITOR
        //Just a dirty ol' variable to store the method info to make it not run like shit.
        System.Reflection.MethodInfo GetSizeOfMainGameView = null;
#endif
        ///<summary>Get the camera's bounds defined by width and height in worldspace. (orthographic camera)</summary>
        private Vector2 GetCameraBounds()
        {
#if UNITY_EDITOR
//Get the resolution of the "main game view". This only matters while in editor.
//!!!!This is very performance intensive!!!
//source: https://answers.unity.com/questions/179775/game-window-size-from-editor-window-in-editor-mode.html
            if(GetSizeOfMainGameView == null)
            {
                System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
                this.GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            }
            System.Object Res = GetSizeOfMainGameView.Invoke(null,null);
            var resolution = (Vector2)Res;
            float screenRatio = resolution.x / resolution.y;
#else
            float screenRatio = (float)Screen.width / (float)Screen.height;
#endif
            float height = camera.orthographicSize;
            float width = screenRatio * height;
            return new Vector2(width, height);
        }

        //Scales a Panel to adjust for different screen resolutions. (mobile support basically)
        //This can be done in a better way probably.
        //Since minigames are now a thing, they should be checked for in here.
        private void SetupPanel(PanelData panel)
        {
            var target = panel.transform;
            var spriteSize = panel.spriteRenderer.bounds.extents; //width and height of the sprite in the scene.
            //Debug.DrawLine(sprite.bounds.min, sprite.bounds.max, Color.red, 5);
            //auto-scale the sprite.
            float scale = cameraBounds.x/spriteSize.x;
            Vector3 temp = target.localScale * scale;
            if(temp != Vector3.positiveInfinity)
                target.localScale = temp;
            else
                Debug.LogError($"Scale tried to be infinite with ({temp}) * {scale}", target);
            if(target.localScale.y > 1) //dont scale up beyond the default.
                target.localScale = Vector3.one;
            //Try to get the minigame.
            panel.Minigame = panel.transform.GetComponentInChildren<Minigame.MinigameBaseClass>();
        }

        public PanelData GetPanel(int index)
            => panels[index];

        public int PanelCount => panels.Count;
    }
}