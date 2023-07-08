using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using script;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;
using Vector3 = UnityEngine.Vector3;

[CustomEditor(typeof(GridManager))]
public class EditorGridManager : Editor
{
   private GridManager m_targget;
   private void OnSceneGUI()
   {
      m_targget = (GridManager) target;

      Vector3[] point = new Vector3[4];
      Handles.color = Color.yellow;

      Vector3 p1 = m_targget.Offset - new Vector3(0.5f,0,0.5f);
      Vector3 p2 = m_targget.Offset + new Vector3(m_targget.Size.x-0.5f , 0, -0.5f);
      point[0] = p2;
      Handles.DrawDottedLine(p1, p2, 1);
      p1 = p2;
      p2 = m_targget.Offset + new Vector3(m_targget.Size.x-0.5f ,0,m_targget.Size.y-1+0.5f );
      point[1] = p2;
      Handles.DrawDottedLine(p1, p2, 1);
      p1 = p2;
      p2 = m_targget.Offset + new Vector3( -0.5f,0, m_targget.Size.y-1 + 0.5f);
      point[2] = p2;
      Handles.DrawDottedLine(p1, p2, 1);
      p1 = p2;
      p2 = m_targget.Offset - new Vector3(0.5f,0,0.5f);
      point[3] = p2;
      Handles.DrawDottedLine(p1, p2, 1);
      Handles.color = Color.yellow*new Color(1,1,1,0.2f);
      Handles.DrawAAConvexPolygon(point);
   }

   public override void OnInspectorGUI()
   {
      base.OnInspectorGUI();
      m_targget = (GridManager) target;

      if( GUILayout.Button("CalculatTheGrid"))m_targget.GenerateCells();
      if (GUILayout.Button("DisplayCells")) m_targget.GenerateDebugCells();
      if (GUILayout.Button("CalculateColliders")) m_targget.CheckColliders();
      if (GUILayout.Button("ClearDebugsCells")) m_targget.ClearGrid();
      
   }
}
