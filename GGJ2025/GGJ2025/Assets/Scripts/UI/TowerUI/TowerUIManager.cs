using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class TowerUIManager : MonoBehaviour
{

    [SerializeField] private TowerInfo towerDataBase;

    [SerializeField] private int currentTowerSelectedIndex;

    [SerializeField] private List<Button> towerSelectionButtons;

    [SerializeField] private int selectedButtonNumber;

    [SerializeField] private Color oringalColor;

    [SerializeField] private Color selectedColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeTowerUI(KeyControl keyControl)
    {
        switch (keyControl.keyCode) 
        {
            case Key.Digit1:
                Debug.Log("1");
                SelectButton(1);
                break;
            case Key.Digit2:
                Debug.Log("2");
                SelectButton(2);
                break;
            case Key.Digit3:
                Debug.Log("3");
                SelectButton(3);
                break;
            case Key.Digit4:
                Debug.Log("4");
                SelectButton(4);
                break;
            case Key.Digit5:
                Debug.Log("5");
                SelectButton(5);
                break;
            case Key.Digit6:
                Debug.Log("6");
                TowerPlacement.Instance.StartRemoving();
                break;
        }
    }

    private void SelectButton(int buttonNumberIndex)
    {
        ResetButtonColors();
        towerSelectionButtons[buttonNumberIndex - 1].image.color = selectedColor;
        TowerPlacement.Instance.StartPlacement(buttonNumberIndex);
        selectedButtonNumber = buttonNumberIndex;
    }

    private void ResetButtonColors()
    {
        foreach (Button temp in towerSelectionButtons)
        {
            temp.image.color = oringalColor;
        }
    }
}
