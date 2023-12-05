using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Splines;
using UnityEditor.Splines.script.SplineExtension;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

[Overlay(typeof(SceneView), "Junction builder", true)]
public class JunctionBuilderOverlay : Overlay
{
    private Label SelectionInfoLabel =new Label();
    private Button ButtonBuildJoiunction = new Button();
    
   public override VisualElement CreatePanelContent() {
       UpdateSelectionInfo();
       Debug.Log(("DoThat"));
       var root = new VisualElement();
       root.Add(SelectionInfoLabel);
       ButtonBuildJoiunction.text = "Build Junction";
       ButtonBuildJoiunction.clickable.clicked += OnBuildJunction;
       root.Add( ButtonBuildJoiunction);
       SplineSelection.changed += OnSelectionChanged;
       return root;
   }

   private void OnBuildJunction()
   {
       List<SelectedSplineElementInfo> selection = SplineToolUtility.GetSelection();
       Intersection intersection = new Intersection();

       Debug.Log("Do That");
       foreach (SelectedSplineElementInfo item in selection)
       {
           SplineContainer container = (SplineContainer)item.Target ;
           Spline spline = container.Splines[item.TargetIndex];
           intersection.AddJunction( item.TargetIndex, item.KnotIndex, spline, spline.Knots.ToList()[item.KnotIndex]);
       }
       Selection.activeObject.GetComponent<SpineRoad>().AddJunction(intersection);
       
   }
   
   private void OnSelectionChanged()=> UpdateSelectionInfo();
   private void ClearSelectionInfo()=> SelectionInfoLabel.text = "";
   
   
   private void UpdateSelectionInfo()
   {
       ClearSelectionInfo();

       List<SelectedSplineElementInfo> infos = SplineToolUtility.GetSelection();

       foreach (var element in infos) {
           SelectionInfoLabel.text += $"Splne{element.TargetIndex}, Knot {element.KnotIndex}\n";
           SplineContainer spline = (SplineContainer) element.Target;
           
       }
       
   }
   
   public struct JunctionInfo
   {
       public int SplineIndex;
       public int KnotIndex;
       public Spline Spline;
       public BezierKnot Knot;
       public JunctionInfo(int splineIndex , int knotIndex , Spline spline , BezierKnot  knot) {
           SplineIndex = splineIndex;
           KnotIndex = knotIndex;
           Spline = spline;
           Knot = knot;
       }
   }
   
   public struct JunctionEdge
   {
       public Vector3 Left;
       public Vector3 Right;

       public Vector3 Center => (Left + Right) / 2;

       public JunctionEdge(Vector3 left, Vector3 right) {
           Left = left;
           Right = right;
       }
   }

   
   [Serializable]
   public class Intersection
   {
       public List<JunctionInfo> junctions;
       [Range( 0,1)]public List<float> Curves;

       public void AddJunction(int splineIndex, int knotIndex, Spline spline, BezierKnot knot) {
           if (junctions == null) {
               junctions = new List<JunctionInfo>();
               Curves = new List<float>();
           }
           
           junctions.Add(new JunctionInfo(splineIndex, knotIndex, spline, knot));
           Curves.Add(0.5f);
       }

       internal IEnumerable<JunctionInfo> getJunctionInfos()
       {
           return junctions;
       }
   }
   
}
