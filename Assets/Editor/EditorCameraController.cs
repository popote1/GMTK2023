using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameratargetController))]
public class EditorCameraController : Editor
{
    private CameratargetController m_target;
    private void OnSceneGUI()
    {
        m_target = (CameratargetController) target;

        Vector3[] point = new Vector3[4];
        Handles.color = Color.blue;

        Vector3 p1 = new Vector3(m_target.MaxPosition.x,0,m_target.MaxPosition.y);
        Vector3 p2 = new Vector3(m_target.MaxPosition.x,0,m_target.MinPosition.y);
        point[0] = p2;
        Handles.DrawDottedLine(p1, p2, 1);
        p1 = p2;
        p2 =  new Vector3(m_target.MinPosition.x,0,m_target.MinPosition.y);
        point[1] = p2;
        Handles.DrawDottedLine(p1, p2, 1);
        p1 = p2;
        p2 = new Vector3(m_target.MinPosition.x,0,m_target.MaxPosition.y);
        point[2] = p2;
        Handles.DrawDottedLine(p1, p2, 1);
        p1 = p2;
        p2 = new Vector3(m_target.MaxPosition.x,0,m_target.MaxPosition.y);
        point[3] = p2;
        Handles.DrawDottedLine(p1, p2, 1);
        Handles.color = Color.blue*new Color(1,1,1,0.2f);
        Handles.DrawAAConvexPolygon(point);
        
       
    }
}
