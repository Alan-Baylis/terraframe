using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGeneration_PluginBase;

namespace TerrainGeneration_HeightMap
{
    class RidgeParticle
    {
        private double X { get; set; }
        private double Y { get; set; }
        private double ForceAngle { get; set; } = 0;
        private double Height { get; set; }
        private float?[][] HeightMap { get; set; }
        private HashSet<String> CreatedCoords { get; set; } = new HashSet<String>();
        private float GaussionDistance { get; set; } = 30;
        private double LastGaussionX { get; set; }
        private double LastGaussionY { get; set; }
        private List<Double> GaussionXs { get; set; } = new List<Double>();
        private List<Double> GaussionYs { get; set; } = new List<Double>();
        private List<Double> GaussionAngle { get; set; } = new List<Double>();
        private List<GeoPoint> Ridge { get; set; } = new List<GeoPoint>();

        public RidgeParticle(float?[][] heightMap, float x, float y, float forceAngle, float height, List<GeoPoint> ridge)
        {
            HeightMap = heightMap;

            X = x;
            Y = y;
            ForceAngle = forceAngle;
            Height = height;
            String coordSignature = "" + (int)X + "#" + (int)Y;
            CreatedCoords.Add(coordSignature);

            heightMap[(int)x][(int)y] = height;

            GaussionXs.Add(X);
            GaussionYs.Add(Y);
            GaussionAngle.Add(ForceAngle);

            Ridge = ridge;
        }

        public bool processParticle()
        {
            ForceAngle = ForceAngle + (0.5f - RidgeHelpers.random.NextDouble()) * (Math.PI / 10);
            double x = Math.Cos(ForceAngle);
            double y = Math.Sin(ForceAngle);

            X = X + x;
            Y = Y + y;

            Height = Height - RidgeHelpers.random.NextDouble() * 0.005f;

            String coordSignature = "" + (int)X + "#" + (int)Y;

            if (X < 0 || X >= HeightMap.Length || Y < 0 || Y >= HeightMap.Length || Height < 0 || (!CreatedCoords.Contains(coordSignature) && HeightMap[(int)X][(int)Y] != null))
            {
                return false;
            }

            if (HeightMap[(int)X][(int)Y] != null && HeightMap[(int)X][(int)Y] != 0)
            {
                GeoPoint geoPoint = new GeoPoint();
                geoPoint.X = (int)X;
                geoPoint.Y = (int)Y;
                geoPoint.Height = (float)Height;
                Ridge.Add(geoPoint);
            }
            HeightMap[(int)X][(int)Y] = (float)Height;

            float distanceToLastGaussion = (float)Math.Sqrt(Math.Pow(LastGaussionX - X, 2) + Math.Pow(LastGaussionY - Y, 2));

            if (distanceToLastGaussion >= GaussionDistance)
            {
                GaussionXs.Add(X);
                GaussionYs.Add(Y);
                GaussionAngle.Add(ForceAngle);

                LastGaussionX = X;
                LastGaussionY = Y;
            }

            CreatedCoords.Add(coordSignature);

            return true;
        }

        public void createGaussions()
        {
            for (int i = 0; i < GaussionAngle.Count; i++)
            {
                calculateCurve(GaussionXs[i], GaussionYs[i], GaussionAngle[i]);
            }
        }

        private void calculateCurve(double initialX, double initialY, double forceAngle)
        {
            double forceAngleA = forceAngle - Math.PI / 2.0f;
            double forceAngleB = forceAngle + Math.PI / 2.0f;
        
            double curX = initialX;
            double curY = initialY;

            double x = Math.Cos(forceAngleA);
            double y = Math.Sin(forceAngleA);

            float de = (float)((1.0 / 8.0) * HeightMap.Length);

            curX += x;
            curY += y;

            float distanceFromCenter = (float)Math.Sqrt(Math.Pow((int)initialX - (int)curX, 2) + Math.Pow((int)initialY - (int)curY, 2));
            float currentHeight = normalDistribution(distanceFromCenter, de, (float)HeightMap[(int)initialX][(int)initialY]);

            while (distanceFromCenter < de && (((int)curX == (int)initialX && (int)curY == (int)initialY) || (curX >= 0 && curX < HeightMap.Length && curY >= 0 && curY < HeightMap.Length && (HeightMap[(int)curX][(int)curY] == null || HeightMap[(int)curX][(int)curY] < currentHeight))))
            {

                if (!((int)curX == (int)initialX && (int)curY == (int)initialY))
                    HeightMap[(int)curX][(int)curY] = currentHeight;

                curX += x;
                curY += y;

                distanceFromCenter = (float)Math.Sqrt(Math.Pow((int)initialX - (int)curX, 2) + Math.Pow((int)initialY - (int)curY, 2));
                currentHeight = normalDistribution(distanceFromCenter, de, (float)HeightMap[(int)initialX][(int)initialY]);
            }

            curX = initialX;
            curY = initialY;

            x = Math.Cos(forceAngleB);
            y = Math.Sin(forceAngleB);

            curX += x;
            curY += y;

            distanceFromCenter = (float)Math.Sqrt(Math.Pow((int)initialX - (int)curX, 2) + Math.Pow((int)initialY - (int)curY, 2));
            currentHeight = normalDistribution(distanceFromCenter, de, (float)HeightMap[(int)initialX][(int)initialY]);

            while (distanceFromCenter < de && (((int)curX == (int)initialX && (int)curY == (int)initialY) || (curX >= 0 && curX < HeightMap.Length && curY >= 0 && curY < HeightMap.Length && (HeightMap[(int)curX][(int)curY] == null || HeightMap[(int)curX][(int)curY] < currentHeight))))
            {

                if (!((int)curX == (int)initialX && (int)curY == (int)initialY))
                    HeightMap[(int)curX][(int)curY] = currentHeight;

                curX += x;
                curY += y;

                distanceFromCenter = (float)Math.Sqrt(Math.Pow((int)initialX - (int)curX, 2) + Math.Pow((int)initialY - (int)curY, 2));
                currentHeight = normalDistribution(distanceFromCenter, de, (float)HeightMap[(int)initialX][(int)initialY]);
            }
        }

        public static float normalDistribution(float distanceFromCenter, float sd, float amp)
        {
            return (float)(amp * Math.Exp(-0.5f * Math.Pow(distanceFromCenter / sd, 2)));
        }

        public bool processParticle_()
        {

            /*float colisionX = 0.01f-RidgeAlgorithm.random.nextFloat()*0.02f;
            float colisionY = 0.01f-RidgeAlgorithm.random.nextFloat()*0.02f;

            setAx(getAx()+colisionX);
            setAy(getAy()+colisionY);

            setX((getX()+getAx()));
            setY(getY()+getAy());

            setHeight(getHeight()-RidgeAlgorithm.random.nextFloat()*0.001f);

            String coordSignature = ""+(int)getX()+"#"+(int)getY();

            if(getX()<0||getX()>=heightMap.length||getY()<0||getY()>=heightMap.length||getHeight()<0 || (!createdCoords.contains(coordSignature) && heightMap[(int)getX()][(int)getY()] != 0) ){
                return false;
            }

            heightMap[(int)getX()][(int)getY()] = getHeight();
            createdCoords.add(coordSignature);

            System.out.println("Ridge: " + getX() + ":" + getY() + ":" + getHeight());
            */
            return true;
        }
    }
}
