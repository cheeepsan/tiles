using System;
using System.Collections.Generic;
using BuildingNS;
using Common.Enum;
using Common.Interface;
using Game;
using ModestTree;
using ResourceNS;
using ResourceNS.Enum;
using SaveStateNS;
using Signals.Building;
using Signals.ResourceNS;
using Signals.UI;
using Ui.Common;
using UnityEngine;
using UnityEngine.UIElements;
using Util;
using Zenject;

struct CurrentTimeViewContainer
{
    public int currentTick;
    public string currentMonthName;
    public int currentMonthIndex;
}

struct CurrentBuildingViewContainer
{
    public string id;
    public GameEntityType type;
}

public class UiRenderer : MonoBehaviour
{
    [Inject] private readonly Configuration _configuration;
    [Inject] private readonly UiSignals _uiSignalBus;
    [Inject] private readonly BuildingSignals _buildingSignalBus;
    [Inject] private readonly BuildingManager _buildingManager;
    [Inject] private readonly ResourceManager _resourceManager;
    [Inject] private readonly SaveState _saveState;
    [Inject] private readonly TimeManager _timeManager;

    private Dictionary<int, Button> buildingButtonDict;

    private Label _infoWindow;
    private Label _resourcesWindow;
    private Label _timeWindow;
    private Label _buildingViewWindow;

    private CurrentTimeViewContainer _currentTime;
    private CurrentBuildingViewContainer? _currentBuilding;

    void Start()
    {
        buildingButtonDict = new Dictionary<int, Button>();
        
        if (_configuration != null)
        {
            UIDocument document = EnsureDocument();
            InitUI(document);
        }
        
        SubscribeToSignals();
    }

    void OnEnable()
    {

    }

    private UIDocument EnsureDocument()
    {
        var document = GetComponent<UIDocument>();
        if (document == null)
        {
            document = gameObject.AddComponent<UIDocument>();
        }

        if (document.panelSettings == null)
        {
            document.panelSettings = Resources.Load<PanelSettings>("UI/toolkit/PanelSettings");
        }

        if (document.visualTreeAsset == null)
        {
            //Debug.Log("Loading visualTreeAsset: " + document.visualTreeAsset);
            //Debug.Log("document" + document);
            document.visualTreeAsset = Resources.Load<VisualTreeAsset>("UI/toolkit/panel");
        }

        return document;
    }

    private void InitUI(UIDocument document)
    {
        
        VisualElement root = document != null ? document.rootVisualElement : null;
        if (root == null)
        {
            Debug.LogError("UI Toolkit HUD failed to load. Check Resources/UI/toolkit/panel.uxml and PanelSettings.");
            return;
        }

        StyleSheet hudStyles = Resources.Load<StyleSheet>("UI/toolkit/panel");
        if (hudStyles != null && !root.styleSheets.Contains(hudStyles))
        {
            root.styleSheets.Add(hudStyles);
        }

        VisualElement buildingButtonPanel = root.Q<VisualElement>("BuildingButtonPanel");
        foreach (CfgBuilding currentBuilding in _configuration.GetCfgBuildingList())
        {
            var capturedBuilding = currentBuilding;
            var button = new Button
            {
                text = capturedBuilding.name,
                name = $"BuildingButton-{capturedBuilding.id}"
            };
            button.AddToClassList("hud-button");
            button.SetEnabled(capturedBuilding.prerequisite.IsEmpty());
            button.clicked += () =>
            {
                _uiSignalBus.FireBuildingButtonEvent(new BuildingButtonClickedSignal
                    { buildingConf = capturedBuilding });
            };

            buildingButtonDict.Add(capturedBuilding.id, button);
            buildingButtonPanel.Add(button);
        }

        root.Q<Button>("SaveButton").clicked += () => _saveState.SaveStateToJson();
        root.Q<Button>("LoadButton").clicked += () => _saveState.LoadStateFromJson();
        root.Q<Button>("PauseButton").clicked += () => _timeManager.TogglePause();

        _infoWindow = root.Q<Label>("BuildingInfoLabel");
        _resourcesWindow = root.Q<Label>("ResourcesLabel");
        _timeWindow = root.Q<Label>("TimeLabel");
        _buildingViewWindow = root.Q<Label>("BuildingViewLabel");
    }

