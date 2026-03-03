// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[DisallowMultipleComponent]
public class SoundPlay : MonoBehaviour {
    private static SoundPlay instance;
    
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource musicSource, sfxSource;
    [SerializeField] private List<AudioClip> musicClips, sfxClips;
    private readonly Dictionary<string, AudioClip> dclips = new();

    private void Start() {
        if (instance != null) { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject); instance = this;

        dclips.Clear();
        var list = musicClips; list.AddRange(sfxClips);
        foreach (AudioClip clip in list) dclips.Add(clip.name, clip);
        
        musicSource.loop = true;
        
        return;
    }

    private AudioClip Search(string name) {
        if (!dclips.ContainsKey(name)) {
            Debug.LogError("No clips with this name existed.");
            return null;
        }
        return dclips[name];
    }

    private float DbCalc(float v01) => Mathf.Log10(Mathf.Clamp(v01, 0.0001f, 1f)) * 20f;
    private float DbCalc_Rev(float x) => Mathf.Pow(10f, 0.05f * x);
    
    private static void SetVolume(float value, string volume_channel) {
        if (instance == null) return;
        if (!instance.audioMixer.SetFloat(volume_channel, instance.DbCalc(value)))
            Debug.LogError($"{volume_channel} parameter is not exposed.");
    }
    private static float GetVolume(string volume_channel) {
        if (instance == null) return float.NaN;
        if (!instance.audioMixer.GetFloat(volume_channel, out float res))
            Debug.LogError($"{volume_channel} parameter is not exposed.");
        return instance.DbCalc_Rev(res);
    }

    public static void SetMusicVolume(float value) => SetVolume(value, "MusicVolume");
    public static float GetMusicVolume() => GetVolume("MusicVolume");

    public static void SetSFXVolume(float value) => SetVolume(value, "SFXVolume");
    public static float GetSFXVolume() => GetVolume("SFXVolume");

    public static void SetMasterVolume(float value) => SetVolume(value, "MasterVolume");
    public static float GetMasterVolume() => GetVolume("MasterVolume");
    

    // SFX
    private Coroutine sfx_coroutine = null, music_coroutine = null;

    public static void PlaySFX(string name, float timer) {
        if (instance != null) instance.Core_PlaySFX(name, timer);
    }
    private void Core_PlaySFX(string name, float timer) {
        if (sfx_coroutine != null) return;
        sfxSource.pitch = Random.Range(0.8f, 1.2f);
        sfxSource.PlayOneShot(Search(name));
        sfx_coroutine = StartCoroutine(Wait(timer));
    }
    private IEnumerator Wait(float timer) {
        yield return new WaitForSeconds(timer);
        sfx_coroutine = null;
    }

    // Music
    public static void PlayMusic(string name) {
        if (instance != null) instance.Core_PlayMusic(name);
    }
    public void Core_PlayMusic(string name) {
        if (music_coroutine != null) return;
        musicSource.clip = Search(name);
        musicSource.Play();
        music_coroutine = StartCoroutine(MusicWait());
    }
    private IEnumerator MusicWait() {
        yield return new WaitUntil(() => music_coroutine == null);
    }

    public static void StopMusic() {
        if (instance != null) instance.Core_StopMusic();
    }
    private void Core_StopMusic() {
        if (music_coroutine == null) return;
        musicSource.Stop();
        musicSource.clip = null;
        music_coroutine = null;
    }
}