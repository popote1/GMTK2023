using System.Xml;
using UnityEngine;

namespace script
{
    public static class Metrics {
        public const int chunkSize = 10;

        public static Vector3 cellColliderBockSize = new Vector3(0.5f, 20, 0.5f);
        public const bool UsDebugCellGroundOffsetting = true;
        //FlowField Parameters
        public const int FlowFieldCellPerFrame = 2000;
        public const int MoveCost=10;
        public const int DiagonalMoveCost = 14;

        public const float ZombieSpawnOffset =  0.5f;
    }
}