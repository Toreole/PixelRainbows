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
        private List<PanelData> panels;
        
        [SerializeField]
        private new Camera camera;

        [SerializeField]
        Transform testTarget;
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

            //draw the camera bounds around each panel so it's easy to see whether it stays within the bounds.
            foreach(var p in panels)
                Gizmos.DrawWireCube(p.transform.position, new Vector3(bounds.x, bounds.y));
        }
#endif

        //Initialize PanelManager for runtime use. Prepare the panels.
        private void Start() 
        {
            cameraBounds = GetCameraBounds();
        }

        ///<summary>Get the camera's bounds defined by width and height in worldspace. (orthographic camera)</summary>
        private Vector2 GetCameraBounds()
        {
            float screenRatio = (float)Screen.width / (float)Screen.height;
            float height = camera.orthographicSize;
            float width = screenRatio * height;
            return new Vector2(width, height);
        }

        void Update() => 
            SetupPanel(testTarget);

        //Just testing something here. (Making sure the object is on-screen in full)
        //!For some stupid reason this flickers between two results when run in Update.
        private void SetupPanel(Transform target)
        {
            cameraBounds = GetCameraBounds();
            var sprite = target.GetComponentInChildren<SpriteRenderer>(); //move main renderer component to data type?
            var spriteSize = sprite.bounds.extents; //width and height of the sprite in the scene.
            //Debug.DrawLine(sprite.bounds.min, sprite.bounds.max, Color.red, 5);
            //auto-scale the sprite.
            float scale = cameraBounds.x/spriteSize.x;
            target.localScale *= scale;
            if(target.localScale.y > 1) //dont scale up beyond the default.
                target.localScale = Vector3.one;
        }

    }
}