namespace TerrainGeneration_PluginBase
{
    public class SketchToolEraser : SketchTool
    {
        public int Size { get; set; } = 10;

        public SketchToolEraser()
        {
            Type = SketchToolType.Eraser;
        }
    }
}
