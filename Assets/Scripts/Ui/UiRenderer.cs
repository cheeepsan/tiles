using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Signals.Building;
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

    private TMP_Text _infoWindow;
    
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
   
        var buttons = GameObject.FindGameObjectsWithTag("button");
        using var enumerator = _configuration.GetCfgBuildingList().GetEnumerator();

        if (enumerator.MoveNext())
        {
            foreach (var button in buttons)
            {
                var currentBuilding = enumerator.Current;
                if (currentBuilding != null)
                {
                    Button b = button.GetComponent<Button>();
                    TMP_Text text = button.GetComponentInChildren<TMP_Text>();

                    text.SetText(currentBuilding.name);
                    
                    b.onClick.AddListener(() =>
                    {
                        _uiSignalBus.FireBuildingButtonEvent(new BuildingButtonClickedSignal { buildingConf = currentBuilding });
                    });
                }

                if (enumerator.MoveNext() == false)
                {
                    break;
                }
            }
            
            var info = GameObject.FindGameObjectWithTag("info");
            _infoWindow = info.GetComponent<TMP_Text>();
            _infoWindow.SetText("Window ready");
        }
    }

    private void UpdateInfoWindow()
    {
        string buildingManagerStatus = _buildingManager.GetStateInfo();
        _infoWindow.SetText(buildingManagerStatus);
    }
    private void SubscribeToSignals()
    {
        Action onBuildingSignal = UpdateInfoWindow;
        _buildingSignalBus.Subscribe<BuildingPlacedSignal>(onBuildingSignal);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
