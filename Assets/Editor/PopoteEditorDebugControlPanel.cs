using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class PopoteEditorDebugControlPanel : EditorWindow
{
   [MenuItem("PopoteTool/DebugControlPanel")]
   public static void ShowWindow()
   {
      PopoteEditorDebugControlPanel window = GetWindow<PopoteEditorDebugControlPanel>();
      window.titleContent = new GUIContent("Debug Control Panel");
   }
   

   public void OnGUI()
   {
      if (EditorControlStatics.DisplayGizmos) {
         if (GUILayout.Button("Hide Gizmos")) EditorControlStatics.DisplayGizmos = false;
      }
      else {
         if (GUILayout.Button("Show Gizmos")) EditorControlStatics.DisplayGizmos = true;
      }
      if (EditorControlStatics.DisplayEnnemisDangerZone) {
         if (GUILayout.Button("Hide Ennemis DangerZone")) EditorControlStatics.DisplayEnnemisDangerZone = false;
      }
      else {
         if (GUILayout.Button("Show Ennemis DangerZone")) EditorControlStatics.DisplayEnnemisDangerZone = true;
      }
      if (EditorControlStatics.DisplayDetectionZone) {
         if (GUILayout.Button("Hide Ennemis Detection")) EditorControlStatics.DisplayDetectionZone = false;
      }
      else {
         if (GUILayout.Button("Show Ennemis Detection")) EditorControlStatics.DisplayDetectionZone = true;
      }
   }
}
