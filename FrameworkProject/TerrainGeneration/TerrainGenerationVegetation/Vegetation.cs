//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace TerrainGeneration_Vegetation
//{
//    class Vegetation
//    {

//        public static string createProducton(string variables, string constants, string axiom, Dictionary<char, string> originDestinationMap, int iterations)
//        {
//            return iterateProduction(axiom, originDestinationMap, iterations);
//        }

//        private static string iterateProduction(string axiom, Dictionary<char, string> originDestinationMap, int iteration)
//        {
//            StringBuilder stringBuilder = new StringBuilder();
//            foreach(char c in axiom)
//            {
//                if (originDestinationMap.ContainsKey(c))
//                    stringBuilder.Append(originDestinationMap[c]);
//                else
//                    stringBuilder.Append(c);
//            }

//            iteration--;
//            if(iteration > 0)
//            {
//                return iterateProduction(stringBuilder.ToString(), originDestinationMap, iteration);
//            }
//            return stringBuilder.ToString();
//        }
//    }
//}
