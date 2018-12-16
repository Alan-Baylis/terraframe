using System;
using TerrainGeneration_PluginBase;

namespace TerrainGeneration_Core
{
    public class TerrainInputValueSketch : TerrainInputValue
    {
        public int[][] Value { get; set; }

        override public void SetValue(TerrainInput terrainInput)
        {
            Value = ((TerrainInputSketch)terrainInput).Value;
        }

        override public void FillValue(TerrainInput terrainInput)
        {
            ((TerrainInputSketch)terrainInput).Value = Value;
        }
    }
}
