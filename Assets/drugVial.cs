using UnityEngine;
using TMPro; // This remains the same, as TMP_Text is also part of the TMPro namespace

public class DrugVial : MonoBehaviour
{
    [SerializeField] TMP_Text textLabel; // Changed from TextMeshProUGUI to TMP_Text
    public string drugName;
    public float percentage = 100f;
    public float density = .05f; // Density of drug in g/mL

    void Start()
    {
        UpdateLabel();
    }

    // This method is called by other scripts to update the volume change
    public void UpdateVolumeChange(float volumeChange)
    {
        float percentageChange = density * volumeChange*0.1f;
        percentage -= percentageChange;
        percentage = Mathf.Clamp(percentage, 0f, 100f); // Ensure percentage stays within 0 and 100
        UpdateLabel();
    }

    void UpdateLabel()
    {
        textLabel.text = $"{drugName}: {percentage:F2}%"; // F2 for two decimal places
    }

    // Call this method to set drug name and percentage from other scripts
    public void SetDrugProperties(string name, float initialPercentage, float pourRateValue)
    {
        drugName = name;
        percentage = initialPercentage;
        density = pourRateValue;
        UpdateLabel();
    }
}
