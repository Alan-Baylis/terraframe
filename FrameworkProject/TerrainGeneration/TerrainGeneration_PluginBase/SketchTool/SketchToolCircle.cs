namespace TerrainGeneration_PluginBase
{
    public class SketchToolCircle : SketchTool
    {
        public string Color { get; set; } = "#000000ff";
        public int Size { get; set; } = 10;

        public SketchToolCircle()
        {
            Type = SketchToolType.Circle; 
        }
    }
}
