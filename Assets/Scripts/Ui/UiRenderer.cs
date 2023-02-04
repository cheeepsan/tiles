using System;
using System.Collections;
using System.Collections.Generic;
using BuildingNS;
using Game;
using ResourceNS.Enum;
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

public class UiRenderer : MonoBehaviour
{
    [Inject] private Configuration _configuration;
    [Inject] private UiSignals _uiSignalBus;
    [Inject] private BuildingSignals _buildingSignalBus;
    [Inject] private BuildingManager _buildingManager;
    [Inject] private readonly UiBuildingButtonFactory _buildingButtonFactory;

    [SerializeField] public GameObject parentPanel;
    [SerializeField] public GameObject buildingButtonPanel;
    [SerializeField] public GameObject buildingInfoPanel;
    [SerializeField] public GameObject resourcesInfoPanel;
    
    private TMP_Text _infoWindow;
    private TMP_Text _resourcesWindow;

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


        _infoWindow = buildingInfoPanel.GetComponentInChildren<TMP_Text>();
        _infoWindow.SetText("Window ready: buildings");
        
        _resourcesWindow = resourcesInfoPanel.GetComponentInChildren<TMP_Text>();
        _resourcesWindow.SetText("Window ready: resources");
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

    private void SubscribeToSignals()
    {
        SubscribeToBuildingSignals();
        SubscribeToResourceSignals();
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
}