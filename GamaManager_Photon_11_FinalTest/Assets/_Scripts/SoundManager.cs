using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    // singleton
    static public SoundManager instance;
    #region singleton
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else { Destroy(this.gameObject); }
    }
    #endregion
    public AudioSource[] audioSourceSFX;
    public AudioSource audioSourceBGM;

    public string[] playSoundName;

    public Sound[] SFXSounds;
    public Sound[] BGMSounds;

    void Start()
    {
        playSoundName = new string[audioSourceSFX.Length];

        VolumeSetting(0.2f);
    }

    public void VolumeSetting(float newVolume)
    {
        for(int i = 0; i < audioSourceSFX.Length; i++)
        {
            audioSourceSFX[i].volume = newVolume;
        }

        audioSourceBGM.volume = newVolume;
    }

    public void PlayBGM(string name)
    {
        for(int i = 0; i < BGMSounds.Length; i++)
        {
            if(name == BGMSounds[i].name)
            {
                audioSourceBGM.Stop();
                audioSourceBGM.clip = BGMSounds[i].clip;
                audioSourceBGM.Play();
            }
        }
    }

    public void PlaySFX(string name)
    {
        for(int i = 0; i < SFXSounds.Length; i++)
        {
            if(name == SFXSounds[i].name)
            {
                for(int j = 0; j< audioSourceSFX.Length; j++)
                {
                    if(!audioSourceSFX[j].isPlaying)
                    {
                        playSoundName[j] = SFXSounds[i].name;
                        audioSourceSFX[j].clip = SFXSounds[i].clip;
                        audioSourceSFX[j].Play();
                        return;
                    }
                }
                Debug.Log("모든 오디오 소스가 사용중입니다.");
                return;
            }
        }
        Debug.Log("같은 이름의 사운드가 없습니다.");
        return;
    }

    public void StopAllSFX()
    {
        for(int i = 0; i < audioSourceSFX.Length; i++)
        {
            audioSourceSFX[i].Stop();
        }
    }

    public void StopSFX(string name)
    {
        for (int i = 0; i < audioSourceSFX.Length; i++)
        {
            if(playSoundName[i] == name)
            {
                audioSourceSFX[i].Stop();
            }
        }
        Debug.Log("재생 중인" + name + "사운드가 없습니다.");
    }

    public void StopBGM()
    {
        audioSourceBGM.Stop();
    }
}
