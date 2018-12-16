using System;
using System.Collections.Generic;
using TerrainGeneration_PluginBase;
using TerrainGeneration_PluginBase.WorkflowData;

namespace TerrainGeneration_RiversAndOceans
{
    class Plugin : TerrainPlugin
    {
        public string Author
        {
            get
            {
                return "Hugo P.";
            }
        }

        public string Description
        {
            get
            {
                return "Bypass plugin simulating water generation";
            }
        }

        public HashSet<LayerType> In
        {
            get
            {
                HashSet<LayerType> terrainTypes = new HashSet<LayerType>();
                terrainTypes.Add(LayerType.HeightMap);
                return terrainTypes;
            }
        }

        public string Name
        {
            get
            {
                return "Water Generation";   
            }
        }

        public HashSet<LayerType> Not
        {
            get
            {
                HashSet<LayerType> terrainTypes = new HashSet<LayerType>();
                terrainTypes.Add(LayerType.Vegetation);
                terrainTypes.Add(LayerType.Roads);
                terrainTypes.Add(LayerType.Urban);
                return terrainTypes;
            }
        }

        public HashSet<LayerType> Out
        {
            get
            {
                HashSet<LayerType> terrainTypes = new HashSet<LayerType>();
                terrainTypes.Add(LayerType.WaterBodies);
                return terrainTypes;
            }
        }

        public int Progress
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public List<TerrainInput> TerrainInputs
        {
            get
            {
                return new List<TerrainInput>();
            }
        }

        public List<TerrainOutput> StartProcess(List<TerrainInput> terrainInputs, Dictionary<LayerType, WorkflowData> workflowData)
        {
            List<TerrainOutput> terrainOutputs = new List<TerrainOutput>();
            return terrainOutputs;
        }
    }
}
