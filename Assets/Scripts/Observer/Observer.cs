using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Wants to know when another object does something interesting 
public abstract class Observer {
    public abstract void OnNotify();
}

public class InstantiatedObject1 : Observer {
    GameObject objectReference;
    InstantiatedObjectEventBase eventReference;

    public InstantiatedObject1(GameObject _objectReference, InstantiatedObjectEventBase _eventReference) {
        this.objectReference = _objectReference;
        this.eventReference = _eventReference;
    }

    public override void OnNotify() {
        GameObject spawnedEnemiesParent = GameObject.Find("Spawned Enemies");
        if (objectReference == null || spawnedEnemiesParent == null || spawnedEnemiesParent.transform.childCount < 3)
            return;

        ChangeColor(eventReference.NewObjectColor());
    }

    void ChangeColor(Color mat) {
        Renderer objectRenderer = objectReference.GetComponent<Renderer>();
        if (objectRenderer == null)
            return;

        objectRenderer.materials[0].color = mat;
    }
}

public class InstantiatedObject2 : Observer {
    GameObject objectReference;
    InstantiatedObjectEventBase eventReference;

    public InstantiatedObject2(GameObject _objectReference, InstantiatedObjectEventBase _eventReference) {
        this.objectReference = _objectReference;
        this.eventReference = _eventReference;
    }

    public override void OnNotify() {
        if (objectReference == null || GameController.instance.player == null || Vector3.Distance(GameController.instance.player.transform.position, objectReference.transform.position) > 5f)
            return;

        ChangeScale(eventReference.NewObjectScale());
    }

    void ChangeScale(Vector3 vec) {
        //objectReference.transform.position += Vector3.up * 0.5f;
        objectReference.transform.localScale = vec;
    }
}
