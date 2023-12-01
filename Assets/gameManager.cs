using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject patientObj; // Reference to the patient prefab
    public Transform spawnPoint; // Where new patients will be spawned
    public DialogueRandomizer dialogueRandomizer; // Reference to the DialogueRandomizer script

    // Singleton instance
    public static GameManager Instance { get; private set; }
    
    // Field to hold the reference to the parent GameObject of the current active bottle
    private BottleController bottleControllerActive;

    public bool patientActive = false;
    public float weightValue = 0.0f;
    public float patientWeight = 0.0f;
    // Drug Dosage Dictionary
    private Dictionary<string, float> drugDosageDictionary = new Dictionary<string, float>();

    // Fields for dosage calculation
    public float totalOver = 0f;
    public float totalUnder = 0f;
    private int maxStars = 5;
    public GameObject[] starObjects; // Array to hold star GameObjects

    
    public float drugThreshold = 1f;
    public float patientAngerThreshold = 30f;
    public float patientSpawnTime = 0.0f;
    public List<int> dailyStarRatings = new List<int>();
    public int starRating = 0;
    public MeshController meshController;
    public float totalStars = 0f;
    public float averageStars = 0f;
    void Awake()
    {
        // Ensure that there's only one instance of GameManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        InitializeDrugDosageDictionary();
    }
     private void InitializeDrugDosageDictionary()
    {
        // Example: Add drug names and their dosages
        drugDosageDictionary.Add("149s", 1.0f); // Dosage in some units
        drugDosageDictionary.Add("BINARY'S SOMA", 4.0f);
        drugDosageDictionary.Add("CHERRY BOMB", 2.0f);
        drugDosageDictionary.Add("COSMIC HORROR", 1.0f);
        drugDosageDictionary.Add("ETERNAL SUNSHINE", 4.0f);
        drugDosageDictionary.Add("FLAUROS", 1.0f);
        drugDosageDictionary.Add("FORGETS", 2.0f);
        drugDosageDictionary.Add("PROZIUM III", 2.0f);
        drugDosageDictionary.Add("ROBO", 4.0f);
        drugDosageDictionary.Add("RONO", 2.0f);
        drugDosageDictionary.Add("SPACE METH", 4.0f);
        drugDosageDictionary.Add("X-LITE", 4.0f);
    
        // Add more drugs and their dosages as needed
    }
    void Start()
    {
        SpawnPatient();
        patientSpawnTime = Time.time;

    }
    void Update()
    {
        if (patientActive == false)
        {
            DespawnPatient();
        }
        UpdateStarDisplay(starRating);
        averageStars = GetAverageStarRatingForTheDay();//probably put this somewhere else like at scene end
    }
    public void UpdateStarDisplay(int totalStars)
{
    // Reset all stars
    foreach (var star in starObjects)
    {
        star.SetActive(false);
    }

    // Activate the number of stars based on totalStars
    for (int i = 0; i < totalStars; i++)
    {
        if (i < starObjects.Length)
        {
            starObjects[i].SetActive(true);
        }
    }
}

    // Method to spawn a new patient
    public void SpawnPatient()
    {
        patientActive = true;
        patientObj.SetActive(true);
        SetupPatient(patientObj);

        // Get all SpriteRenderer components in the patient object and its children
        SpriteRenderer[] patientRenderers = patientObj.GetComponentsInChildren<SpriteRenderer>();

        // Reset each renderer's color to fully opaque
        foreach (SpriteRenderer renderer in patientRenderers)
        {
            Color originalColor = renderer.material.color;
            renderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        }
        patientSpawnTime = Time.time;
    }

    public void DespawnPatient()
    {
        patientActive = true;
        CompareWeight(GameObject.FindGameObjectWithTag("Patient").GetComponent<PatientManager>().GetDialogue());
        StartCoroutine(FadeAndDespawnPatient(GameObject.FindGameObjectWithTag("Patient")));

    }
        // Coroutine to fade out the patient and then despawn
