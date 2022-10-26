using System.Collections;
using System.Collections.Generic;
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
    void Start()
    {
        if (_configuration != null)
        {
            Instantiate();
        }
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
                    
                    b.onClick.AddListener(() => { Debug.Log(currentBuilding.name);});
                }

                if (enumerator.MoveNext() == false)
                {
                    break;
                }
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
