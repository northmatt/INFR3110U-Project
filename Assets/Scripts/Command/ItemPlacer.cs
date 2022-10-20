using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPlacer : MonoBehaviour {
    public static GameObject PlaceItem(GameObject item, Vector3? position = null) {
        if (position == null) {
            EditorManager.instance.instantiatedPrefab = Instantiate(item);
            return EditorManager.instance.instantiatedPrefab;
        }

        return Instantiate(item, (Vector3)position, Quaternion.identity);
    }

    public static void RemoveItem(GameObject item) {
        GameObject.Destroy(item);
    }
}
