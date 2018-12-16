using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TerrainGeneration_PluginBase;

namespace TerrainGeneration_Core
{
    class PluginManager
    {
        public static string PLUGINS_PATH {
            get
            {
                string terraFrameFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + Path.DirectorySeparatorChar + "TerraFrame";
                Console.WriteLine(terraFrameFolder);

                if (!Directory.Exists(terraFrameFolder))
                    Directory.CreateDirectory(terraFrameFolder);

                string pluginFolder = terraFrameFolder + Path.DirectorySeparatorChar + "TerrainPlugins";

                if (!Directory.Exists(pluginFolder))
                    Directory.CreateDirectory(pluginFolder);

                return pluginFolder;
            }
        }

        internal static Dictionary<TerrainPipelineLayer, List<TerrainPlugin>> AvailablePlugins
        {
            get {
                Dictionary<TerrainPipelineLayer, List<TerrainPlugin>> availablePlugins = new Dictionary<TerrainPipelineLayer, List<TerrainPlugin>>(new TerrainPipelineLayerComparator());
                List<TerrainPlugin> plugins = readAvailablePlugins();

                foreach (TerrainPlugin plugin in plugins) {
                    TerrainPipelineLayer pipelineLayer = new TerrainPipelineLayer();
                    pipelineLayer.LayerCode = TerrainPipelineLayer.ToPipelineLayerCode(plugin.Out, plugin.In, plugin.Not);
                    if (!availablePlugins.ContainsKey(pipelineLayer))
                    {
                        availablePlugins.Add(pipelineLayer, new List<TerrainPlugin>());
                    }

                    availablePlugins[pipelineLayer].Add(plugin);
                }

                return availablePlugins;
            }
        }

        internal static List<TerrainPlugin> GetTerrainPipeline(TerrainProject terrainProject)
        {
            return getTerrainPipeline(terrainProject.Configurations);
        }

        internal static List<TerrainPlugin> GetTerrainPipeline()
        {
            Configurations configurations = Configurations.LoadConfigurations();
            return getTerrainPipeline(configurations);
        }

        private static List<TerrainPlugin> getTerrainPipeline(Configurations configurations)
        {
            List<TerrainPlugin> terrainPipeline = new List<TerrainPlugin>();
            Dictionary<TerrainPipelineLayer, List<TerrainPlugin>> availablePlugins = AvailablePlugins;

            foreach (TerrainPipelineLayer terrainPipelineLayer in configurations.Layers)
            {
                TerrainPlugin terrainPlugin = null;
                if (availablePlugins.ContainsKey(terrainPipelineLayer))
                {
                    foreach (TerrainPlugin plugin in availablePlugins[terrainPipelineLayer])
                    {
                        if (terrainPipelineLayer.PluginId == new TerrainPluginInfo(plugin).Id)
                        {
                            terrainPlugin = plugin;
                            break;
                        }
                    }
                }

                terrainPipeline.Add(terrainPlugin);
            }

            return terrainPipeline;
        }

        private static List<TerrainPlugin> readAvailablePlugins()
        {
            List<TerrainPlugin> plugins = new List<TerrainPlugin>();

            string pluginsPath = PLUGINS_PATH;
            Console.Out.WriteLine("Plugins path: " + pluginsPath);
            Console.Out.WriteLine("Loading plugins...");
            string[] dllFileNames = null;
            if (Directory.Exists(pluginsPath))
            {
                dllFileNames = Directory.GetFiles(pluginsPath, "*.dll");

                ICollection<Assembly> assemblies = new List<Assembly>(dllFileNames.Length);
                foreach (string dllFile in dllFileNames)
                {
                    AssemblyName an = AssemblyName.GetAssemblyName(dllFile);
                    Assembly assembly = Assembly.Load(an);
                    assemblies.Add(assembly);
                }

                Type pluginType = typeof(TerrainPlugin);
                ICollection<Type> pluginTypes = new List<Type>();
                foreach (Assembly assembly in assemblies)
                {
                    if (assembly != null)
                    {
                        Type[] types = assembly.GetTypes();
                        foreach (Type type in types)
                        {
                            if (type.IsInterface || type.IsAbstract)
                            {
                                continue;
                            }
                            else
                            {
                                if (type.GetInterface(pluginType.FullName) != null)
                                {
                                    pluginTypes.Add(type);
                                }
                            }
                        }
                    }
                }

                Console.Out.WriteLine("### Available plugins: ###");

                foreach (Type type in pluginTypes)
                {
                    TerrainPlugin plugin = (TerrainPlugin)Activator.CreateInstance(type);
                    plugins.Add(plugin);
                    Console.Out.WriteLine("##################");
                    Console.Out.WriteLine("Hash code: " + plugin.GetHashCode());
                    foreach(LayerType layerType in plugin.Out)
                        Console.Out.WriteLine(layerType.ToString());
                    Console.Out.WriteLine(plugin.Name);
                    Console.Out.WriteLine("##################");
                }
            }
            else
            {
                Console.Out.WriteLine("Plugin path does not exist!!!");
            }

            return plugins;
        }
    }
}
