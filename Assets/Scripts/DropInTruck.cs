using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class DropInTruck : MonoBehaviour
{
    [System.Serializable]
    public class TruckPosition
    {
        public Transform position;
        public GameObject ghostObject;
        public bool isOccupied = false;
        public GameObject compatibleItem; 
    }

    private int counter = 0;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI itemsText;

    [SerializeField] private List<TruckPosition> itemPositions = new List<TruckPosition>();

    private void Start()
    {
        foreach (var pos in itemPositions)
        {
            if (pos.position == null || pos.ghostObject == null || pos.compatibleItem == null)
            {
                Debug.LogWarning("Not all truck positions, ghost objects, or compatible items are set up correctly!");
            }
        }
        UpdateText();
    }

    public TruckPosition FindNearestAvailablePosition(GameObject item, float maxDistance)
    {
        TruckPosition nearestPosition = null;
        float minDistance = maxDistance;

        foreach (var pos in itemPositions)
        {          
            if (!pos.isOccupied && pos.compatibleItem == item)
            {
                float distance = Vector3.Distance(item.transform.position, pos.position.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestPosition = pos;
                }
            }
        }
        return nearestPosition;
    }

    public void PlaceItemInPosition(GameObject item, TruckPosition position)
    {
        item.GetComponent<Rigidbody>().isKinematic = true;
        item.transform.position = position.position.position;
        item.transform.rotation = position.position.rotation;
        item.transform.SetParent(position.position);

        position.ghostObject.SetActive(false);
        position.isOccupied = true;
        counter++;
        UpdateText();
    }

    public void RemoveItemFromPosition(Transform itemPosition)
    {
        TruckPosition freedPosition = itemPositions.Find(pos => pos.position == itemPosition);

        if (freedPosition != null)
        {
            freedPosition.isOccupied = false;
            freedPosition.ghostObject.SetActive(true);
            counter--;
            UpdateText();
        }
    }

    private void UpdateText()
    {
        itemsText.text = "Items in car: " + counter + "/" + itemPositions.Count;
    }
}
