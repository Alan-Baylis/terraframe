namespace TerrainGeneration_PluginBase
{
    public enum FormFieldType
    {
        Text,
        Number,
        Integer,
        Check,
        Options
    }

    public class FormField
    {
        public string Title { get; set; } = "";
        public string Key { get; set; } = "";
        public FormFieldType Type { get; internal set; }
    }
}
