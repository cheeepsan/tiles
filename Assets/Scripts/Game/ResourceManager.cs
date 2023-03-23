using System;

using System.Collections.Generic;
using System.Linq;

using BuildingNS;
using Common;
using GridNS;
using ResourceNS;
using ResourceNS.Enum;
using Signals.ResourceNS;
using Signals.UI;
using UnityEngine;
using Util;
using Zenject;


namespace Game
{
    /// <summary>
    ///  Keep track of available resource spots: fruits, available farms etc...
    /// </summary>
    public class ResourceManager
    {
        [Inject] private readonly UiSignals _uiSignals;
        [Inject] private Configuration _configuration;
        [Inject] private FarmPlotFactory _farmPlotFactory;
        [Inject] private GroundTileset _groundTileset;

        private readonly ResourceSignals _resourceSignals;

        private Dictionary<String, Resource> _resources;
        private Dictionary<String, PlaceableBuilding> _buildings;

        private Dictionary<ResourceType, float> _accumulatedResources;
        private Queue<Tuple<ResourceType, float>> _storingQueue;

        private Dictionary<String, Transform> _farmPlots;

        public ResourceManager(ResourceSignals resourceSignals)
        {
            _resourceSignals = resourceSignals;
            _resources = new Dictionary<String, Resource>();
            _buildings = new Dictionary<String, PlaceableBuilding>();
            _accumulatedResources = new Dictionary<ResourceType, float>();
            _storingQueue = new Queue<Tuple<ResourceType, float>>(); // TODO: Is it even needed. Is it even smart
            _farmPlots = new Dictionary<String, Transform>();

            TimeManager.On10Tick += delegate(object sender, TimeManager.On10TickEventArgs args)
            {
                PingAvailableResources();
                DequeueResourceQueue();
            };
            
            TimeManager.OnMonthChange += delegate(object sender, TimeManager.OnMonthChangeEventArgs args)
            {

                YearlyCycle(args);
            };

            SubscribeToSignals();
        }

        private void YearlyCycle(TimeManager.OnMonthChangeEventArgs args)
        {
            /*
             * 0 - 3 flood
             * 4 - 7 field works
             * 8 - 11 gather
             */

           //var list = new List<Transform>();
           //foreach (var pair in _farmPlots)
           //{
           //    // transform of transform
           //    list.Add(pair.Value.parent.transform);

           //}

           //ConcaveAlgo a = new ConcaveAlgo(list);
           //List<MeshData> meshes = a.CalculateMeshes();
           //foreach (var meshData in meshes)
           //{
           //    Mesh m = new Mesh
           //    {
           //        name = "test mesh " + meshData.name
           //    };

           //    GameObject gb = (GameObject)Resources.Load("Meshes/TideMesh");

           //    var inst = GameObject.Instantiate(gb);
           //    inst.name = meshData.name;
           //
           //
           //    m.vertices = meshData.triangleVertices;
           //    m.triangles = meshData.triagnles;
           //    m.normals = meshData.normals;
           //    m.tangents = meshData.tangents;
           //    m.uv = meshData.uuvs;
           //    var center = meshData.centroid;
           //    inst.transform.position = new Vector3((float)center.X, 0.66f, (float)center.Y);
           //    m.RecalculateNormals();
           //    inst.GetComponent<MeshFilter>().sharedMesh = m;
           //    m.RecalculateBounds();
           //}
            
            List<int> floodMonth = new List<int>() { 0, 1, 2 };

            int month = args.month;
            if (floodMonth.Contains(month))
            {
                // flood
                if (month == 0)
                {
                }
            }

            if (month == 4)
            {
                // remove flood
                // create farms
                CreateFarms();
            }
        }

        private void CreateFarms()
        {
            // TODO clean up. Farms is id 
            CfgBuilding farm = _configuration.GetCfgWorldObjectsList().Find(x => x.id == 1);
            GameObject fromResources = (GameObject)Resources.Load(farm.path);

            int totalFarms = _buildings.Values.Count(x => x.preferredResource == ResourceType.Farm);
            
            foreach (var key in _farmPlots.Take(totalFarms))
            {
                var t = key.Value.transform;

                var farmPlot = _farmPlotFactory.Create(fromResources, t);
                farmPlot.transform.position = new Vector3(t.position.x, t.position.y + 0.5f, t.position.z);

            }
        }
        // TODO move to stockpile manager
        private void AddResourceToQueue(Tuple<ResourceType, float> r)
        {
            _storingQueue.Enqueue(r);
        }

