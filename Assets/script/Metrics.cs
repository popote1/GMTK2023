using System.Xml;
using UnityEngine;

namespace script
{
    public static class Metrics {
        public const int chunkSize = 10;
        
        //FlowField Parameters
        public const int FlowFieldCellPerFrame = 2000;
        public const int MoveCost=10;
        public const int DiagonalMoveCost = 14;

        public const float ZombieSpawnOffset =  0.5f;
    }
}