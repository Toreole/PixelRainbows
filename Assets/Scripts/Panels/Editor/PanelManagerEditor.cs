using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PixelRainbows.Panels;

namespace PixelRainbows.Editing
{
    [CustomEditor(typeof(PanelManager))]
    public class PanelManagerEditor : Editor
    {
        private PanelManager manager;

        SerializedProperty cameraProperty;
        SerializedProperty panelList;
        SerializedProperty panelPrefabProperty;
        SerializedProperty frameSpriteProperty;
        SerializedProperty panelParentProperty;
        bool foldout = false;
        int panelIndex;

        //The panel and it's properties.
        SerializedProperty selectedPanel;
        SerializedProperty panelTransform;
        SerializedProperty panelRenderer;
        SerializedProperty panelTransitionTime;
        SerializedProperty panelTransitionCurve;
        SerializedProperty panelPlacement;
        SerializedProperty panelSubPanels;

        bool autoRefreshArrangement = true;

        //initialize the editor.
        private void OnEnable() 
        {
            //Get the manager as the "real" object in case we need to access it directly.
            manager = target as PanelManager;
            cameraProperty = serializedObject.FindProperty("camera");
            panelList = serializedObject.FindProperty("panels");
            panelPrefabProperty = serializedObject.FindProperty("blankPanelPrefab");
            panelParentProperty = serializedObject.FindProperty("panelParent");
            frameSpriteProperty = serializedObject.FindProperty("frameSprite");

            panelIndex = Mathf.Clamp(manager.lastEditedPanel, 0, panelList.arraySize-1); //Get the panel index from the manager, this guarantees that we're never out of bounds.
            manager.lastEditedPanel = panelIndex; //set it back just incase
            //initialize the selectedPanel properties.
            if(panelList.arraySize > 0)
            {
                selectedPanel = panelList.GetArrayElementAtIndex(panelIndex);
                FindRelativeProperties(selectedPanel);
            }
        }

        //well this draws the editor, nothing special really.
        public override void OnInspectorGUI()
        {
            //the three basic properties.
            EditorGUILayout.PropertyField(panelPrefabProperty);
            EditorGUILayout.PropertyField(cameraProperty);
            EditorGUILayout.PropertyField(panelParentProperty);
            EditorGUILayout.PropertyField(frameSpriteProperty);
            if(!frameSpriteProperty.objectReferenceValue) //Give the message to the designer.
                EditorGUILayout.HelpBox("A frame sprite is required for the sub-panel workflow", MessageType.Info);

            //make sure the camera is assigned before we do anything else
            if(!cameraProperty.objectReferenceValue || !panelPrefabProperty.objectReferenceValue)
            {
                EditorGUILayout.HelpBox("Camera and Prefab need to be assigned in order to edit.", MessageType.Warning);
                serializedObject.ApplyModifiedProperties();
                return;
            }
            EditorGUILayout.Separator();
            //Check if the list of panels is empty. Then give options to initialize it.
            if(panelList.arraySize == 0)
            {
                DrawListInitialization();
                return;
            }
            else 
            {
                //Clear the list with the panel data.
                if(GUILayout.Button("Clear Panel Data"))
                {
                    ClearPanelList();
                    return;
                }
                EditSelectedPanel();
                ShowPanelControls();
            }
            //apply modified properties at the very end.
            serializedObject.ApplyModifiedProperties();
        }

        //Obsolete code. This wouldve been used for the freeform editing, but that no longer exists so why bother.
        //private void OnSceneGUI() 
        //{
        //    if(panelIndex < panelList.arraySize && panelList.arraySize > 0)
        //    {
        //        var element = panelList.GetArrayElementAtIndex(panelIndex);
        //        Transform t = (Transform)element.FindPropertyRelative("transform").objectReferenceValue;
        //        t.position = Handles.PositionHandle(t.position, t.rotation);
        //    }
        //}

        ///<summary>Clears the panelList and resets the editor references.</summary>
        void ClearPanelList()
        {
            panelList.ClearArray(); //clean the array.
            //block of just resets.
            panelTransform = null;
            panelRenderer = null;
            panelTransitionTime = null;
            panelTransitionCurve = null;
            panelPlacement = null;
            panelIndex = 0;
            serializedObject.ApplyModifiedProperties();
            //force the editor to refresh.
            Repaint();
        }

