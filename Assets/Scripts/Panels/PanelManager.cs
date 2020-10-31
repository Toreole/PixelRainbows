using System.Collections;
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
        
        [SerializeField]
        private new Camera camera;

        [SerializeField]
        private Transform panelParent;
#endregion

#region Runtime_Vars
        private Vector2 cameraBounds;

        //The index of the currently selected panel
        int selectedPanel;
#endregion

#if UNITY_EDITOR
        //local static instance for the panel manager, just in case its needed.
        //if its not yet assigned, it will find it in the scene.
        protected static PanelManager Instance { get { return instance ?? (instance = FindObjectOfType<PanelManager>()); }}
        protected static PanelManager instance;

        //Does a PanelManager exist in this scene?
        public static bool Exists => Instance;

        //Draw gizmos for all the panels in the collection.
        private void OnDrawGizmos() 
        {
            if(!camera)
                return;
            //figure out the "real" size of the camera with the current screen bounds.
            var bounds = GetCameraBounds();
            Vector3 cubeSize = bounds * 2f;

            Gizmos.color = Color.blue;
            //draw the camera bounds around each panel so it's easy to see whether it stays within the bounds.
            foreach(var p in panels)
                Gizmos.DrawWireCube(p.transform.position, cubeSize);
        }
#endif

        //Initialize PanelManager for runtime use. Prepare the panels.
        private void Start() 
        {
            //the camera bounds are vital for everything else.
            cameraBounds = GetCameraBounds();

            //original positions before adjusting, necessary for FreeForm panel placement to work.
            Vector3[] prePositions = new Vector3[panels.Count];

            //Adjust the size and position of all panels.
            for(int i = 0; i < panels.Count; i++)
            {
                //the current panel:
                PanelData current = panels[i];
                //change the size.
                SetupPanel(current);
                //save the original editor position.
                prePositions[i] = current.transform.position;
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
                            current.transform.position = previous.transform.position - new Vector3(0f, cameraBounds.y);
                            break;
                        case PanelPlacement.Above:
                            current.transform.position = previous.transform.position + new Vector3(0f, cameraBounds.y);
                            break;
                        //FreeForm uses the in-editor relative direction from the previous to this panel. 
                        //This may not be
                        case PanelPlacement.FreeForm:
                            Vector3 relative = prePositions[i] - prePositions[i-1];
                            relative.Normalize();
                            relative *= cameraBounds.x / relative.x;
                            current.transform.position = previous.transform.position + relative;
                            break;
                    }
                }
            }
        }

        ///<summary>Get the camera's bounds defined by width and height in worldspace. (orthographic camera)</summary>
        private Vector2 GetCameraBounds()
        {
            float screenRatio = (float)Screen.width / (float)Screen.height;
            float height = camera.orthographicSize;
            float width = screenRatio * height;
            return new Vector2(width, height);
        }

        //Scales a Panel to adjust for different screen resolutions. (mobile support basically)
        //This can be done in a better way probably.
        private void SetupPanel(PanelData panel)
        {
            var target = panel.transform;
            var spriteSize = panel.spriteRenderer.bounds.extents; //width and height of the sprite in the scene.
            //Debug.DrawLine(sprite.bounds.min, sprite.bounds.max, Color.red, 5);
            //auto-scale the sprite.
            float scale = cameraBounds.x/spriteSize.x;
            target.localScale *= scale;
            if(target.localScale.y > 1) //dont scale up beyond the default.
                target.localScale = Vector3.one;
        }
    }
}