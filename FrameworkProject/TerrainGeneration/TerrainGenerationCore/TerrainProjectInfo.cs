using System.Collections.Generic;
using TerrainGeneration_PluginBase;

namespace TerrainGeneration_Core
{
    public class TerrainProjectInfo
    {
        public string Name { get; set; }
        public long CreationDate { get; set; }
        public long LastTimeUsed { get; set; }
        public string ThumbnailPath { get; set; }
        public List<LayerType> ConfiguredLayers { get; set; }
        public List<LayerType> ProcessedLayers { get; set; }
    }
}
