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
        [SerializeField]
        private GameObject blankPanelPrefab;

        [SerializeField]
        private List<PanelData> panels;
        
        [SerializeField]
        private new Camera camera;

#if UNITY_EDITOR
        //local static instance for the panel manager, just in case its needed.
        //if its not yet assigned, it will find it in the scene.
        protected static PanelManager Instance { get { return instance ?? (instance = FindObjectOfType<PanelManager>()); }}
        protected static PanelManager instance;

        //Does a PanelManager exist in this scene?
        public static bool Exists => Instance;

        //The index of the currently selected panel
        int selectedPanel;

        //Draw gizmos for all the panels in the collection.
        private void OnDrawGizmos() 
        {
            //figure out the "real" size of the camera with the current screen bounds.
            float screenRatio = (float)Screen.width / (float)Screen.height;
            float height = camera.orthographicSize;
            float width = screenRatio * height;

            //draw the camera bounds around each panel so it's easy to see whether it stays within the bounds.
            foreach(var p in panels)
                Gizmos.DrawWireCube(p.transform.position, new Vector3(width, height));
        }
#endif

    }
}