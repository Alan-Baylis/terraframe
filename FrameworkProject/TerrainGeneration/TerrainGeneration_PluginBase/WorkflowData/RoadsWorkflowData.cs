using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGeneration_PluginBase.WorkflowSubData;

namespace TerrainGeneration_PluginBase.WorkflowData
{
    public class RoadsWorkflowData : WorkflowData
    {
        public RoadPath[] RoadPaths { get; set; }
        public TerrainOutputMesh RoadModels { get; set; }

        public RoadsWorkflowData()
        {
            LayerType = LayerType.Roads;
        }
    }
}
