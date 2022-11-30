using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class SaveLoadPlugin : MonoBehaviour
{
    public bool savePlayerPositionOnQuit;
    public bool savePlayerStatsOnQuit;

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
    private string mPath, fn;

    // Start is called before the first frame update
    void Start() {
        player = GameController.instance.player.transform;

        mPath = Application.dataPath;
        fn = mPath + "/save.txt";
        
        LoadPlayerData();
    }

    void OnApplicationQuit() {
        StartSaving(fn);

        if (savePlayerPositionOnQuit)
            WritePosition(0, player.position);

        if (savePlayerStatsOnQuit)
            WritePosition(1, new Vector3(GameController.instance.player.health, 0f, GameController.instance.player.GetCrouch() ? 1f : 0f));

        // SaveEnemies();
        EndSaving();
    }

    void LoadPlayerData()
    {
        ReadData(fn);
        if (GetLength() <= 0)
            return;

        if (GetNthType(0) == 0)
            player.transform.position = GetNthPosition(0);

        if (GetNthType(1) == 1) {
            GameController.instance.player.health = GetNthPosition(1).x;

            if (GetNthPosition(1).z == 1f)
                GameController.instance.player.ToggleCrouch(true);
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
