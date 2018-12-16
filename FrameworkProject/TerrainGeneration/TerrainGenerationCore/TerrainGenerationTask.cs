using System;
using System.Collections.Generic;
using System.ComponentModel;
using TerrainGeneration_PluginBase;

namespace TerrainGeneration_Core
{
    public class TerrainGenerationTask
    {
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        public event EventHandler TaskFinished;
        private HashSet<TerrainPipelineLayer> pipelineLayersToProcess = new HashSet<TerrainPipelineLayer>(new TerrainPipelineLayerComparator());
        private TerrainProject terrainProject;

        public TerrainGenerationTask()
        {
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Cancelled)
                Console.Out.WriteLine("Cancelled");
            else
                Console.Out.WriteLine("Complete");

            OnTaskFinished();

            if (e.Error != null)
                throw (e.Error);
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Console.Out.WriteLine("Working 1");
            Dictionary<TerrainPlugin, TerrainPipelineLayer> pluginsToProcess = new Dictionary<TerrainPlugin, TerrainPipelineLayer>();

            Dictionary<TerrainPipelineLayer, TerrainPlugin> terrainPluginMap = terrainProject.PipelinePluginMap;
            Console.Out.WriteLine("Working 1.5");
            foreach (TerrainPipelineLayer terrainPipelieLayer in pipelineLayersToProcess)
            {
                pluginsToProcess.Add(terrainPluginMap[terrainPipelieLayer], terrainPipelieLayer);
            }
            Console.Out.WriteLine("Working 2");
            List<TerrainOutput> previousTerrainOutputs = new List<TerrainOutput>();
            bool somethingProcessed = false;
            foreach (TerrainPlugin terrainPlugin in terrainProject.TerrainPipeline)
            {
                Console.Out.WriteLine("Working 3");
                if (terrainPlugin != null)
                {
                    Console.Out.WriteLine("Working 4");
                    if (pluginsToProcess.ContainsKey(terrainPlugin))
                    {
                        Console.Out.WriteLine("Working 5");
                        List<TerrainOutput> newTerrainOutputs = terrainPlugin.StartProcess(terrainProject.TerrainInputs[pluginsToProcess[terrainPlugin]], terrainProject.WorkflowData);
                        somethingProcessed = true;
                        previousTerrainOutputs = newTerrainOutputs;
                        Console.Out.WriteLine("Working 6");
                        try
                        {
                            if (terrainProject.TerrainOutputs.ContainsKey(pluginsToProcess[terrainPlugin]))
                                terrainProject.TerrainOutputs[pluginsToProcess[terrainPlugin]] = newTerrainOutputs;
                            else
                                terrainProject.TerrainOutputs.Add(pluginsToProcess[terrainPlugin], newTerrainOutputs);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                        }
                    }
                    else
                    {
                        TerrainPipelineLayer terrainPipelineLayer = new TerrainPipelineLayer();
                        terrainPipelineLayer.LayerCode = TerrainPipelineLayer.ToPipelineLayerCode(terrainPlugin.Out, terrainPlugin.In, terrainPlugin.Not);

                        if (terrainProject.TerrainOutputs.ContainsKey(terrainPipelineLayer))
                        {
                            if (somethingProcessed)
                            {
                                previousTerrainOutputs = new List<TerrainOutput>();
                                terrainProject.TerrainOutputs[terrainPipelineLayer] = new List<TerrainOutput>();
                            }
                            else
                            {
                                previousTerrainOutputs = terrainProject.TerrainOutputs[terrainPipelineLayer];
                            }
                        }
                    }
                }
            }
            Console.Out.WriteLine("Saving");
            terrainProject.Save();

            Console.Out.WriteLine("Working End");
        }

        internal void StartTask(TerrainProject terrainProject)
        {
            Console.Out.WriteLine("Starting");
            this.terrainProject = terrainProject;
            //backgroundWorker.RunWorkerAsync();
            BackgroundWorker_DoWork(null, null);
            Console.Out.WriteLine("Complete");
            OnTaskFinished();
        }

        protected virtual void OnTaskFinished()
        {
            EventHandler handler = TaskFinished;
            if (handler != null)
            {
                EventArgs eventArgs = new EventArgs();
                handler(this, eventArgs);
            }
        }

        public void AddLayerToProcess(TerrainPipelineLayer terrainPipelineLayer)
        {
            pipelineLayersToProcess.Add(terrainPipelineLayer);
        }
    }
}
