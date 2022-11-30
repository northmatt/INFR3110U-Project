using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPlacer : MonoBehaviour {
    public static GameObject PlaceItem(GameObject item, Vector3? position = null, Quaternion? rotation = null) {
        if (position == null || rotation == null) {
            EditorController.instance.instantiatedPrefab = Instantiate(item, EditorController.instance.instantiateParent);
            return EditorController.instance.instantiatedPrefab;
        }

        return Instantiate(item, (Vector3)position, (Quaternion)rotation, EditorController.instance.instantiateParent);
    }

    public static void RemoveItem(GameObject item) {
        GameObject.Destroy(item);
    }
}
