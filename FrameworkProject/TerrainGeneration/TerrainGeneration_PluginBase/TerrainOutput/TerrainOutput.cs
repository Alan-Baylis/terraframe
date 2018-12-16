using System.Xml.Serialization;

namespace TerrainGeneration_PluginBase
{
    public enum OutputType
    {
        Values,
        Image,
        Mesh,
        Group,
        Object
    }

    [XmlInclude(typeof(TerrainOutputGroup)), XmlInclude(typeof(TerrainOutputImage)), XmlInclude(typeof(TerrainOutputMesh)), XmlInclude(typeof(TerrainOutputValues)), XmlInclude(typeof(TerrainOutputObject))]
    public abstract class TerrainOutput
    {
        public string Title { get; set; } = "";
        public string Key { get; set; } = "";
        public OutputType Type { get; internal set; }
    }
}
