using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGeneration_PluginBase.WorkflowSubData;

namespace TerrainGeneration_PluginBase.WorkflowData
{
    public class VegetationWorkflowData : WorkflowData
    {
        public ObjectArea[] VegetationAreas { get; set; }
        public TerrainOutputMesh[] VegetationModels { get; set; }

        public VegetationWorkflowData()
        {
            LayerType = LayerType.Vegetation;
        }
    }
}
