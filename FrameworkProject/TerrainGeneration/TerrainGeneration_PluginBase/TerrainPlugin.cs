using System.Collections.Generic;

namespace TerrainGeneration_PluginBase
{
    public enum LayerType
    {
        HeightMap = 1,
        WaterBodies = 2,
        Vegetation = 4,
        Roads = 8,
        Urban = 16
    }

    public interface TerrainPlugin
    {
        string Name { get; }
        string Description { get; }
        string Author { get; }
        HashSet<LayerType> Out { get; }
        HashSet<LayerType> In { get; }
        HashSet<LayerType> Not { get; }
        int Progress { get; }
        List<TerrainInput> TerrainInputs { get; }
        List<TerrainOutput> StartProcess(List<TerrainInput> terrainInputs, Dictionary<LayerType, WorkflowData.WorkflowData> workflowData);
    }
}
