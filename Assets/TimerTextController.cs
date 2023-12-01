using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class TimerTextController : MonoBehaviour
{
    // Start and end times in hours
    private const float startTimeInHours = 8f; // 8 AM
    private const float endTimeInHours = 19f; // 7 PM

    // Total duration in game time (11 hours)
    private const float totalGameTime = (endTimeInHours - startTimeInHours) * 3600f; // 11 hours in seconds

    // Real time duration (modifiable in editor)
    public float totalRealTime = 600f; // Default: 10 minutes in seconds

    // Current game time in seconds
    private float currentGameTime = 0.0f;
    private bool soundPlayed = false; // Flag to check if the sound has been played


    // Day done flag
    public bool dayDone = false;

    // TMP text
    [SerializeField] TMP_Text timerText;
    public GameObject Patient;
    
    private BoxCollider2D myCollider; // Reference to the BoxCollider2D on this GameObject
    public AudioSource dayEndSound;

    void Start()
    {
        // Initialize the current game time to the start time
        currentGameTime = startTimeInHours * 3600f;
        // Get the collider component and initially disable it
        myCollider = GetComponent<BoxCollider2D>();
        myCollider.enabled = false;
    }

    void Update()
    {
        if (!dayDone)
        {
            // Update the current game time based on the real time elapsed
            currentGameTime += (Time.deltaTime * totalGameTime) / totalRealTime;

            // Format and display the current game time
            System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(currentGameTime);
            timerText.text = timeSpan.ToString("hh':'mm':'ss");

            // Check if the end time has been reached
            if (currentGameTime >= endTimeInHours * 3600f)
            {
                dayDone = true;
            }
        }
        else
        {
            // Change text to 'LEAVE'
            timerText.text = "LEAVE";
            if (!soundPlayed)
            {
                // Deactivate the patient object
                Patient.SetActive(false);
                myCollider.enabled = true;
                dayEndSound.Play();
                soundPlayed = true; // Set the flag to true after playing the sound
            }
        }

    }
        private void OnMouseDown()
    {
        // Load the 'ScoreCard' scene when this collider is clicked
        // Ensure this code only runs when dayDone is true
        if (dayDone)
        {
            SceneManager.LoadScene("ScoreBoard1");
        }
    }
}
