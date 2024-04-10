
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class SpineRoad : MonoBehaviour
{

    [Header("References")]
    public SplineContainer SplineContainer;
    public MeshFilter MeshFilter;
    
    [Header("Parameters")]
    public float Width = 1;
    
    public int Index;
    public int Resolution = 6;
    public float CurveStep = 0.2f;
    [Range(0, 1)] public  float time;

    float3 _position;
    float3 _tangent;
    float3 _upVector;

    List<Vector3> _p1;
    List<Vector3> _p2;
    public List<JunctionBuilderOverlay.Intersection> _intercection = new List<JunctionBuilderOverlay.Intersection>();


    void Update() {
        
        GetVerts();
    }

    private void OnEnable()=> Spline.Changed += OnSplineChange;
    private void OnDisable()=> Spline.Changed -= OnSplineChange;
    [ContextMenu("ClearJunctions")]
    public void ClearJunction() => _intercection.Clear();
    

    private void OnSplineChange(Spline arg1, int arg2, SplineModification arg3)
    {
        BuildMesh();
    }

    public void GetVerts()
    {
        _p1=new List<Vector3>();
        _p2=new List<Vector3>();

        float step = 1f /  (float)Resolution;
        Vector3 p1;
        Vector3 p2;
        for (int j = 0; j < Index; j++) {
            for (int i = 0; i < Resolution; i++) {
                float t = step * i;
                SampleSplineWidth(j,t, out  p1, out  p2);
                _p1.Add(p1);
                _p2.Add(p2);
            }
            SampleSplineWidth(j,1, out  p1, out  p2);
            _p1.Add(p1);
            _p2.Add(p2);
        }
    }

    public void AddJunction(JunctionBuilderOverlay.Intersection junctionInfo)
    {
        _intercection.Add(junctionInfo);
        Debug.Log("DoTRhat with "+junctionInfo.junctions.Count);
        BuildMesh();
    }

    private void SampleSplineWidth(int j,float t, out Vector3 p1, out Vector3 p2)
    {
        SplineContainer.Evaluate(j, t,out _position,out _tangent,out _upVector);
        
        Vector3 right = Vector3.Cross(_tangent, _upVector).normalized;
        p1 = (Vector3) _position + (Width * right);
        p2 = (Vector3) _position - (Width * right);
    }

    [ContextMenu("Build Mesh")]
    private void BuildMesh()
    {
        Mesh m = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        int offset = 0;

        int lenghth = _p2.Count;

        for (int currentSplineIndex = 0; currentSplineIndex < Index; currentSplineIndex++) {
            int splineOffset = Resolution * currentSplineIndex;
            splineOffset += currentSplineIndex;

            for (int i = 1; i < Resolution + 1; i++) {
                int vertOffset = splineOffset + i;
                Vector3 p1 = _p1[vertOffset - 1];
                Vector3 p2 = _p2[vertOffset - 1];
                Vector3 p3 = _p1[vertOffset];
                Vector3 p4 = _p2[vertOffset];


                offset = 4 * Resolution * currentSplineIndex;
                offset += 4 * (i - 1);

                int t1 = offset + 0;
                int t2 = offset + 2;
                int t3 = offset + 3;

                int t4 = offset + 3;
                int t5 = offset + 1;
                int t6 = offset + 0;

                verts.AddRange(new List<Vector3>() {p1, p2, p3, p4});
                tris.AddRange(new List<int>() {t1, t2, t3, t4, t5, t6});
            }
        }

        offset = verts.Count;
        for (int i = 0; i < _intercection.Count; i++)
        {

            JunctionBuilderOverlay.Intersection intersection = _intercection[i];
            int count = 0;
            List<JunctionBuilderOverlay.JunctionEdge> junctionEdge = new List<JunctionBuilderOverlay.JunctionEdge>();

            //List<Vector3> points = new List<Vector3>();
            Vector3 center = new Vector3();
            foreach (var junction in intersection.getJunctionInfos())
            {
                int splineIndex = junction.SplineIndex;
                float t = junction.KnotIndex == 0 ? 0 : 1;
                SampleSplineWidth(splineIndex, t, out Vector3 p1, out Vector3 p2);

                if (junction.KnotIndex == 0) junctionEdge.Add(new JunctionBuilderOverlay.JunctionEdge(p1, p2));
                else junctionEdge.Add(new JunctionBuilderOverlay.JunctionEdge(p2, p1));
                center += p1;
                center += p2;
                count++;
            }

            center = center / (junctionEdge.Count*2);
            
            //junctionEdge.Sort((x,y)=> {
            //    Vector3 xDir = x - center;
            //    Vector3 yDir = y - center;
            //    float angleA = Vector3.SignedAngle(center.normalized, xDir.normalized, Vector3.up);
            //    float angleB = Vector3.SignedAngle(center.normalized, yDir.normalized, Vector3.up);
//
            //    if (angleA > angleB) return 1;
            //    if (angleA < angleB) return -1;
            //     return 0;
//
            //});
            
            junctionEdge.Sort((x,y)=> SortPoints(center, x.Center, y.Center));

            //int pointsOffset = verts.Count;
            //for (int j = 1; j <= points.Count; j++) {
            //    verts.Add(center);
            //    verts.Add(points[j-1]);
            //    if (j == points.Count) verts.Add(points[0]);
            //    else verts.Add(points[j]);
            //    
            //    tris.Add(pointsOffset+((j-1)*3)+0);
            //    tris.Add(pointsOffset+((j-1)*3)+1);
            //    tris.Add(pointsOffset+((j-1)*3)+2);
            //}
            List<Vector3> curvePoints = new List<Vector3>();
            Vector3 mid;
            Vector3 c;
            Vector3 b;
            Vector3 a;
            BezierCurve curve;

            for (int j = 1; j <= junctionEdge.Count; j++) {
                a = junctionEdge[j - 1].Left;
                curvePoints.Add(a);
                b = (j < junctionEdge.Count) ? junctionEdge[j].Right : junctionEdge[0].Right;
                mid = Vector3.Lerp(a, b, 0.5f);
                Vector3 dir = center - mid;
                mid = mid - dir;
                c = Vector3.Lerp(mid, center, intersection.Curves[j-1]);
                curve = new BezierCurve(a, c, b);
                for (float t = 0; t < 1f; t+=CurveStep)
                {
                    Vector3 pos = CurveUtility.EvaluatePosition(curve, t);
                    curvePoints.Add(pos);
                }
                curvePoints.Add(b);
            }
            curvePoints.Reverse();

            int pointsOffset = verts.Count;
            for (int j = 1; j <= curvePoints.Count; j++)
            {
                verts.Add(center);
                verts.Add(curvePoints[j-1]);
                if( j==curvePoints.Count) verts.Add(curvePoints[0]);
                else verts.Add( curvePoints[j]);
                
                tris.Add( pointsOffset+((j-1)*3)+0);
                tris.Add( pointsOffset+((j-1)*3)+1);
                tris.Add( pointsOffset+((j-1)*3)+2);
            }
        }

        m.SetVertices(verts);
        m.SetTriangles(tris, 0);
        MeshFilter.mesh = m;
        Debug.Log("Done that vetices =" + verts.Count);
        
    }

    private int SortPoints(Vector3 center, Vector3 x, Vector3 y) {
        
            Vector3 xDir = x - center;
            Vector3 yDir = y - center;
            float angleA = Vector3.SignedAngle(center.normalized, xDir.normalized, Vector3.up);
            float angleB = Vector3.SignedAngle(center.normalized, yDir.normalized, Vector3.up);

            if (angleA > angleB) return -1;
            if (angleA < angleB) return 1;
            return 0;

        
    }

    private void OnDrawGizmos() {
        Handles.matrix = transform.localToWorldMatrix;
        if (_p1 == null) return;
        for (int i = 0; i < _p1.Count; i++) {
            Handles.SphereHandleCap(0, _p1[i], quaternion.identity, 1, EventType.Repaint);
            Handles.SphereHandleCap(0, _p2[i], quaternion.identity, 1, EventType.Repaint);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_p1[i], _p2[i]);
        }
        
    }
}