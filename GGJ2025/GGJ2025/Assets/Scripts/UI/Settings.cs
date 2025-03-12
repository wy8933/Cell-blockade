using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Slider SFXSlider;
    public Slider MusicSlider;
    public Dropdown LanguageDropdown;

    public void Start()
    {
        SFXSlider.value = SoundManager.Instance.SFXMult;
        MusicSlider.value = SoundManager.Instance.MusicMult;
    }

    /// <summary>
    /// Change the SFX value
    /// </summary>
    public void SFXOnValueChange()
    {
        SoundManager.Instance.SFXMult = SFXSlider.value;
        Debug.Log(SFXSlider.value);
    }

    /// <summary>
    /// Change the background music value
    /// </summary>
    public void MusicOnValueChange()
    {
        SoundManager.Instance.MusicMult = MusicSlider.value;
        SoundManager.Instance.BGM.volume = MusicSlider.value * 0.5f;
        Debug.Log(MusicSlider.value);
    }

    public void OnLanguageChanged()
    {
        var selectedLocale = LocalizationSettings.AvailableLocales.Locales[LanguageDropdown.value];
        LocalizationSettings.SelectedLocale = selectedLocale;
    }
}
