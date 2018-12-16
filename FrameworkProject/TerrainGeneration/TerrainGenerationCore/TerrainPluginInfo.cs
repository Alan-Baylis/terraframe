using System;
using System.Collections.Generic;
using TerrainGeneration_PluginBase;

namespace TerrainGeneration_Core
{
    public class TerrainPluginInfo
    {
        public string Name { get; }
        public int Id { get; }
        public string Description { get; }
        public string Author { get; }
        public HashSet<LayerType> Out { get; }
        public HashSet<LayerType> In { get; }
        public HashSet<LayerType> Not { get; }

        public TerrainPluginInfo(TerrainPlugin terrainPlugin)
        {
            Name = terrainPlugin.Name;
            Author = terrainPlugin.Author;
            Description = terrainPlugin.Description;
            Id = getPluginHashCode(terrainPlugin);
            Out = terrainPlugin.Out;
            In = terrainPlugin.In;
            Not = terrainPlugin.Not;
        }

        private static int getPluginHashCode(TerrainPlugin terrainPlugin)
        {
            string details = terrainPlugin.Name;
            foreach (LayerType terrainLayerPipelineType in Enum.GetValues(typeof(LayerType)))
            {
                if (terrainPlugin.Out.Contains(terrainLayerPipelineType))
                    details += Enum.GetName(typeof(LayerType), terrainLayerPipelineType);
            }
            foreach (LayerType terrainLayerPipelineType in Enum.GetValues(typeof(LayerType)))
            {
                if (terrainPlugin.In.Contains(terrainLayerPipelineType))
                    details += Enum.GetName(typeof(LayerType), terrainLayerPipelineType);
            }
            foreach (LayerType terrainLayerPipelineType in Enum.GetValues(typeof(LayerType)))
            {
                if (terrainPlugin.Not.Contains(terrainLayerPipelineType))
                    details += Enum.GetName(typeof(LayerType), terrainLayerPipelineType);
            }
            

            return details.GetHashCode();
        } 
    }
}
