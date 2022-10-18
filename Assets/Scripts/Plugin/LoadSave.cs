using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSave : MonoBehaviour {
    string text;

    void Start() {
        // text = Resources.Load(Application.dataPath + "/save.txt") as string;
        TextAsset mydata = Resources.Load(Application.dataPath + "/save") as TextAsset;
        Debug.Log(mydata);
    }
}
