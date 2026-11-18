using UnityEngine;
using TMPro;

public class FishCounter : MonoBehaviour
{

    public TMP_Text counterText;

    void Start()
    {
        //counterText.text = "1/0";
    }

    public void UpdateCount(int current, int total)
    {
        if (counterText != null)
            counterText.text = current + "/" + total;
        else
            Debug.LogError("counterText is null!");
    }
}