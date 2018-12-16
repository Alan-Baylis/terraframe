namespace TerrainGeneration_PluginBase
{
    public enum SketchToolType
    {
        Brush,
        Circle,
        Circunference,
        Eraser
    }
    public class SketchTool
    {
        public string Title { get; set; } = "";
        public SketchToolType Type { get; internal set; }
    }
}
