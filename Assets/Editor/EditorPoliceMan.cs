    using script;
    using UnityEditor;
    using UnityEngine;
    [CustomEditor(typeof(PoliceMan))]
    public class EditorPoliceMan :Editor
    {
        private PoliceMan m_target;
        private void OnSceneGUI() {
            m_target = (PoliceMan) target;
            Handles.color = Color.red*new Color(1,1,1,0.2f);
            Handles.DrawSolidDisc(m_target.transform.position, Vector3.up, m_target.AttackRange);
        }
    }
