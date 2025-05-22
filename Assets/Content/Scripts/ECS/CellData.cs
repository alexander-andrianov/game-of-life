using Unity.Entities;

namespace Content.Scripts.ECS
{
    public struct CellData : IComponentData
    {
        public bool IsAlive;
        public int X;
        public int Y;
        public bool PreviousAlive;
        public int StableCount;
    }
} 