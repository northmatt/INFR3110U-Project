using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public static GameController instance;

    public PlayerAction inputAction;
    public GameObject player;
    public Canvas editorUI;
    //added for key rebinding
    public Canvas pauseUI;
    public bool gamePaused = false;
    public bool CursorLocked = false;
    public byte collectables = 0;

    private void OnEnable() {
        inputAction.Enable();
    }

    private void OnDisable() {
        inputAction.Disable();
    }

    private void Awake() {
        //Not ideal to init singleton in Awake(), but needs to run before any script runs Start()
        if (instance != null) {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        inputAction = new PlayerAction();
    }

    private void Start() {
        //Need to make it find player on scene reload, also cameras on EditorManager
        //DontDestroyOnLoad(this.gameObject);

        player = GameObject.FindGameObjectWithTag("Player");

        CursorHidden(true);

        inputAction.Player.Menu.performed += cntxt => TogglePauseMenu();
    }

    public void DoPause(bool isPaused) {
        gamePaused = isPaused;
        CursorHidden(!gamePaused);

        Time.timeScale = isPaused ? 0 : 1;

        pauseUI.enabled = isPaused;
    }

    void CursorHidden(bool isHidden) {
        CursorLocked = isHidden;
        Cursor.lockState = CursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !CursorLocked;
    }

    void TogglePauseMenu() {
        if (EditorController.instance.editorMode)
            //    //added key rebinding ui toggle
            //    //update: this method doesnt work, the canvas didnt show up
            //{
            //    editorUI.enabled = true;
            //}
            //else
            //{
            //    editorUI.enabled = false;
            //}
            return;
           

        DoPause(!gamePaused);
    }
}
