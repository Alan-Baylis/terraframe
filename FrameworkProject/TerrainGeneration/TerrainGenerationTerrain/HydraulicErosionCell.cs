using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerrainGeneration_HeightMap
{
    class HydraulicErosionCell
    {
        public float Altitude { get; set; } = 0;
        public float Water { get; set; } = 0;
        public float Sediment { get; set; } = 0;

        public HydraulicErosionCell getCopy()
        {
            HydraulicErosionCell copy = new HydraulicErosionCell();
            copy.Altitude = Altitude;
            copy.Water = Water;
            copy.Sediment = Sediment;
            return copy;
        }
    }
}
