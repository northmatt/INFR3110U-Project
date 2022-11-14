using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class SaveLoadPlugin : MonoBehaviour
{
    public bool savePlayerPositionOnQuit;

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

    private Transform player;
    private PlayerAction playerInput;
    private string mPath, fn;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GameController.instance.inputAction;
        player = GameController.instance.player.transform;

        mPath = Application.dataPath;
        fn = mPath + "/save.txt";
        Debug.Log(fn);
        
        LoadPlayerPosition();
    }

    void OnApplicationQuit()
    {
        StartSaving(fn);
        if (savePlayerPositionOnQuit)
        {
            WritePosition(0, player.position);
            Debug.Log("Saved player to " + player.position);
        }
        // SaveEnemies();
        EndSaving();
    }

    void LoadPlayerPosition()
    {
        ReadData(fn);
        if (GetLength() <= 0 || GetNthType(0) != 0)
        {
            Debug.Log("Player's location not stored");
        }
        else
        {
            player.transform.position = GetNthPosition(0);
            Debug.Log("Loaded player to " + player.transform.position);
        }
    }

    void SaveEnemies()
    {
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
