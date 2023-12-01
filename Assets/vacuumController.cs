using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vacuumController : MonoBehaviour
{
    public float suctionStrength = .5f;
    public float suctionRadius = 0.005f;
    public Transform suctionTip; // Assign the tip of the vacuum in the inspector

    public bool isSucking = false;
    private Vector3 offset;
    private bool isDragging = false;
    public AudioSource vacuumSound;
    void Update()
    {
        HandleMouseInput();
    }

    void HandleMouseInput()
    {
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));
            }
        }
    }

        if (Input.GetMouseButton(0) && isDragging) // Left mouse button held
        {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        mouseWorldPos.z = transform.position.z; // Keep the original z position

                    // Get the viewport bounds
        Vector3 minScreenBounds = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10.0f));
        Vector3 maxScreenBounds = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 10.0f));

        // Clamp the position within the viewport bounds
        mouseWorldPos.x = Mathf.Clamp(mouseWorldPos.x, minScreenBounds.x, maxScreenBounds.x);
        mouseWorldPos.y = Mathf.Clamp(mouseWorldPos.y, minScreenBounds.y, maxScreenBounds.y);

            transform.position = mouseWorldPos;
        }

        if (Input.GetMouseButtonUp(0)) // Left mouse button released
        {
            isDragging = false;
        }
        if (isDragging){
        if (Input.GetMouseButtonDown(1)) // Right mouse button clicked
        {
            vacuumSound.Play();
            isSucking = true;
        }

        if (Input.GetMouseButtonUp(1)) // Right mouse button released
        {
            isSucking = false;
        }
        }
    }

    public bool IsSucking()
    {
        return isSucking;
    }

    public Vector3 GetSuctionPoint()
    {
        return suctionTip.position;
    }

    public float CalculateSuctionEffect(Vector3 vertexPosition)
    {
        Vector3 suctionPointXZ = new Vector3(GetSuctionPoint().x, GetSuctionPoint().y, vertexPosition.z);
        float distance = Vector3.Distance(vertexPosition, suctionPointXZ);
        Debug.Log("Distance: " + distance);
        Debug.Log("suction point: " + suctionPointXZ);
        if (distance < suctionRadius)
        {
            return Mathf.Lerp(suctionStrength, 0, distance / suctionRadius);
        }
        return 0;
    }
}