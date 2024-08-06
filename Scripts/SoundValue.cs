using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundValue : MonoBehaviour
{
    [SerializeField] private List<AudioSource> audioSources = new List<AudioSource>();
    private JsonManager jsonManager;
    private PlayerData playerData;

    void Update()
    {
        jsonManager = GetComponent<JsonManager>();
        playerData = jsonManager.ReadJson();

        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.volume = playerData.soundValue;
        }
    }
}
