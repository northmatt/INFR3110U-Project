using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour 
{
    public static GameController instance;

    public PlayerAction playerInput;
    public GameObject player;
    public Canvas editorUI;
    public bool gamePaused = false;
    public bool CursorLocked = false;
    public byte collectables = 0;

    private void OnEnable() 
    {
        playerInput.Enable();
    }

    private void OnDisable() 
    {
        playerInput.Disable();
    }

    private void Awake() 
    {
        //Not ideal to init singleton in Awake(), but needs to run before any script runs Start()
        if (instance != null) 
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        playerInput = new PlayerAction();
    }

    private void Start() 
    {
        //Need to make it find player on scene reload, also cameras on EditorManager (wrote this weeks ago but idk why I need to do EditorManager given it reloads with scene, should look into it more)
        //DontDestroyOnLoad(this.gameObject);

        player = GameObject.FindGameObjectWithTag("Player");

        CursorHidden(true);

        playerInput.Player.Menu.performed += cntxt => TogglePauseMenu();
    }

    public void DoPause(bool isPaused) 
    {
        gamePaused = isPaused;
        CursorHidden(!gamePaused);

        Time.timeScale = isPaused ? 0 : 1;
    }

    void CursorHidden(bool isHidden)
    {
        CursorLocked = isHidden;
        Cursor.lockState = CursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !CursorLocked;
    }

    void TogglePauseMenu() 
    {
        if (EditorManager.instance.editorMode)
            return;

        DoPause(!gamePaused);
    }
}
