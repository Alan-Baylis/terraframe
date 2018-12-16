using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGeneration_PluginBase;

namespace TerrainGeneration_HeightMap
{
    class RidgeHelpers
    {
        public static int PARTICLE_COUNT = 5;

        private static List<RidgeParticle> particles;
        public static Random random;

        public static void createRidges(float?[][] heightMap, List<List<GeoPoint>> ridges, Random random, int ridgeParticles)
        {
            PARTICLE_COUNT = ridgeParticles;
            RidgeHelpers.random = random;

            createParticles(heightMap, ridges);
            processParticles();
        }

        private static void createParticles(float?[][] heightMap, List<List<GeoPoint>> ridges)
        {

            particles = new List<RidgeParticle>();
            HashSet<String> takenPositions = new HashSet<String>();

            for (int i = 0; i < PARTICLE_COUNT; i++)
            {

                List<GeoPoint> ridge = new List<GeoPoint>();

                float x = (float)random.NextDouble() * heightMap.Length;
                float y = (float)random.NextDouble() * heightMap.Length;

                while (takenPositions.Contains("" + (int)x + "#" + (int)y))
                {
                    x = (float)random.NextDouble() * heightMap.Length;
                    y = (float)random.NextDouble() * heightMap.Length;
                }

                takenPositions.Add("" + (int)x + "#" + (int)y);

                float initialAngle = (float)(random.NextDouble() * (2 * Math.PI));

                float initialHeight = 0.5f + ((float)random.NextDouble() * 0.5f);

                GeoPoint geoPoint = new GeoPoint();
                geoPoint.X = (int)x;
                geoPoint.X = (int)y;
                geoPoint.Height = initialHeight;

                ridge.Add(geoPoint);

                RidgeParticle p1 = new RidgeParticle(heightMap, x, y, initialAngle, initialHeight, ridge);
                RidgeParticle p2 = new RidgeParticle(heightMap, x, y, (float)(initialAngle - Math.PI), initialHeight, ridge);

                particles.Add(p1);
                particles.Add(p2);

                ridges.Add(ridge);
            }
        }

        private static void processParticles()
        {

            List<RidgeParticle> particlesCopy = new List<RidgeParticle>(particles);

            while (particles.Count > 0)
            {
                List<RidgeParticle> particlesToRemove = new List<RidgeParticle>();

                foreach (RidgeParticle p in particles)
                {
                    if (!p.processParticle())
                    {
                        particlesToRemove.Add(p);
                    }
                }

                particles = particles.Except(particlesToRemove).ToList();
            }

            /*for( Particle particle : particlesCopy ){
                particle.createGaussions();
            }*/
        }
    }
}
