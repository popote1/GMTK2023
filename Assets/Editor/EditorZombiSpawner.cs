using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ZombieSpawner))]
public class EditorZombiSpawner : Editor
{
    private ZombieSpawner m_target;
    private void OnSceneGUI() {
        m_target = (ZombieSpawner) target;
        
    }
    
}
