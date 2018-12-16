using System.Collections.Generic;

namespace TerrainGeneration_Core
{
    class TerrainPipelineLayerComparator : IEqualityComparer<TerrainPipelineLayer>
    {
        public bool Equals(TerrainPipelineLayer x, TerrainPipelineLayer y)
        {
            return x.LayerCode.Equals(y.LayerCode);
        }

        public int GetHashCode(TerrainPipelineLayer obj)
        {
            return obj.LayerCode;
        }
    }
}