    /*
     * TODO:
     *  This is a dangerous event, since it's being triggered by same action as SubscribeOnBuildingPlacedEvent in
     *  BuildingManager. Probably because UIRenderer is created later then BuildingManager it retrieves updated data
     *  from BuildingManager. This should be called AFTER SubscribeOnBuildingPlacedEvent in controllable manner. Should
     *  be refactored later
     */
    private void UpdateInfoWindow()
    {
        List<int> builtBuildings = _buildingManager.GetBuiltUniqueBuildingList();

        foreach (var cfgBuilding in _configuration.GetCfgBuildingList())
        {
            bool prerequisiteComplete = cfgBuilding.prerequisite.TrueForAll(x => builtBuildings.Contains(x));
            Button btn = buildingButtonDict[cfgBuilding.id];
            if (prerequisiteComplete && !btn.enabledSelf)
            {
                btn.SetEnabled(true);
            }
        }

        string buildingManagerStatus = _buildingManager.GetStateInfo();
        _infoWindow.text = buildingManagerStatus;
    }

    private void UpdateResourcesWindow(Dictionary<ResourceType, float> resources)
    {
        string data = "";
        foreach (var resource in resources)
        {
            data += $"{resource.Key}: {resource.Value}\n";
        }

        _resourcesWindow.text = data;
    }

    private void UpdateTimeWindow(int tick, int month, string monthName)
    {
        _timeWindow.text = $"Tick: {tick}, month {monthName}";
    }

    private void UpdateBuildingViewInfoWindow(UiBuildingInfo buildingInfo)
    {
        if (!_currentBuilding.HasValue || _currentBuilding.HasValue && _currentBuilding.Value.id != buildingInfo.id)
        {
            _currentBuilding = new CurrentBuildingViewContainer()
            {
                id = buildingInfo.id,
                type = buildingInfo.type
            };
        }

        string data = $"{buildingInfo.name}\n";

        switch (buildingInfo.type)
        {
            case GameEntityType.Building:
                data += $"{buildingInfo.workerInfo}\n";
                break;
            case GameEntityType.Resource:
                data += $"{buildingInfo.resourceInfo}\n";
                break;
            default:
                break;
        }

        _buildingViewWindow.text = data;
    }

    private void SubscribeToSignals()
    {
        SubscribeToBuildingSignals();
        SubscribeToResourceSignals();
        SubscribeToOnTick();
        SubscribeToBuildingViewSignals();
    }

    private void SubscribeToResourceSignals()
    {
        _uiSignalBus.Subscribe<UpdateResourcesViewSignal>
            ((x) => { UpdateResourcesWindow(x.resources); });
    }

    private void SubscribeToBuildingSignals()
    {
        Action onBuildingSignal = UpdateInfoWindow;
        _buildingSignalBus.Subscribe<BuildingPlacedSignal>(onBuildingSignal);
    }

    private void SubscribeToBuildingViewSignals()
    {
        _uiSignalBus.Subscribe<BuildingInfoViewSignal>(signal => UpdateBuildingViewInfoWindow(signal.buildingInfo));
    }

    // TODO this should be on demand, not on tick
    private void UpdateBuildingInfoOnTick()
    {
        bool buildingIsSet = _currentBuilding.HasValue;
        if (buildingIsSet)
        {
            CurrentBuildingViewContainer building = _currentBuilding.Value;
            bool entityExists = false;
            UiBuildingInfo buildingInfo = null;
            switch (building.type)
            {
                case GameEntityType.Building:
                    entityExists = _buildingManager.GetAllBuildings().TryGetValue(building.id, out PlaceableBuilding foundBuilding);
                    if (entityExists && foundBuilding is IViewableInfo)
                    {
                        buildingInfo = ((IViewableInfo)foundBuilding).CreateUiBuildingInfo();
                    }

                    break;
                case GameEntityType.Resource:
                    entityExists = _resourceManager.GetAllResources().TryGetValue(building.id, out Resource foundResource);
                    if (entityExists && foundResource is IViewableInfo)
                    {
                        buildingInfo = ((IViewableInfo)foundResource).CreateUiBuildingInfo();
                    }
                    break;
            }

            if (buildingInfo != null)
            {
                UpdateBuildingViewInfoWindow(buildingInfo);
            }
        }
    }

    private void SubscribeToOnTick()
    {
        TimeManager.OnTick += delegate(object sender, TimeManager.OnTickEventArgs args)
        {
            int tick = args.currentTick;
            int month = args.month;
            string monthName = args.monthName;

            UpdateBuildingInfoOnTick();
            UpdateTimeWindow(tick, month, monthName);
        };
    }
}
