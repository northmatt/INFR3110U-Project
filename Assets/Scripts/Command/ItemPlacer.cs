using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPlacer : MonoBehaviour {
    static List<Transform> items = new List<Transform>();

    public static void PlaceItem(Transform item) {
        Transform newItem = item;
        items.Add(newItem);
    }
    public static void RedoItem(Vector3 position, Transform item) {
        //Fix this, no reference after object deletion from line23
        if (!item)
            return;

        Transform newitem = Instantiate(item, position, Quaternion.identity);
        items.Add(newitem);
    }
    public static void RemoveItem(Vector3 position) {
        foreach (Transform curItem in items) {
            if (curItem.position == position) {
                GameObject.Destroy(curItem.gameObject);
                items.Remove(curItem);
                break;
            }
        }
    }
}
