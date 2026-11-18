using UnityEngine;

[CreateAssetMenu(fileName = "NewMusicRegion", menuName = "Fish Game/Music Region Data")]
public class MusicRegionData : ScriptableObject
{
    public string regionName;
    public AudioClip music;

    [Range(0f, 1f)]
    public float volume = 1f;  //can change
}