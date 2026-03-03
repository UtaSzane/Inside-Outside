using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class SoundControl: MonoBehaviour {
    [SerializeField] private Slider sfx_slider, music_slider;
    [SerializeField] private TextMeshProUGUI sfx_text, music_text;
    public void SetVolume(bool isSFX) {
        if (isSFX) {
            SoundPlay.SetSFXVolume(sfx_slider.value / 100f);
            sfx_text.text = $"SFX: {sfx_slider.value}%";
        }
        if (!isSFX) {
            SoundPlay.SetMusicVolume(music_slider.value / 100f);
            music_text.text = $"Music: {music_slider.value}%";
        }
    }
    private void Update() {
        var sfx = Mathf.RoundToInt(SoundPlay.GetSFXVolume() * 100f);
        if (sfx_slider.value != sfx) sfx_slider.value = sfx;
        
        var msc = Mathf.RoundToInt(SoundPlay.GetMusicVolume() * 100f);
        if (music_slider.value != msc) music_slider.value = msc;
    }
}