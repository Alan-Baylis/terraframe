using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerrainGeneration_PluginBase
{
    public enum CellType
    {
        Grass,
        Rock,
        Snow
    }

    public class GeoPoint
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int LateralSpace { get; set; } = 0;
        public float MaxHeightDifference { get; set; } = 0;
        public float DistanceToRidges { get; set; } = -1;
        public bool ValidCell { get; set; } = true;
        public bool Ridge { get; set; } = false;
        public float Height { get; set; } = 0;
        public float NearestRidgeHeight { get; set; } = 0;
        public GeoPoint NearestRidge { get; set; } = null;
        public CellType Type { get; set; } = CellType.Grass;
        public int VertexIdx { get; set; } = 1;
        public float[] uv { get; set; }
        public Dictionary<string, List<float[]>> uvMap { get; set; }
        public int currentUv { get; set; }

        public GeoPoint() {}

        public GeoPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public GeoPoint(int x, int y, int lateralSpace, float maxHeightDifference)
        {
            this.X = x;
            this.Y = y;
            this.LateralSpace = lateralSpace;
            this.MaxHeightDifference = maxHeightDifference;
        }

        public static List<GeoPoint> getAdjacentCells(GeoPoint[][] geoPoints, GeoPoint geoPoint)
        {
            List<GeoPoint> adjCells = new List<GeoPoint>();

            if (geoPoint.X > 0 && geoPoint.Y > 0)
                adjCells.Add(geoPoints[geoPoint.X - 1][geoPoint.Y - 1]);
            if (geoPoint.Y > 0)
                adjCells.Add(geoPoints[geoPoint.X][geoPoint.Y - 1]);
            if (geoPoint.X < geoPoints.Length - 1 && geoPoint.Y > 0)
                adjCells.Add(geoPoints[geoPoint.X + 1][geoPoint.Y - 1]);
            if (geoPoint.X < geoPoints.Length - 1)
                adjCells.Add(geoPoints[geoPoint.X + 1][geoPoint.Y]);
            if (geoPoint.X < geoPoints.Length - 1 && geoPoint.Y < geoPoints.Length - 1)
                adjCells.Add(geoPoints[geoPoint.X + 1][geoPoint.Y + 1]);
            if (geoPoint.Y < geoPoints.Length - 1)
                adjCells.Add(geoPoints[geoPoint.X][geoPoint.Y + 1]);
            if (geoPoint.X > 0 && geoPoint.Y < geoPoints.Length - 1)
                adjCells.Add(geoPoints[geoPoint.X - 1][geoPoint.Y + 1]);
            if (geoPoint.X > 0)
                adjCells.Add(geoPoints[geoPoint.X - 1][geoPoint.Y]);

            return adjCells;
        }
    }
}
