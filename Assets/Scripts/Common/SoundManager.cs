using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("UI Interact")]
    public AudioClip clickSound;
    public AudioClip completedSound;
    public AudioClip failedSound;
    public AudioClip selectedCharacterSound;
    public AudioClip placedCharacterSound;
    public AudioClip cancelCharacterSound;
    public AudioClip upgradeCharacterSound;
    public AudioClip decreaseHealthSound;

    [Header("Character Attack")]
    public AudioClip witcherSound;
    public AudioClip bomberSound;
    public AudioClip hammerSound;
    public AudioClip archerSound;
    public AudioClip enemyDead;
    [SerializeField] private AudioSource audioSource;

    public void Awake()
    {
        if (Instance != null) 
        {
            DestroyImmediate(gameObject);
        } 
        else 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Start()
    {
        audioSource.Play();
    }

    public void PlaySound(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip, 3f);
    }
}
