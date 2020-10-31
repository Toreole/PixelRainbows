﻿using System.Collections;
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

        private void OnEnable() 
        {
            manager = target as PanelManager;
            cameraProperty = serializedObject.FindProperty("camera");
            panelList = serializedObject.FindProperty("panels");
            panelPrefabProperty = serializedObject.FindProperty("blankPanelPrefab");
            panelParentProperty = serializedObject.FindProperty("panelParent");
            //initialize the selectedPanel properties.
            if(panelList.arraySize > 0)
            {
                selectedPanel = panelList.GetArrayElementAtIndex(panelIndex);
                FindRelativeProperties(selectedPanel);
            }
        }

        public override void OnInspectorGUI()
        {
            //the three basic properties.
            EditorGUILayout.PropertyField(panelPrefabProperty);
            EditorGUILayout.PropertyField(cameraProperty);
            EditorGUILayout.PropertyField(panelParentProperty);

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
                if(GUILayout.Button("Clear Panel Data"))
                {
                    panelList.ClearArray();
                    panelTransform = null;
                    panelRenderer = null;
                    panelTransitionTime = null;
                    panelTransitionCurve = null;
                    panelPlacement = null;
                    panelIndex = 0;
                    serializedObject.ApplyModifiedProperties();
                    Repaint();
                    return;
                }
                
                //Draw the box around the selected panel editor.
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUI.indentLevel++;
                
                foldout = EditorGUILayout.Foldout(foldout, $"Current Panel: {panelIndex}");
                if(foldout)
                {
                    EditorGUILayout.PropertyField(panelTransform);
                    EditorGUILayout.PropertyField(panelRenderer);
                    EditorGUILayout.PropertyField(panelTransitionTime);
                    EditorGUILayout.PropertyField(panelTransitionCurve);
                    EditorGUILayout.PropertyField(panelPlacement);
                }
                
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
                if(foldout) //The buttons should only show with the foldout being active, otherwise they are kind of pointless.
                {
                    //Navigation.
                    EditorGUILayout.BeginHorizontal();
                    if(panelIndex > 0 && GUILayout.Button("Previous"))
                    {
                        panelIndex--;
                        FindRelativeProperties(panelList.GetArrayElementAtIndex(panelIndex));
                    }
                    if(panelIndex < panelList.arraySize-1 && GUILayout.Button("Next"))
                    {
                        panelIndex++;
                        FindRelativeProperties(panelList.GetArrayElementAtIndex(panelIndex));
                    }
                    EditorGUILayout.EndHorizontal();
                    //Adding and removing panels.
                    EditorGUILayout.BeginHorizontal();
                    Color defaultColor = GUI.backgroundColor;
                    GUI.backgroundColor = Color.red;
                    if(GUILayout.Button("Remove Selected Panel"))
                        RemovePanel(panelIndex);
                    GUI.backgroundColor = Color.green;
                    if(GUILayout.Button("Add Panel After"))
                        AddPanel(panelIndex);
                    GUI.backgroundColor = defaultColor;
                    EditorGUILayout.EndHorizontal();
                }

            }
            //apply modified properties at the very end.
            serializedObject.ApplyModifiedProperties();
        }

        void FindRelativeProperties(SerializedProperty panelDataInstanceProperty)
        {
            //find all the properties of the panel.
            panelTransform       = panelDataInstanceProperty.FindPropertyRelative("transform");
            panelRenderer        = panelDataInstanceProperty.FindPropertyRelative("spriteRenderer");
            panelTransitionTime  = panelDataInstanceProperty.FindPropertyRelative("transitionTime");
            panelTransitionCurve = panelDataInstanceProperty.FindPropertyRelative("transitionCurve");
            panelPlacement       = panelDataInstanceProperty.FindPropertyRelative("placement");
        }
    
        void DrawListInitialization()
        {
            EditorGUILayout.HelpBox("Drag an object from the hierarchy into this slot to use its children.", MessageType.Info);
            Transform target = (Transform)EditorGUILayout.ObjectField(null, typeof(Transform), true);
            if(target && target.childCount > 0)
            {
                //override the parent.
                panelParentProperty.objectReferenceValue = target;
                foreach(Transform p in target)
                {
                    panelList.InsertArrayElementAtIndex(panelList.arraySize);
                    var element = panelList.GetArrayElementAtIndex(panelList.arraySize-1);
                    InitializePanelElementWithDefaults(element, p);
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
            panelList.InsertArrayElementAtIndex(insertionIndex+1);
            var element = panelList.GetArrayElementAtIndex(insertionIndex);
            InitializePanelElementWithDefaults(element, instance.transform);
        }

        void RemovePanel(int deletionIndex)
        {
            panelList.DeleteArrayElementAtIndex(deletionIndex);
            if(panelIndex >= panelList.arraySize)
                panelIndex = panelList.arraySize -1;
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
