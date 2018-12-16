namespace TerrainGeneration_PluginBase
{
    public enum InputType
    {
        Form,
        Sketch,
        Group
    }

    public abstract class TerrainInput
    {
        public string Title { get; set; } = "";
        public string Key { get; set; } = "";
        public InputType Type { get; internal set; }
    }
}
