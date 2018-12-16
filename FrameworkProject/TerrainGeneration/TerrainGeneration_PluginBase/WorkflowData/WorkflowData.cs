using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TerrainGeneration_PluginBase.WorkflowData
{
    [XmlInclude(typeof(HeightmapWorkflowData)), XmlInclude(typeof(RoadsWorkflowData)), XmlInclude(typeof(UrbanWorkflowData)), XmlInclude(typeof(VegetationWorkflowData)), XmlInclude(typeof(WaterBodiesWorkflowData))]
    public class WorkflowData
    {
        public LayerType LayerType { get; set; }

        public WorkflowData() { }
    }
}
