using System.Collections.Generic;

namespace UnityEditor.Splines.script.SplineExtension
{
    public static class SplineToolUtility {
        
        public static bool HasSelection() {
            return SplineSelection.HasActiveSplineSelection();
        }

        public static List<SelectedSplineElementInfo> GetSelection() {
            List<SelectableSplineElement> elements = SplineSelection.selection;
            List<SelectedSplineElementInfo> infos = new List<SelectedSplineElementInfo>();
            
            foreach (var element in elements) {
                infos.Add(new SelectedSplineElementInfo(element.target , element.targetIndex, element.knotIndex));
            }
            return infos;
        }
        
        
    }

    public struct SelectedSplineElementInfo
    {
        public object Target;
        public int TargetIndex;
        public int KnotIndex;
        public SelectedSplineElementInfo(object Object, int Index, int knot)
        {
            Target = Object;
            TargetIndex = Index;
            KnotIndex = knot;
        }
    }
}