private IEnumerator FadeAndDespawnPatient(GameObject patient)
{
    // Get all SpriteRenderer components in the patient object and its children
    SpriteRenderer[] patientRenderers = patient.GetComponentsInChildren<SpriteRenderer>();
    Collider2D patientCollider = patient.GetComponent<Collider2D>();

    // Temporarily disable the patient's collider
    if (patientCollider != null)
    {
        patientCollider.enabled = false;
    }

    // Check if there are any renderers
    if (patientRenderers.Length > 0)
    {
        float fadeDuration = 2f; // Duration of the fade in seconds
        float elapsedTime = 0;

        // Store the original colors of each renderer
        Color[] originalColors = new Color[patientRenderers.Length];
        for (int i = 0; i < patientRenderers.Length; i++)
        {
            originalColors[i] = patientRenderers[i].material.color;
        }

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            // Apply fading to each renderer
            for (int i = 0; i < patientRenderers.Length; i++)
            {
                Color originalColor = originalColors[i];
                patientRenderers[i].material.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Set the final transparent color for each renderer
        for (int i = 0; i < patientRenderers.Length; i++)
        {
            Color originalColor = originalColors[i];
            patientRenderers[i].material.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        }
    }

    // Re-enable the collider after the fade out
    if (patientCollider != null)
    {
        patientCollider.enabled = true;
    }

    SpawnPatient(); // Spawn a new patient after the current one is despawned
}


    private void CompareWeight(DialoguePatInfo patientInfo)
    {
        if (drugDosageDictionary.TryGetValue(patientInfo.Trait, out float standardDose))
        {
            float calculatedDose = standardDose * patientInfo.Weight;
            float dosageDifference = Mathf.Abs(calculatedDose - weightValue);

            // Calculate total over or under dosage
            if (calculatedDose > weightValue)
                totalOver += dosageDifference;
            else
                totalUnder += dosageDifference;

            // Calculate star rating based on dosage accuracy
            starRating = CalculateStarRating(dosageDifference/calculatedDose);
            if (patientInfo != null && bottleControllerActive != null)
            {
                if (patientInfo.Trait != bottleControllerActive.drugVial.drugName)
                {
                    starRating = 0;
                }
            }
            else
            {
                    starRating = 0;

            }

            Debug.Log("Star Rating: " + starRating);
            Debug.Log("Total Over: " + totalOver);
            Debug.Log("Total Under: " + totalUnder);

            // Add star rating to the list
            dailyStarRatings.Add(starRating);
            Debug.Log("Daily Star Ratings: " + dailyStarRatings);
        }
        else
        {
            Debug.LogError("Drug not found in dictionary: " + patientInfo.Trait);
        }
    }
    
    // Calculate star rating based on total over or under dosage
    private int CalculateStarRating(float dosageDifference)
    {
        // Define thresholds for star rating drops (e.g., every 20% over/under)
        float threshold = drugThreshold; // 20%
        int starsDropped = (int)(dosageDifference / threshold);
        Debug.Log("dosage diff " + dosageDifference);
        Debug.Log("threshold: " + threshold);
        Debug.Log("Stars Dropped: " + starsDropped);
        float elapsedTime = Time.time - patientSpawnTime;
        int timeStarsDrop = (int)(Mathf.Max(0, elapsedTime) / patientAngerThreshold);
        Debug.Log("Time Stars Drop: " + timeStarsDrop);
        return Mathf.Clamp(maxStars - starsDropped-timeStarsDrop, 0, maxStars);
    }

    // Method to set the parent GameObject of the current active bottle
    public void SetActiveBottleController(BottleController bottleController)
    {
        bottleControllerActive = bottleController;
        Debug.Log("Active Bottle: " + bottleControllerActive.drugVial.drugName);
        // Optional: Add any additional logic you need when the active bottle parent changes
        UpdateMeshControllerBottleController();
    }
    private void UpdateMeshControllerBottleController()
    {
        // Assuming you have a reference to MeshController
        if (meshController != null)
        {
            meshController.SetBottleController(bottleControllerActive);
        }
    }
    // Set up the patient with random appearance and dialogue
    private void SetupPatient(GameObject patient)
    {
        // Get the RandomizeAppearance and PatientManager script from the new patient
        RandomizeAppearance randomizeAppearance = patient.GetComponent<RandomizeAppearance>();
        PatientManager patientManager = patient.GetComponent<PatientManager>();

        // Ensure scripts are attached
        if (randomizeAppearance == null || patientManager == null)
        {
            Debug.LogError("Required scripts are not attached to the patient prefab.");
            return;
        }

        // Set up the patient
        patientManager.SetRandomizer(dialogueRandomizer);
        patientManager.InitializePatient();
    }
    // Method to get the total stars for the day
    public int GetTotalStarsForTheDay()
    {
        int totalStars = 0;
        foreach (int stars in dailyStarRatings)
        {
            totalStars += stars;
        }
        return totalStars;
    }

    // Method to get the average star rating for the day
    public float GetAverageStarRatingForTheDay()
    {
        if (dailyStarRatings.Count == 0) return 0;
        int totalStars = GetTotalStarsForTheDay();
        return (float)totalStars / dailyStarRatings.Count;
    }
}
