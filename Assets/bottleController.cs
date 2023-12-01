using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleController : MonoBehaviour
{
    public ParticleSystem sandParticles; // Assign in the Inspector
    public float rotationSpeed = 30f; // Degrees per second
    public float resetTime = 2f; // Time in seconds to reset rotation
    public float maxFlowRate = 50f; // Maximum particle emission rate

    private bool isDragging = false;
    private float pourIntensity = 0f;
    private Vector3 offset;
    private float currentRotation = 0f;
    public GameObject meshHolder;
    private MeshController meshController;
    public float truePourIntensity = 0f;
    public GameManager gameManager;

    public DrugVial drugVial;
    public Transform emissionPoint;
    public AudioSource bottlePickUp;
    public AudioSource pouringSound; // Assign the pouring sound AudioSource in the Inspector
    private bool wasPouringPreviously = false; // To track if it was pouring in the last frame


    void Start()
    {
                meshController = meshHolder.GetComponent<MeshController>();

    }
    void Update()
    {
        if (!isDragging)
        {
            HandleMouseClick();
        }

        if (isDragging)
        {
            HandleDrag();
            HandlePour();
        }

        ResetRotation();
        HandlePouringSound();
        UpdateSandEmission();
    }

    void HandlePouringSound()
    {
        // Check if pouring just started (truePourIntensity surpasses 0)
        if (truePourIntensity > 0f && !wasPouringPreviously)
        {
            pouringSound.Play(); // Start or restart the pouring sound
        }
        else if (truePourIntensity <= 0f && wasPouringPreviously)
        {
            pouringSound.Stop(); // Stop the pouring sound if pouring has ended
        }

        // Adjust the volume based on truePourIntensity
        pouringSound.volume = truePourIntensity+0.5f;

        // Update wasPouringPreviously for the next frame
        wasPouringPreviously = truePourIntensity > 0.001f;
    }
    void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));
                // Set the parent GameObject of this bottle as the active bottle in the GameManager
                bottlePickUp.Play();
                gameManager.SetActiveBottleController(this);
                
            }
        }
    }

    void HandleDrag()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mouseWorldPos = GetMouseWorldPosition() + offset;
        // Get the viewport bounds
        Vector3 minScreenBounds = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10.0f));
        Vector3 maxScreenBounds = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 10.0f));

        // Clamp the position within the viewport bounds
        mouseWorldPos.x = Mathf.Clamp(mouseWorldPos.x, minScreenBounds.x, maxScreenBounds.x);
        mouseWorldPos.y = Mathf.Clamp(mouseWorldPos.y, minScreenBounds.y, maxScreenBounds.y);

        transform.position = mouseWorldPos;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

void HandlePour()
{
    if (Input.GetMouseButtonDown(1)) // Right click just pressed
    {
        // Calculate the initial pourIntensity based on currentRotation
        pourIntensity = Mathf.InverseLerp(0f, -90f, currentRotation);
    }

    if (Input.GetMouseButton(1)) // Right click held
    {
        pourIntensity += Time.deltaTime / (90f / rotationSpeed); // Scale pour intensity based on rotation speed
        pourIntensity = Mathf.Clamp(pourIntensity, 0f, 1f); // Clamp the intensity between 0 and 1
        currentRotation = Mathf.Lerp(0f, -90f, pourIntensity);
    }
    else if (Input.GetMouseButtonUp(1))
    {
        // Optional: Do something when button is released, if needed
    }

    transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);

}

    void ResetRotation()
    {
        if (!Input.GetMouseButton(1) && currentRotation != 0f)
        {
            currentRotation = Mathf.Lerp(currentRotation, 0f, Time.deltaTime / resetTime);
            transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = 10.0f; // Set a fixed distance from the camera
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void UpdateSandEmission()
    {
        // Set emission rate to 0 if the rotation is less than 10 degrees
        if (Mathf.Abs(currentRotation) < 45f)
        {
            var emission = sandParticles.emission;
            emission.rateOverTime = 0f;
            truePourIntensity = 0f;

        }
        else
        {
            // Calculate emission rate based on the current rotation
            float emissionRate = Mathf.InverseLerp(0f, -90f, currentRotation-45f) * maxFlowRate;
            var emission = sandParticles.emission;
            emission.rateOverTime = emissionRate;
            truePourIntensity = pourIntensity-0.5f;
        }
    }
    public Vector3 GetEmissionPoint()
    {
        // Calculate the emission point's X and Y based on the bottle's position
        // Z-coordinate is a placeholder (0.0f) for now
        Vector3 localEmissionPoint = new Vector3(emissionPoint.position.x, 0.0f, 0.0f);
        //Debug.Log("Bottle emission point: " + localEmissionPoint);
        return emissionPoint.TransformPoint(localEmissionPoint);
    }
    public float GetPourIntensity()
    {
        if (drugVial != null)
        {

        drugVial.UpdateVolumeChange(truePourIntensity);
        }
        return truePourIntensity;
    }
}
