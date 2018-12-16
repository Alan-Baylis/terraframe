using System.Collections.Generic;

namespace TerrainGeneration_PluginBase
{
    public class TerrainOutputObject : TerrainOutput
    {
        public TerrainOutputObject()
        {
            Type = OutputType.Object;
        }

        public List<float[]> Object { get; set; }
    }
}
