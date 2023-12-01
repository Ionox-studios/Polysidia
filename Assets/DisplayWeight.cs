using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayWeight : MonoBehaviour
{
    public float weight = 0.0f; // Actual weight
    private float displayedWeight = 0.0f; // Weight displayed on the UI
    public GameObject meshHolder;
    private MeshController meshController;

    [SerializeField] TMP_Text weightText;

    private float lerpSpeed = 2.0f; // Speed at which the weight display updates

    // Start is called before the first frame update
    void Start()
    {
        meshController = meshHolder.GetComponent<MeshController>();
        weight = meshController.weight;
        displayedWeight = weight; // Initialize displayed weight
    }

    // Update is called once per frame
    void Update()
    {
        // Update actual weight
        weight = meshController.weight;

        // Smoothly update the displayed weight
        displayedWeight = Mathf.Lerp(displayedWeight, weight, lerpSpeed * Time.deltaTime);

        // Update the text display with the interpolated weight value
        weightText.text = displayedWeight.ToString("F2") + " mg";
    }
}
