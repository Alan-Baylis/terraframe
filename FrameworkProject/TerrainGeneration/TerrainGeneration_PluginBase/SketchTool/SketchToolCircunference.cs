namespace TerrainGeneration_PluginBase
{
    public class SketchToolCircunference : SketchTool
    {
        public string Color { get; set; } = "#000000ff";
        public int Size { get; set; } = 10;

        public SketchToolCircunference()
        {
            Type = SketchToolType.Circunference;
        }
    }
}
