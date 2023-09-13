    using script;
    using UnityEditor;
    using UnityEngine;
    [CustomEditor(typeof(CivillianAgent))]
    public class EditorCivillian :Editor
    {
        private CivillianAgent m_target;
        private void OnSceneGUI() {
            m_target = (CivillianAgent) target;
            Handles.color = Color.blue*new Color(1,1,1,0.2f);
            Handles.DrawSolidDisc(m_target.transform.position, Vector3.up, m_target.DetectionDistance);
        }
    }
