using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class SaveLoadPlugin : MonoBehaviour
{
    [DllImport("SaveLoadPlugin")]
    private static extern void StartSaving(string fileName);
    [DllImport("SaveLoadPlugin")]
    private static extern void WritePosition(int type, Vector3 position);
    [DllImport("SaveLoadPlugin")]
    private static extern void EndSaving();

    [DllImport("SaveLoadPlugin")]
    private static extern void ReadData(string fileName);
    [DllImport("SaveLoadPlugin")]
    private static extern Vector3 GetNthPosition(int n);
    [DllImport("SaveLoadPlugin")]
    private static extern int GetNthType(int n);
    [DllImport("SaveLoadPlugin")]
    private static extern int GetLength();

    private PlayerAction playerInput;
    private string mPath, fn;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GameController.instance.inputAction;
        playerInput.Editor.Save.performed += cntxt => SaveEnemies();
        playerInput.Editor.Load.performed += cntxt => LoadEnemies();

        mPath = Application.dataPath;
        fn = mPath + "/save.txt";
        Debug.Log(fn);
    }

    void SaveEnemies()
    {
        StartSaving(fn);
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (obj.name.Contains("Enemy 1"))
            {
                WritePosition(1, obj.transform.position);
            }
            else if (obj.name.Contains("Enemy 2"))
            {
                WritePosition(2, obj.transform.position);
            }
            else
            {
                WritePosition(0, obj.transform.position);
            }
        }
        EndSaving();
    }

    void LoadEnemies()
    {
        ReadData(fn);
        for (int i = 0; i < GetLength(); i++)
        {
            Debug.Log("Enemy type " + GetNthType(i) + " at " + GetNthPosition(i));
        }
    }
}
