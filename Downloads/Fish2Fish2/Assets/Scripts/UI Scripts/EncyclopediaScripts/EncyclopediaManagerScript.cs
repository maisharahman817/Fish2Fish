using System.Linq;
using UnityEngine;

using UnityEngine;
using System.Linq;
// LINQ lets us filter collections with .Where() instead of manual foreach + if
// Reference: https://learn.microsoft.com/en-us/dotnet/csharp/linq/get-started/write-linq-queries

public class EncyclopediaManager : MonoBehaviour
{

    public static EncyclopediaManager Instance;

    [SerializeField] private GameObject encyclopediaEntry;
    [SerializeField] private Transform content;
    // [System.Serializable] lets Unity display this plain C# class in the Inspector
    // Reference: https://docs.unity3d.com/2022.1/Documentation/ScriptReference/Serializable.html


    private Biome currentBiome = Biome.All;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start() { PrintEntries(); }


    void OnEnable() { PrintEntries(); }
    //every time panel is turned back on 

    public void FilterByBiome(int biome) //primiteve
    {
        currentBiome = (Biome)biome;
        PrintEntries();
    }

    #region Chat generated
    public void PrintEntries()
    {
        if (FishDiscoveryManager.Instance == null) return;

        // Destroy all existing entries before rebuilding — simpler than
        // trying to add/remove only what changed
        foreach (Transform entry in content)
            Destroy(entry.gameObject);

        var discovered = FishDiscoveryManager.Instance.GetDiscoveredFish();

        // If showing All, use the full set; otherwise filter with LINQ
        // f => f.biome == currentBiome is a lambda: a short inline function
        // where f is each fish and the expression is the condition to keep it
        var filtered = currentBiome == Biome.All
            ? discovered
            : discovered.Where(f => f.biome == currentBiome);

        foreach (FishData fish in filtered)
        {
            GameObject entry = Instantiate(encyclopediaEntry, content);
            entry.GetComponent<EncyclopediaEntry>().Setup(fish);
        }
    }
    #endregion
}