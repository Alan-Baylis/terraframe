using System.Collections.Generic;

namespace TerrainGeneration_PluginBase
{
    public class TerrainOutputGroup : TerrainOutput
    {
        public List<TerrainOutput> ChildOutputs { get; set; } = new List<TerrainOutput>();

        public TerrainOutputGroup()
        {
            Type = OutputType.Group;
        }

        public TerrainOutputGroup(string title)
        {
            Title = title;
            Type = OutputType.Group;
        }
    }
}
