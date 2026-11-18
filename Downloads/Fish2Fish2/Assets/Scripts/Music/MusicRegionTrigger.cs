using UnityEngine;

public class MusicRegionTrigger : MonoBehaviour
{
    public MusicRegionData region;

    void OnTriggerEnter(Collider other)
    {

        if (!other.CompareTag("Player")) return;

        BackgroundMusicManager.Instance?.PlayRegion(region);
    }
}