        // TODO: Resource add/removal should be synced. Maybe removal prio 1, addition may be skipped?
        private void DequeueResourceQueue()
        {
            bool dataInQueue = _storingQueue.Count > 0;
            if (dataInQueue)
            {
                while (_storingQueue.Count > 0)
                {
                    Tuple<ResourceType, float> element = _storingQueue.Dequeue();
                    if (_accumulatedResources.ContainsKey(element.Item1))
                    {
                        if (_accumulatedResources.TryGetValue(element.Item1, out float resourceValue))
                        {
                            _accumulatedResources[element.Item1] = resourceValue + element.Item2;
                        }
                    }
                    else
                    {
                        _accumulatedResources.Add(element.Item1, element.Item2);
                    }
                }

                _uiSignals.FireUpdateResourcesViewSignal(new UpdateResourcesViewSignal()
                    { resources = _accumulatedResources });
            }
        }

        private void PingAvailableResources()
        {
            //Debug.Log("Polling data for resource manager");
            List<Resource> availableResources = new List<Resource>();
            List<PlaceableBuilding> availableBuildings = new List<PlaceableBuilding>();

            foreach (var b in _buildings.Values)
            {
                if (b.IsAvailable())
                {
                    availableBuildings.Add(b);
                }
            }

            foreach (var r in _resources.Values)
            {
                if (r.IsAvailable())
                {
                    availableResources.Add(r);
                }
            }

            if (availableResources.Count > 0 && availableBuildings.Count > 0)
            {
                // todo, sort building by priorities
                // todo, get available resources by preferred type
                // todo, resolve buildings and resources in batch 

                PlaceableBuilding building = availableBuildings.First();
                Resource resource = availableResources.First();
                building.SetCurrentResource(resource);
            }

            availableResources.Clear();
            availableBuildings.Clear();
        }
        
        private void AddFarmPlot(Transform farmPlotCoord)
        {
            String farmPlotGuid = _groundTileset.GetGroundTileByPosition(farmPlotCoord.position);
            
            if (!_farmPlots.ContainsKey(farmPlotGuid))
            {
                _farmPlots.Add(farmPlotGuid, farmPlotCoord);
            }
        }

        private void SubscribeToSignals()
        {
            SubscribeToResourceAvailableSignal();
            SubscribeToBuildingRegistered();
            SubscribeToAddResourceToQueue();
            SubscribeToAddAvailableFarmPlot();
        }

        private void SubscribeToBuildingRegistered()
        {
            _resourceSignals.Subscribe<RegisterBuildingSignal>
                ((x) => { _buildings.Add(x.sender.GetId(), x.sender); });
        }

        private void SubscribeToAddResourceToQueue()
        {
            _resourceSignals.Subscribe<AddResourceToQueueSignal>
                ((x) => { AddResourceToQueue(x.resource); });
        }

        private void SubscribeToAddAvailableFarmPlot()
        {
            _resourceSignals.Subscribe<AddAvailableFarmPlotSignal>
                ((x) => { AddFarmPlot(x.farmPlotTransform); });
        }

        private void SubscribeToResourceAvailableSignal()
        {
            Action<ResourceAvailableSignal, Dictionary<String, Resource>> ResourceAvailable = (s, d) =>
            {
                d.Add(s.resourceId, s.resource);
            };

            _resourceSignals.Subscribe2<ResourceAvailableSignal, Dictionary<String, Resource>>((x, b) =>
                    ResourceAvailable(x, b)
                , _resources);
        }

        public Dictionary<ResourceType, float> GetAllResources()
        {
            return _accumulatedResources;
        }

        public void SetAllResources(Dictionary<ResourceType, float> resource)
        {
            _accumulatedResources = resource;
            _uiSignals.FireUpdateResourcesViewSignal(new UpdateResourcesViewSignal()
                { resources = _accumulatedResources });
        }
    }
}