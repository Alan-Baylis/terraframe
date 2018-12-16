using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerrainGeneration_PluginBase.WorkflowData
{
    public class HeightmapWorkflowData : WorkflowData
    {
        public float[][] HeightmapCells{ get; set; }
        public float CellSize { get; set; }
        public float MinHeight { get; set; }
        public float MaxHeight { get; set; }

        public HeightmapWorkflowData()
        {
            LayerType = LayerType.HeightMap;
        }
    }
}
