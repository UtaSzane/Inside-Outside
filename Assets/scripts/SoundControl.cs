using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class SoundControl: MonoBehaviour {
    [SerializeField] private Slider sfx_slider, music_slider, master_slider;
    [SerializeField] private TextMeshProUGUI sfx_text, music_text, master_text;
    public void SetSFXVolume() {
        SoundPlay.SetSFXVolume(sfx_slider.value / 100f);
        sfx_text.text = $"{sfx_slider.value}%";
    }
    public void SetMusicVolume() {
        SoundPlay.SetMusicVolume(music_slider.value / 100f);
        music_text.text = $"{music_slider.value}%";
    }
    public void SetMasterVolume() {
        SoundPlay.SetMasterVolume(master_slider.value / 100f);
        master_text.text = $"{master_slider.value}%";
    }

    private void Update() {
        var sfx = Mathf.RoundToInt(SoundPlay.GetSFXVolume() * 100f);
        if (sfx_slider.value != sfx) sfx_slider.value = sfx;
        
        var msc = Mathf.RoundToInt(SoundPlay.GetMusicVolume() * 100f);
        if (music_slider.value != msc) music_slider.value = msc;

        var mst = Mathf.RoundToInt(SoundPlay.GetMasterVolume() * 100f);
        if (master_slider.value != mst) master_slider.value = mst;
    }
}