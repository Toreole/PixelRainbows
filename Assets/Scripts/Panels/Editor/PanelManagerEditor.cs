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
        bool foldout = false;

        private void OnEnable() 
        {
            manager = target as PanelManager;
            cameraProperty = serializedObject.FindProperty("camera");
            panelList = serializedObject.FindProperty("panels");
            panelPrefabProperty = serializedObject.FindProperty("blankPanelPrefab");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(panelPrefabProperty);
            EditorGUILayout.PropertyField(cameraProperty);
            //make sure the camera is assigned before we do anything else
            if(!cameraProperty.objectReferenceValue || !panelPrefabProperty.objectReferenceValue)
            {
                EditorGUILayout.HelpBox("Camera and Prefab need to be assigned in order to edit.", MessageType.Warning);
                serializedObject.ApplyModifiedProperties();
                return;
            }
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUI.indentLevel++;
            foldout = EditorGUILayout.Foldout(foldout, "Current Panel (index)");
            if(foldout)
            {
                EditorGUILayout.LabelField("This is content");
                EditorGUILayout.LabelField("This is content");
                EditorGUILayout.LabelField("This is content");
                EditorGUILayout.LabelField("This is content");
            }
                EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            //apply modified properties at the very end.
            serializedObject.ApplyModifiedProperties();
        }
    }
}
