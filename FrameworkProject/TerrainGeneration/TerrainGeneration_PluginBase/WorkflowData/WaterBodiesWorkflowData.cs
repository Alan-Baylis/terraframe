using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerrainGeneration_PluginBase.WorkflowData
{
    public class WaterBodiesWorkflowData : WorkflowData
    {
        public WaterPath[] WaterPaths { get; set; }
        public TerrainOutputMesh[] WaterModels { get; set; }

        public WaterBodiesWorkflowData()
        {
            LayerType = LayerType.WaterBodies;
        }
    }
}
