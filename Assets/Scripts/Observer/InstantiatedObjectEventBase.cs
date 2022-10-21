using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Events
public abstract class InstantiatedObjectEventBase {
    public abstract Color NewObjectColor();
    public abstract Vector3 NewObjectScale();
}

public class InstantiatedObjectEvent1 : InstantiatedObjectEventBase {
    public override Color NewObjectColor() {
        return Color.yellow;
    }

    public override Vector3 NewObjectScale() {
        return Vector3.one * 80f;
    }
}

public class InstantiatedObjectEvent2 : InstantiatedObjectEventBase {
    public override Color NewObjectColor() {
        return Color.green;
    }

    public override Vector3 NewObjectScale() {
        return Vector3.one * 40f;
    }
}
