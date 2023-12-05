using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public class SplineSampler:MonoBehaviour
{
    public SplineContainer SplineContainer;
    public float Width = 1;
    
    public int Index;
    [Range(0, 1)] public  float time;

    float3 _position;
    float3 _tangent;
    float3 _upVector;

    private Vector3 _p1;
    private Vector3 _p2;
    
    
    void Update()
    {
        SplineContainer.Evaluate(Index, time,out _position,out _tangent,out _upVector);
        
        Vector3 right = Vector3.Cross(_tangent, _upVector).normalized;
        _p1 = (Vector3) _position + (Width * right);
        _p2 = (Vector3) _position - (Width * right);
    }

    private void OnDrawGizmos() {
        Handles.matrix = transform.localToWorldMatrix;
        Handles.SphereHandleCap(0, _p1, quaternion.identity, 1, EventType.Repaint);
        Handles.SphereHandleCap(0, _p2, quaternion.identity, 1, EventType.Repaint);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_p1, _p2);
    }
    
   
} 
