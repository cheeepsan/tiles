using System;
using System.Collections;
using System.Collections.Generic;
using BuildingNS;
using Game;
using ResourceNS.Enum;
using SaveStateNS;
using Signals.Building;
using Signals.ResourceNS;
using Signals.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Util;
using Zenject;
using Button = UnityEngine.UI.Button;

struct CurrentTimeViewContainer
{
    public int currentTick;
    public String currentMonthName;
    public int currentMonthIndex;
}

public class UiRenderer : MonoBehaviour
{
    [Inject] private readonly Configuration _configuration;
    [Inject] private readonly UiSignals _uiSignalBus;
    [Inject] private readonly BuildingSignals _buildingSignalBus;
    [Inject] private readonly BuildingManager _buildingManager;
    [Inject] private readonly UiBuildingButtonFactory _buildingButtonFactory;
    [Inject] private readonly SaveState _saveState;
    [Inject] private readonly TimeManager _timeManager;

    [SerializeField] public GameObject parentPanel;
    [SerializeField] public GameObject buildingButtonPanel;
    [SerializeField] public GameObject buildingInfoPanel;
    [SerializeField] public GameObject resourcesInfoPanel;
    [SerializeField] public GameObject timeInfoPanel;
    [SerializeField] public GameObject menuButtonPanel;
    
    private TMP_Text _infoWindow;
    private TMP_Text _resourcesWindow;
    private TMP_Text _timeWindow;

    private CurrentTimeViewContainer currentTime;
    
    void Start()
    {
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
    }

    private void UpdateInfoWindow()
    {
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

    private void UpdateTimeWindow(int tick, int month, String monthName)
    {
        String data = $"Tick: {tick}, month {monthName}";
        _timeWindow.SetText(data);
    }
    
    private void SubscribeToSignals()
    {
        SubscribeToBuildingSignals();
        SubscribeToResourceSignals();
        SubscribeToOnTick();
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

    private void SubscribeToOnTick()
    {
        TimeManager.OnTick += delegate(object sender, TimeManager.OnTickEventArgs args)
        {
            int tick = args.currentTick;
            int month = args.month;
            String monthName = args.monthName;
            
            UpdateTimeWindow(tick, month, monthName);
        };
    }
}