using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JetBrains.Annotations; // Make sure to include the TMPro namespace

public class ScoreCardManager : MonoBehaviour
{
    public TMP_Text scoreCardText; // Reference to your TMP Text object
    public float total;
    public float stars;
    public int numberOfCustomers;

    // Start is called before the first frame update
    void Start()
    {

        if (GameManager.Instance != null && scoreCardText != null)
        {
            // Calculate the scores and display
            DisplayScoreCard();
        }
        else
        {
            Debug.LogError("GameManager or TMP_Text is not set in ScoreCardManager");
        }
    }

    private void DisplayScoreCard()
    {
        numberOfCustomers = GameManager.Instance.dailyStarRatings.Count;
        float totalWages = numberOfCustomers * 30;
        float medicationSurplusBonus = GameManager.Instance.totalOver * 5;
        float medicationLostCost = GameManager.Instance.totalUnder * 10;
        float starMultiplier = GameManager.Instance.averageStars / 5.0f;
        total = (totalWages + medicationSurplusBonus - medicationLostCost) * starMultiplier;
        stars = GameManager.Instance.averageStars;

        scoreCardText.text = string.Format(
            "CUSTOMERS COMPLETED:\n\t{0} wages (x $30)\n\n" +
            "UNDERPRESCRIBED:\n\t{1} bonus (x $5/mg)\n\n" +
            "OVERPRESCRIBED:\n\t{2} loss (x $10/mg)\n\n" +
            "STAR RATING: {3} / 5\n\n" +
            "TOTAL:\n\t${4:F2}",
            numberOfCustomers, medicationSurplusBonus, medicationLostCost, GameManager.Instance.averageStars, total
        );
    }

    // Update is called once per frame
    void Update()
    {
        // You can update the score card in real-time if needed
        // or trigger this through some other event.
    }
}
