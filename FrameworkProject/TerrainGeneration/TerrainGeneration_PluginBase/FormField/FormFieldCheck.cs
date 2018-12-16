namespace TerrainGeneration_PluginBase
{
    public class FormFieldCheck : FormField
    {
        public bool Value { get; set; } = false;

        public FormFieldCheck()
        {
            Type = FormFieldType.Check;
        } 
    }
}
