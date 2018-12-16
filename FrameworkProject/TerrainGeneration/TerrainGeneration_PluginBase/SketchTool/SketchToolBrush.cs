namespace TerrainGeneration_PluginBase
{
    public class SketchToolBrush : SketchTool
    {
        public string Color { get; set; } = "#000000ff";
        public int Width { get; set; } = 1;
        public bool VelocityBasedOpacity { get; set; } = false;

        public SketchToolBrush()
        {
            Type = SketchToolType.Brush;
        }
    }
}
