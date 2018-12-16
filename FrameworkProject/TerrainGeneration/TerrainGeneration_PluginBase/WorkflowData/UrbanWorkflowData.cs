using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGeneration_PluginBase.WorkflowSubData;

namespace TerrainGeneration_PluginBase.WorkflowData
{
    public class UrbanWorkflowData : WorkflowData
    {
        public RoadPath[] StreetPaths { get; set; }
        public ObjectArea[] BuildingAreas { get; set; }
        public TerrainOutputMesh[] StreetModels { get; set; }
        public TerrainOutputMesh[] BuildingModels { get; set; }

        public UrbanWorkflowData()
        {
            LayerType = LayerType.Urban;
        }
    }
}