        ///<summary>Shows the UI buttons that control the panel list. Deletion, Creation, Navigation, etc.</summary>
        void ShowPanelControls()
        {
            if(foldout) //The buttons should only show with the foldout being active, otherwise they are kind of pointless.
            {
                EditorGUILayout.BeginHorizontal();
                GUI.backgroundColor = Color.gray;
                if(panelIndex > 0)
                {
                    //jump to start of array.
                    if(GUILayout.Button("<<"))
                        InitEdit(0);
                    //previous panel
                    if(GUILayout.Button("<"))
                    InitEdit(panelIndex-1);
                }
                //calculate start and end index.
                int maxIndex = panelList.arraySize-1;
                int remaining = maxIndex - panelIndex; //panels between the current and the last index
                int minOffset = 4 - Mathf.Clamp(remaining, 0, 2); //the offset from current to the first displayed. when panelIndex is the last, this is 4.
                int startIndex = Mathf.Max(0, panelIndex-minOffset);
                int endIndex = Mathf.Min(panelList.arraySize-1, panelIndex+4-(panelIndex-startIndex));
                //show all the options.
                for(int i = startIndex; i <= endIndex; i++)
                {
                    if(i == panelIndex) //current selected given highlighting.
                        GUI.backgroundColor = Color.white;
                    else
                        GUI.backgroundColor = Color.gray;
                    //select the panel i
                    if(GUILayout.Button(i.ToString()))
                        InitEdit(i);
                }
                if(panelIndex < maxIndex)
                {
                    //next panel.
                    if(GUILayout.Button(">"))
                        InitEdit(panelIndex+1);
                    //jump to end of array.
                    if(GUILayout.Button(">>"))
                        InitEdit(maxIndex);
                }
                EditorGUILayout.EndHorizontal();
                //TODO: swapping panels?

                //Adding and removing panels. //Yes i am adding {} for readability.
                EditorGUILayout.BeginHorizontal();
                {
                    //Removing panels with red button.
                    Color defaultColor = GUI.backgroundColor;
                    GUI.backgroundColor = Color.red;
                    if(GUILayout.Button("Remove Selected Panel"))
                        RemovePanel(panelIndex);
                    //Adding panels with green button.
                    GUI.backgroundColor = Color.green;
                    if(GUILayout.Button("Add Panel After"))
                        AddPanel(panelIndex);
                    GUI.backgroundColor = defaultColor;
                }
                EditorGUILayout.EndHorizontal();
            }
            //Arranging the panels for preview purposes.
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Arrange Tiles"))
            {
                manager.AutoArrangePanels(false);
            }
            autoRefreshArrangement = EditorGUILayout.Toggle("Auto Re-Arrange Panels", autoRefreshArrangement);
            EditorGUILayout.EndHorizontal();

        }

        ///<summary>Initialize Editing for a panel at the given index</summary>
        void InitEdit(int index)
        {
            panelIndex = index;
            manager.lastEditedPanel = panelIndex;
            FindRelativeProperties(panelList.GetArrayElementAtIndex(panelIndex));
            Selection.activeObject = ((Transform)panelTransform.objectReferenceValue).gameObject;
            SceneView.lastActiveSceneView.FrameSelected();
        }

