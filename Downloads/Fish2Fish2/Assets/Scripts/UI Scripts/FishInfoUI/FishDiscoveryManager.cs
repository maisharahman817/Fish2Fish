using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
// [System.Serializable] lets Unity display this plain C# class in the Inspector
// Reference: https://docs.unity3d.com/2022.1/Documentation/ScriptReference/Serializable.html

public class RegionData
{
    public string regionName;
    public List<FishData> requiredFish;
    public AudioClip music;
    [Range(0f, 1f)] public float volume = 1f;
    public UnityEvent onCompleted;
    [HideInInspector] public bool unlocked = false;
    // HideInInspector: keeps unlocked out of the Inspector so it can't be edited accidentally, but public so FishDiscoveryManager can read and write it
    //reference: https://docs.unity3d.com/ScriptReference/HideInInspector.html
}

public class FishDiscoveryManager : MonoBehaviour
{
    public static FishDiscoveryManager Instance { get; private set; }

    [Header("UI References")]
    public FishInfoPopup infoPopup;

    public FishCounter counter;

    [Header("Fish Data")]
    public FishData[] allFish;

    [Header("End Game")]
    public GameObject endGamePanel;

    [Header("Regions")]
    public List<RegionData> regions;

    private HashSet<FishData> discovered = new HashSet<FishData>();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Discover(FishData data)
    {
        bool isNew = discovered.Add(data);
        if (!isNew) return;

        if (infoPopup != null) infoPopup.Show(data);
        else Debug.LogError("infoPopup is null!");

        if (counter != null) counter.UpdateCount(discovered.Count, allFish.Length);
        else Debug.LogError("counter is null!");

        if (EncyclopediaManager.Instance != null) EncyclopediaManager.Instance.PrintEntries();
        else Debug.LogError("EncyclopediaManagerScript.Instance is null!");

        CheckAllRegions();
        CheckWinCondition();
    }

    void CheckWinCondition()
    {
        // if nobody assigned the array, .Length would crash
        if (allFish == null || discovered.Count < allFish.Length) return;

        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);
            Time.timeScale = 0f; // freeze the game on win
        }
        else Debug.LogWarning("endGamePanel is null!");
    }

    #region
    void CheckAllRegions()
    {
        foreach (RegionData region in regions)
        {
            if (region.unlocked) continue;
            if (region.requiredFish == null || region.requiredFish.Count == 0) continue;

            bool complete = true;
            foreach (FishData fish in region.requiredFish)
            {
                if (!discovered.Contains(fish)) { complete = false; break; }
            }

            if (complete)
            {
                region.unlocked = true;
                Debug.Log(region.regionName + " complete!");
                region.onCompleted?.Invoke();
                // ?. is the null-conditional operator: only calls Invoke() if onCompleted is not null
            }
        }
    }
    #endregion

    public HashSet<FishData> GetDiscoveredFish()
    {
        return discovered;
    }
}