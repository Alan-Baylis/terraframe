namespace TerrainGeneration_PluginBase
{
    public class TerrainOutputImage : TerrainOutput
    {
        public string ImagePath { get; set; } = "";
        public int[][] ImageData { get; set; }

        public TerrainOutputImage()
        {
            Type = OutputType.Image;
        }

        public TerrainOutputImage(string title)
        {
            Title = title;
            Type = OutputType.Image;
        }
    }
}
