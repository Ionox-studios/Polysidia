using UnityEngine;

public class syringeController : MonoBehaviour
{
    public Camera mainCamera; // Assign in the Inspector
    public MeshController meshController; // Assign in the Inspector
    public GameManager gameManager; // Assign in the Inspector
    public GameObject fullSyringeSprite; // Assign in the Inspector
    public GameObject emptySyringeSprite; // Assign in the Inspector

    public float weightRecord = 0.0f;
    public bool holdingWeight = false;
    private bool isDragging = false;
    public AudioSource syringeIn;
    public AudioSource syringeOut;
    private Vector2 startingPosition; // Variable to store the starting position

    void Start()
    {
        startingPosition = transform.position; // Store the starting position

        emptySyringeSprite.SetActive(true);
        fullSyringeSprite.SetActive(false);
        Debug.Log("Starting position set to: " + startingPosition); // Debugging

    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 rayPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            int layerMask = LayerMask.GetMask("SyringeLayer");

            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                weightRecord = meshController.weight;
                holdingWeight = true;
                meshController.ResetMesh();
                isDragging = true;

                // Switch to full syringe sprite
                fullSyringeSprite.SetActive(true);
                emptySyringeSprite.SetActive(false);
                syringeIn.Play();
            }
        }

        if (isDragging && Input.GetMouseButton(0))
        {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // Get the viewport bounds
        Vector2 minScreenBounds = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector2 maxScreenBounds = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        // Clamp the position within the viewport bounds
        mousePos.x = Mathf.Clamp(mousePos.x, minScreenBounds.x, maxScreenBounds.x);
        mousePos.y = Mathf.Clamp(mousePos.y, minScreenBounds.y, maxScreenBounds.y);

        transform.position = mousePos;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered");
        if (other.gameObject.CompareTag("Patient") && holdingWeight)
        {
            // Emit the value to the GameManager
            gameManager.weightValue = weightRecord;

            // Reset weightRecord and holdingWeight
            weightRecord = 0.0f;
            holdingWeight = false;

            // Switch back to empty syringe sprite
            fullSyringeSprite.SetActive(false);
            emptySyringeSprite.SetActive(true);
            gameManager.patientActive = false;
            syringeOut.Play();
            // Teleport the syringe back to its starting position
            isDragging = false;
            transform.position = startingPosition;
            Debug.Log("Conditions met, teleporting syringe."); // Debugging

        }
    }
}
