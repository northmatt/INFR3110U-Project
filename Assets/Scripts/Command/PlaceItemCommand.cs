using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceItemCommand : ICommand {
    Vector3 position;
    Transform item;

    public PlaceItemCommand(Vector3 _position, Transform _item) {
        this.position = _position;
        this.item = _item;
    }

    public void Execute() {
        ItemPlacer.PlaceItem(item);
    }

    public void Undo() {
        ItemPlacer.RemoveItem(position);
    }

    public void Redo() {
        ItemPlacer.RedoItem(position, item);
    }
}
