using System.Collections.Generic;

namespace TerrainGeneration_PluginBase
{
    public class TerrainInputSketch : TerrainInput
    {
        public List<SketchTool> SketchTools { get; set; } = new List<SketchTool>();
        public string BaseColor { get; set; } = "#ffffffff";
        public int Width { get; set; } = 100;
        public int Height { get; set; } = 100;
        public int[][] Value { get; set; } 

        public TerrainInputSketch()
        {
            Type = InputType.Sketch;
        }

        public TerrainInputSketch(string title)
        {
            Title = title;
            Type = InputType.Sketch;
        }
    }
}