        ///<summary>Draw the editor box for just the selected panel.</summary>
        void EditSelectedPanel()
        {
            //Draw the box around the selected panel editor.
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            
            foldout = EditorGUILayout.Foldout(foldout, $"Current Panel: {panelIndex}");
            if(foldout)
            {
                //Time for properties yay!!!
                EditorGUILayout.PropertyField(panelTransform);
                EditorGUILayout.PropertyField(panelRenderer);
                EditorGUILayout.PropertyField(panelTransitionTime);
                EditorGUILayout.PropertyField(panelTransitionCurve);
                //temporary buffer
                var previousPlacement = panelPlacement.enumValueIndex;
                EditorGUILayout.PropertyField(panelPlacement);
                //check whether the value has changed, auto refresh if enabled.
                if(autoRefreshArrangement && previousPlacement != panelPlacement.enumValueIndex)
                {
                    serializedObject.ApplyModifiedProperties(); //this is odd but ok
                    manager.AutoArrangePanels();
                }
                if(panelIndex > 0)
                {
                    SerializedProperty previous = panelList.GetArrayElementAtIndex(panelIndex-1);
                    var prePlace = previous.FindPropertyRelative("placement").enumValueIndex;
                    var panPlace = panelPlacement.enumValueIndex;
                    //check if we got an Above+Below or Below+Above
                    if(prePlace == (int)PanelPlacement.Above && panPlace == (int)PanelPlacement.Below ||
                        prePlace == (int)PanelPlacement.Below && panPlace == (int)PanelPlacement.Above)
                        //Address the issue.
                        EditorGUILayout.HelpBox("Two consecutive panels are placed Below and Above the previous. This is not supported.", MessageType.Error);
                }
                if(frameSpriteProperty.objectReferenceValue)
                    ShowSubPanelWorkflow();
            }
            
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        ///<summary>Shows the subpanel workflow options for the current panel.</summary>
        void ShowSubPanelWorkflow()
        {
            //1. check whether it is not a subpanel already.
            if(panelSubPanels.arraySize == 0)
            {
                //1.1. give option to convert it. Change main renderer sprite, instantiate first subpanel being the old panel sprite.
                if(GUILayout.Button("Set to use Sub-Panels.")) //This shouldnt be displayed, if the panel is a minigame.
                    ConvertSelectedPanelToMulti();
            }
            else 
            {
                //2. if its a subpanel holder already
                EditorGUILayout.LabelField("Sub-Panels:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                for(int i = 0; i < panelSubPanels.arraySize; i++)
                {
                    //2.1. show the list of subpanels
                    var element = panelSubPanels.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(element);
                }
                EditorGUI.indentLevel--;
                //2.2. add drop-in for new sprites that will auto-create new subpanels.
                Sprite sprite = null;
                if(sprite = (Sprite)EditorGUILayout.ObjectField("Add SubPanel from Sprite:", null, typeof(Sprite), false))
                {
                    CreateSubPanel(panelSubPanels, sprite);
                }
            }
        }

        ///<summary>Converts the selected panel into one that uses sub-panels.</summary>
        void ConvertSelectedPanelToMulti()
        {
            //1. Get the sprite from the renderer, and the in-scene size.
            SpriteRenderer mainRenderer = (SpriteRenderer)panelRenderer.objectReferenceValue;
            Sprite mainSprite = mainRenderer.sprite;
            //Bounds bounds = mainRenderer.bounds; //Might be a possibility.
            //2. reset scale if needed
            mainRenderer.transform.localScale = Vector3.one;
            //3. instantiate new sprite renderer object, set original sprite
            CreateSubPanel(panelSubPanels, mainSprite);
            //4. assign framesprite to main renderer.
            mainRenderer.drawMode = SpriteDrawMode.Sliced;
            mainRenderer.sprite = (Sprite)frameSpriteProperty.objectReferenceValue;
            //re-set the size to how it was before?
        }

        ///<summary>Creates a SubPanel based on the sprite and the current selected panel, and adds it to the subPanelArray.</summary>
        void CreateSubPanel(SerializedProperty subPanelArray, Sprite sprite)
        {
            Transform parent = (Transform)panelTransform.objectReferenceValue;
            GameObject subPanel = new GameObject("new Sub-Panel");
            subPanel.transform.parent = parent;
            subPanel.transform.position = parent.position;
            //create the sprite renderer and assign the sprite immediately. who needs temporary variables lmao.
            var subRenderer = subPanel.AddComponent<SpriteRenderer>();
            subRenderer.sprite = sprite;
            //Insert the subRenderer into the subPanelArray. by default at the last position.
            subPanelArray.InsertArrayElementAtIndex(subPanelArray.arraySize);
            subPanelArray.GetArrayElementAtIndex(subPanelArray.arraySize-1).objectReferenceValue = subRenderer;
        }

        ///<summary>Finds all the relative properties of the current element of the panel list and fills the serializedproperty variables with them.</summary>
        void FindRelativeProperties(SerializedProperty panelDataInstanceProperty)
        {
            //find all the properties of the panel.
            panelTransform       = panelDataInstanceProperty.FindPropertyRelative("transform");
            panelRenderer        = panelDataInstanceProperty.FindPropertyRelative("spriteRenderer");
            panelTransitionTime  = panelDataInstanceProperty.FindPropertyRelative("transitionTime");
            panelTransitionCurve = panelDataInstanceProperty.FindPropertyRelative("transitionCurve");
            panelPlacement       = panelDataInstanceProperty.FindPropertyRelative("placement");
            panelSubPanels       = panelDataInstanceProperty.FindPropertyRelative("subPanels");
        }
    
        ///<summary>Presents the UI to intialize the panel list.</summary>
        void DrawListInitialization()
        {
            EditorGUILayout.HelpBox("Drag an object from the hierarchy into this slot to use its children.", MessageType.Info);
            Transform target = (Transform)EditorGUILayout.ObjectField(null, typeof(Transform), true);
            if(target ) //target already has children (those are panels)
            {
                //override the parent.
                panelParentProperty.objectReferenceValue = target;

                //Fill the panelList with existing panels.
                if(target.childCount > 0)
                    foreach(Transform p in target)
                    {
                        panelList.InsertArrayElementAtIndex(panelList.arraySize);
                        var element = panelList.GetArrayElementAtIndex(panelList.arraySize-1);
                        InitializePanelElementWithDefaults(element, p);
                    }
                else //There are no panels yet, instantiate one to begin with.
                {
                    Transform transform = Instantiate<GameObject>(panelPrefabProperty.objectReferenceValue as GameObject).transform;
                    panelList.InsertArrayElementAtIndex(0);
                    var element = panelList.GetArrayElementAtIndex(0);
                    InitializePanelElementWithDefaults(element, transform);
                }
                
                //Reset the inspector.
                panelIndex = 0;
                FindRelativeProperties(panelList.GetArrayElementAtIndex(panelIndex));
                //never forget to apply the modified properties at the end.
                serializedObject.ApplyModifiedProperties();
                Repaint();
            }
        }

        ///<summary>Creates a new panel in the scene and adds it to the list</summary>
        void AddPanel(int insertionIndex)
        {
            //yuck!
            GameObject instance = Instantiate((GameObject)panelPrefabProperty.objectReferenceValue, 
                                            Vector3.zero, 
                                            Quaternion.identity, 
                                            (Transform)panelParentProperty.objectReferenceValue);
            panelList.InsertArrayElementAtIndex(insertionIndex);
            var element = panelList.GetArrayElementAtIndex(insertionIndex);
            InitializePanelElementWithDefaults(element, instance.transform);
            panelList.MoveArrayElement(insertionIndex, insertionIndex+1);
            serializedObject.ApplyModifiedProperties();
            //refresh the panelIndex and properties.
            InitEdit(insertionIndex+1);
            //move the panels to where they belong.
            if(autoRefreshArrangement)
                manager.AutoArrangePanels(false);

        }

        ///<summary>Deletes a panel from the list, and optionally deletes the associated GameObject</summary>
        void RemovePanel(int deletionIndex, bool deleteObject = true)
        {
            if(deleteObject)
            {
                var element = panelList.GetArrayElementAtIndex(deletionIndex);
                DestroyImmediate((element.FindPropertyRelative("transform").objectReferenceValue as Transform).gameObject);
            }
            panelList.DeleteArrayElementAtIndex(deletionIndex);
            serializedObject.ApplyModifiedProperties();
            InitEdit(Mathf.Clamp(panelIndex-1, 0, panelList.arraySize-1));
        }

        ///<summary>Initializes an element in the panelList with default values from the source Transform.</summary>
        void InitializePanelElementWithDefaults(SerializedProperty panelListElement, Transform source)
        {
            panelListElement.FindPropertyRelative("transform").objectReferenceValue = source.transform;
            panelListElement.FindPropertyRelative("spriteRenderer").objectReferenceValue = source.GetComponent<SpriteRenderer>();
            panelListElement.FindPropertyRelative("transitionCurve").animationCurveValue = AnimationCurve.EaseInOut(0,0,1,1);
            panelListElement.FindPropertyRelative("transitionTime").floatValue = 1f;
        }
    }
}
