using System;
using System.Collections;
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
using TMPro;
using Ui.Common;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Util;
using Zenject;
using Button = UnityEngine.UI.Button;

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
    [Inject] private readonly UiBuildingButtonFactory _buildingButtonFactory;
    [Inject] private readonly SaveState _saveState;
    [Inject] private readonly TimeManager _timeManager;

    [SerializeField] public GameObject parentPanel;
    [SerializeField] public GameObject buildingButtonPanel;
    [SerializeField] public GameObject buildingInfoPanel;
    [SerializeField] public GameObject resourcesInfoPanel;
    [SerializeField] public GameObject timeInfoPanel;
    [SerializeField] public GameObject menuButtonPanel;
    [SerializeField] public GameObject buildingViewPanel;

    private Dictionary<int, Button> buildingButtonDict;

    private TMP_Text _infoWindow;
    private TMP_Text _resourcesWindow;
    private TMP_Text _timeWindow;
    private TMP_Text _buildingViewWindow;
    
    private CurrentTimeViewContainer _currentTime;
    private CurrentBuildingViewContainer? _currentBuilding; 
    
    void Start()
    {
        buildingButtonDict = new Dictionary<int, Button>();
        
        if (_configuration != null)
        {
            Instantiate();
        }

        SubscribeToSignals();
    }

    public void Instantiate()
    {
        CfgUi buttonConfiguration = _configuration.GetUIConfiguration()[CfgUiElementsEnum.BuildingButton];
        GameObject buttonPrefab = (GameObject)Resources.Load(buttonConfiguration.path);
        using var enumerator = _configuration.GetCfgBuildingList().GetEnumerator();
        foreach (CfgBuilding currentBuilding in _configuration.GetCfgBuildingList())
        {
            Button button = _buildingButtonFactory.Create(buttonPrefab, buildingButtonPanel.transform);
            TMP_Text text = button.GetComponentInChildren<TMP_Text>();
            text.SetText(currentBuilding.name);
            
            buildingButtonDict.Add(currentBuilding.id, button);
            
            if (currentBuilding.prerequisite.IsEmpty())
            {
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }
            
            button.onClick.AddListener(() =>
            {
                _uiSignalBus.FireBuildingButtonEvent(new BuildingButtonClickedSignal
                    { buildingConf = currentBuilding });
            });
        }

        CfgUi menuButtonConfiguration = _configuration.GetUIConfiguration()[CfgUiElementsEnum.MenuButton];
        GameObject menuButtonPrefab = (GameObject)Resources.Load(menuButtonConfiguration.path);

        Button saveButton = _buildingButtonFactory.Create(menuButtonPrefab, menuButtonPanel.transform);
        TMP_Text saveText = saveButton.GetComponentInChildren<TMP_Text>();
        saveText.SetText("Save");
        saveButton.onClick.AddListener(() =>
        {
            _saveState.SaveStateToJson();
        });
        
        Button loadButton = _buildingButtonFactory.Create(menuButtonPrefab, menuButtonPanel.transform);
        TMP_Text loadText = loadButton.GetComponentInChildren<TMP_Text>();
        loadText.SetText("Load");
        loadButton.onClick.AddListener(() =>
        {
            _saveState.LoadStateFromJson();
        });
        
        Button pauseButton = _buildingButtonFactory.Create(menuButtonPrefab, menuButtonPanel.transform);
        TMP_Text pauseText = pauseButton.GetComponentInChildren<TMP_Text>();
        pauseText.SetText("Pause");
        pauseButton.onClick.AddListener(() =>
        {
            _timeManager.TogglePause();
        });
        
        
        _infoWindow = buildingInfoPanel.GetComponentInChildren<TMP_Text>();
        _infoWindow.SetText("Window ready: buildings");
        
        _resourcesWindow = resourcesInfoPanel.GetComponentInChildren<TMP_Text>();
        _resourcesWindow.SetText("Window ready: resources");
        
        _timeWindow = timeInfoPanel.GetComponentInChildren<TMP_Text>();
        _timeWindow.SetText("Window ready: time");

        _buildingViewWindow = buildingViewPanel.GetComponentInChildren<TMP_Text>();
        _buildingViewWindow.SetText("Window info ready");
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
            if (prerequisiteComplete && !btn.IsInteractable())
            {
                btn.interactable = true;
            }
        }
        
        string buildingManagerStatus = _buildingManager.GetStateInfo();
        _infoWindow.SetText(buildingManagerStatus);
    }

    private void UpdateResourcesWindow(Dictionary<ResourceType, float> resources)
    {
        string data = "";
        foreach (var resource in resources)
        {
            data += $"{resource.Key}  : {resource.Value} + \n";
        }

        _resourcesWindow.SetText(data);
    }

    private void UpdateTimeWindow(int tick, int month, string monthName)
    {
        string data = $"Tick: {tick}, month {monthName}";
        _timeWindow.SetText(data);
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
        
        _buildingViewWindow.SetText(data);
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