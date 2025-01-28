using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUpManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject _powerUpPanel;
    [SerializeField] private List<Button> _powerUpButtons;
    [SerializeField] private List<TMP_Text> _powerUpNames;
    [SerializeField] private List<TMP_Text> _powerUpDescriptions;
    [SerializeField] private List<Image> _powerUpIcons;

    [Header("Power-Up Data")]
    [SerializeField] private List<BuffData> _firstTimePowerUps;
    [SerializeField] private List<BuffData> _normalPowerUps;
    private List<BuffData> _chosenPowerUps = new List<BuffData>();

    [Header("References")]
    [SerializeField] private BuffHandler _buffHandler;
    private GameObject _player;
    private bool _isFirstTime = true;

    private void Start()
    {
        _powerUpPanel.SetActive(false);
        _player = GameObject.FindGameObjectWithTag("Player");
        _buffHandler = _player.GetComponent<BuffHandler>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.Pause();
        }
    }

    /// <summary>
    /// Show the selection of power up to the menu
    /// </summary>
    public void ShowPowerUpSelection()
    {
        _chosenPowerUps.Clear();
        List<BuffData> powerUpPool = _isFirstTime ? _firstTimePowerUps : _normalPowerUps;

        if (powerUpPool.Count == 0)
        {
            return;
        }

        // Choose 3 random power up from the list
        HashSet<int> selectedIndices = new HashSet<int>();
        while (selectedIndices.Count < _powerUpButtons.Count)
        {
            int randomIndex = Random.Range(0, powerUpPool.Count);
            if (!selectedIndices.Contains(randomIndex))
            {
                selectedIndices.Add(randomIndex);
                _chosenPowerUps.Add(powerUpPool[randomIndex]);
            }
        }

        // Add the info of the power up to the menu and add event listener
        for (int i = 0; i < _powerUpButtons.Count; i++)
        {
            BuffData buff = _chosenPowerUps[i];

            _powerUpNames[i].text = buff.buffName;
            _powerUpDescriptions[i].text = buff.description;
            _powerUpIcons[i].sprite = buff.icon;

            int index = i;
            _powerUpButtons[i].onClick.RemoveAllListeners();
            _powerUpButtons[i].onClick.AddListener(() => ApplyPowerUp(index));
        }

        // Show the menu
        _powerUpPanel.SetActive(true);
        GameManager.Instance.isPowerUp = true;
        GameManager.Instance.Pause();
    }

    /// <summary>
    /// Apply the choosen power up to player
    /// </summary>
    /// <param name="index">Which power up is it from the power up list</param>
    public void ApplyPowerUp(int index)
    {
        if (index < 0 || index >= _chosenPowerUps.Count)
        {
            return;
        }

        BuffData selectedBuff = _chosenPowerUps[index];

        // First power up will always be setting the weapon type player wants to use
        if (_isFirstTime) 
        {
            switch (_chosenPowerUps[index].buffName) {
                case "Rapid-Fire":
                    _player.GetComponent<PlayerController>().weaponType = WeaponType.RapidFire;
                    break;
                case "Scatter":
                    _player.GetComponent<PlayerController>().weaponType = WeaponType.Scatter;
                    break;
                case "ShotGun":
                    _player.GetComponent<PlayerController>().weaponType = WeaponType.ShotGun;
                    break;

            } 
        }

        // Get the info of the buff
        BuffInfo buffInfo = new BuffInfo { buffData = selectedBuff, currentStack = 1 };
        buffInfo.target = _player;
        buffInfo.creator = _player;

        // Add buff to player's buff list
        _buffHandler.AddBuff(buffInfo);
        _isFirstTime = false;

        // Exit power up menu
        _powerUpPanel.SetActive(false);
        GameManager.Instance.Pause();
        GameManager.Instance.isPowerUp = false;
    }
}
