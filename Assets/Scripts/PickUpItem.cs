using TMPro;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float pickupRange = 3f;
    [SerializeField] private float holdRange = 2f;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI interactionText;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource; 


    private GameObject highlightedObject = null;
    private GameObject heldObject = null;
    [SerializeField] private DropInTruck dropInTruck;

    private Outline outlineComponent; 



    private void Update()
    {
        if (heldObject == null)
        {
            HandleObjectHighlighting();
            HandleObjectPickUp();
        }
        else
        {
            HandleHoldRangeAdjustment();
            HoldObject();
            HandleObjectDrop();
        }
    }

    private void HandleHoldRangeAdjustment()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            holdRange = Mathf.Min(holdRange + 0.1f, 2f);
        }
        else if (scroll < 0f)
        {
        
            holdRange = Mathf.Max(holdRange - 0.1f, 1f);
        }
    }

    private void HandleObjectDrop()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            DropObject();
        }
    }

    private void HandleObjectHighlighting()
    {
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hit, pickupRange))
        {
            if (hit.collider.CompareTag("Pickable"))
            {
                GameObject target = hit.collider.gameObject;

               
                if (highlightedObject != target)
                {
                    ClearHighlight();
                    highlightedObject = target;
                    outlineComponent = highlightedObject.GetComponent<Outline>();
                    if (outlineComponent != null)
                    {
                        outlineComponent.enabled = true;
                    }
                    interactionText.text = "Press E to pick up";
                }
            }
            else
            {
                ClearHighlight();
            }
        }
        else
        {
            ClearHighlight();
        }
    }

    private void HandleObjectPickUp()
    {
        if (highlightedObject != null && Input.GetKeyDown(KeyCode.E))
        {
            if (highlightedObject.transform.parent != null && highlightedObject.transform.parent.CompareTag("TruckPosition"))
            {
                dropInTruck.RemoveItemFromPosition(highlightedObject.transform.parent);
                highlightedObject.transform.SetParent(null);
            }
            PickUp(highlightedObject);
        }
    }

    

    private void HoldObject()
    {
        Vector3 holdPosition = playerCamera.position + playerCamera.forward * holdRange;

      
        Vector3 directionToHoldPosition = holdPosition - heldObject.transform.position;
        float distanceToHoldPosition = directionToHoldPosition.magnitude;

        
        if (Physics.Raycast(heldObject.transform.position, directionToHoldPosition.normalized, out RaycastHit hit, distanceToHoldPosition))
        {
          
            heldObject.transform.position = hit.point - directionToHoldPosition.normalized * 0.1f;
        }
        else
        {
            
            heldObject.transform.position = holdPosition;
        }
    }


    private void PickUp(GameObject target)
    {
        heldObject = target;
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        audioSource.PlayOneShot(audioSource.clip);
        ClearHighlight();
    }

    private void DropObject()
    {
        if (heldObject != null)
        {
            float maxDistance = 1.0f;           
            DropInTruck.TruckPosition nearestPosition = dropInTruck.FindNearestAvailablePosition(heldObject, maxDistance);

            if (nearestPosition != null)
            {               
                dropInTruck.PlaceItemInPosition(heldObject, nearestPosition);
            }
            else
            {
                Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                }
            }
            heldObject = null;
            audioSource.PlayOneShot(audioSource.clip);
        }
    }


    private void ClearHighlight()
    {
        if (highlightedObject != null && outlineComponent != null)
        {
            outlineComponent.enabled = false; 
        }
        highlightedObject = null;
        outlineComponent = null;
        interactionText.text = "";
    }
}
