using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceItemCommand : ICommand {
    GameObject objectReference;
    GameObject prefabReference;
    Vector3 position;
    Quaternion rotation;

    public PlaceItemCommand(GameObject _prefabReference) {
        this.prefabReference = _prefabReference;
    }

    public void Execute() {
        objectReference = ItemPlacer.PlaceItem(prefabReference);
    }

    public void Undo() {
        this.position = objectReference.transform.position;
        this.rotation = objectReference.transform.rotation;
        ItemPlacer.RemoveItem(objectReference);
    }

    public void Redo() {
        objectReference = ItemPlacer.PlaceItem(prefabReference, position, rotation);
    }
}
