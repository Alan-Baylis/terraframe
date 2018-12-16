
using System.Collections.Generic;

namespace TerrainGeneration_PluginBase
{
    public class TerrainInputGroup : TerrainInput
    {
        public List<TerrainInput> ChildInputs { get; set; } = new List<TerrainInput>();

        public TerrainInputGroup()
        {
            Type = InputType.Group;
        }

        public TerrainInputGroup(string title)
        {
            Type = InputType.Group;
            Title = title;
        }
    }
}
