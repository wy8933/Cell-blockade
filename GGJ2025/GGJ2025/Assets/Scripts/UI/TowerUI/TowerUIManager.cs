using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class TowerUIManager : MonoBehaviour
{

    [SerializeField] private GameObject _buttonHolder; 

    [SerializeField] private int currentTowerSelectedIndex;

    [SerializeField] private List<Button> towerSelectionButtons;

    [SerializeField] private int selectedButtonNumber;

    [SerializeField] private Color oringalColor;

    [SerializeField] private Color selectedColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        if (EnemyWaveManager.Instance.currenTime > 0)
        {
            TowerPlacement.Instance.isBuildingMode = true;
            _buttonHolder.SetActive(true);
        }
        else
        {
            TowerPlacement.Instance.isBuildingMode = false;
            TowerPlacement.Instance.StopPlacement();
            _buttonHolder.SetActive(false);
        }

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
                SelectRemoveButton();
                break;
        }
    }

    public void SelectButton(int buttonNumberIndex)
    {
        ResetButtonColors();
        selectedButtonNumber = buttonNumberIndex;
        towerSelectionButtons[selectedButtonNumber - 1].image.color = selectedColor;
        TowerPlacement.Instance.StartPlacement(selectedButtonNumber);
        
    }

    public void SelectRemoveButton()
    {
        ResetButtonColors();
        selectedButtonNumber = 6;
        towerSelectionButtons[selectedButtonNumber - 1].image.color = selectedColor;
        TowerPlacement.Instance.StartRemoving();
        
    }

    private void ResetButtonColors()
    {
        foreach (Button temp in towerSelectionButtons)
        {
            temp.image.color = oringalColor;
        }
    }

    public void ScrollSelect(Vector2 scroll)
    {
        //Debug.Log(scroll.y);
        if (scroll.y > 0)
        {
            selectedButtonNumber += 1;
            if (selectedButtonNumber > 6)
            {
                selectedButtonNumber = 1;
            }

        }
        else if (scroll.y < 0)
        {
            selectedButtonNumber -= 1;
            if (selectedButtonNumber < 1)
            {
                selectedButtonNumber = 6;
            }
        }

        if (selectedButtonNumber != 6)
        {
            SelectButton(selectedButtonNumber);
        }
        else
        {
            SelectRemoveButton();
        }
    }
}
