using UnityEngine;
using Redcode.Pools;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    PoolManager poolManager;
    public AudioSource BGMAudioSource;
    private void Awake()
    {
        Instance = this;
        poolManager = GetComponent<PoolManager>();
        BGMAudioSource.clip = BGM[0];
        BGMAudioSource.loop = true;
        BGMAudioSource.volume = BGMSoundVolume / 100f;
        BGMAudioSource.Play();
    }

    public AudioClip[] SFX;
    public AudioClip[] BGM;

    List<SoundObject> soundObjects = new List<SoundObject>();
    List<SuccessSoundObject> successSoundObjects = new List<SuccessSoundObject>();
    List<ButtonSoundObject> buttonSoundObjects = new List<ButtonSoundObject>();
    public void SyncVolume()
    {
        foreach (var soundObject in soundObjects)
        {
            soundObject.GetComponent<AudioSource>().volume = SFXSoundVolume / 100f;
        }
        foreach (var soundObject in successSoundObjects)
        {
            soundObject.GetComponent<AudioSource>().volume = SFXSoundVolume / 100f;
        }
        foreach (var soundObject in buttonSoundObjects)
        {
            soundObject.GetComponent<AudioSource>().volume = SFXSoundVolume / 100f;
        }

    }
    public float BGMSoundVolume = 50f;
    public float SFXSoundVolume = 50f;
    IEnumerator ReturnToPoolAfterPlay(SoundObject audioSource)
    {
        yield return new WaitWhile(() => audioSource.GetComponent<AudioSource>().isPlaying);
        soundObjects.Remove(audioSource);
        poolManager.TakeToPool<SoundObject>(audioSource);
    }

    IEnumerator ReturnToPoolAfterPlay(SuccessSoundObject audioSource)
    {
        yield return new WaitWhile(() => audioSource.GetComponent<AudioSource>().isPlaying);
        successSoundObjects.Remove(audioSource);
        poolManager.TakeToPool<SuccessSoundObject>(audioSource);
    }

    IEnumerator ReturnToPoolAfterPlay(ButtonSoundObject audioSource)
    {
        yield return new WaitWhile(() => audioSource.GetComponent<AudioSource>().isPlaying);
        buttonSoundObjects.Remove(audioSource);
        poolManager.TakeToPool<ButtonSoundObject>(audioSource);
    }
    public void PlaySFX(int index, float volumeScale = 1f, bool RandomPitch = false, bool Loop = false)
    {
        SoundObject SoundObject = poolManager.GetFromPool<SoundObject>();
        AudioSource AudioSource = SoundObject.GetComponent<AudioSource>();
        AudioSource.clip = SFX[index];
        if(RandomPitch)
            AudioSource.pitch = Random.Range(0.9f, 1.1f);
        else
            AudioSource.pitch = 1f;
        AudioSource.volume = (SFXSoundVolume / 100f) * volumeScale;
        if(Loop)
            AudioSource.loop = true;
        else
            AudioSource.loop = false;
        AudioSource.Play();
        StartCoroutine(ReturnToPoolAfterPlay(SoundObject));
    }

    public void PlayButtonSound(int index)
    {
        ButtonSoundObject SoundObject = poolManager.GetFromPool<ButtonSoundObject>();
        AudioSource AudioSource = SoundObject.GetComponent<AudioSource>();
        AudioSource.clip = SFX[index];
        AudioSource.volume = SFXSoundVolume;

        AudioSource.Play();
        buttonSoundObjects.Add(SoundObject);
        StartCoroutine(ReturnToPoolAfterPlay(SoundObject));
    }
    public void PlayPitchSFX(int index, float PitchOffset, float volumeScale = 1f)
    {
        SoundObject SoundObject = poolManager.GetFromPool<SoundObject>();
        AudioSource AudioSource = SoundObject.GetComponent<AudioSource>();
        AudioSource.clip = SFX[index];
        AudioSource.pitch = 1f + PitchOffset;
        AudioSource.volume = (SFXSoundVolume / 100f) * volumeScale;
        AudioSource.Play();
        soundObjects.Add(SoundObject);

        StartCoroutine(ReturnToPoolAfterPlay(SoundObject));
    }

    public void SuccessSFX(int index, float volumeScale)
    {
        SuccessSoundObject SoundObject = poolManager.GetFromPool<SuccessSoundObject>();
        AudioSource AudioSource = SoundObject.GetComponent<AudioSource>();
        AudioSource.clip = SFX[index];
        AudioSource.volume = (SFXSoundVolume / 100f) * volumeScale;
        AudioSource.Play();
        successSoundObjects.Add(SoundObject);

        StartCoroutine(ReturnToPoolAfterPlay(SoundObject));
    }
}
