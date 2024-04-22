using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitStartOrder))]
public class EditorUnitStartOrder : Editor {
    private UnitStartOrder m_targget;
    private void OnSceneGUI()
    {
        m_targget = (UnitStartOrder) target;

        if (m_targget.Targets.Length == 0||m_targget.Targets[0]==null) return;
        //Gizmos.color = Color.blue;
        //Gizmos.DrawSphere(m_targget.Targets[0].position, 1);
        Handles.color = Color.blue;
        Handles.DrawWireDisc(m_targget.Targets[0].position, Vector3.up, 3);
        Handles.color = Color.green;
        for (int i = 1; i < m_targget.Targets.Length; i++) {
            if (m_targget.Targets[i] == null) return;
            Handles.DrawDottedLine(m_targget.Targets[i-1].position, m_targget.Targets[i].position, 10);
            if (i ==m_targget.Targets.Length-1)
            {
                Handles.color = Color.green;
                Handles.DrawWireDisc(m_targget.Targets[i].position, Vector3.up, 2);
            }
        }
    }
}