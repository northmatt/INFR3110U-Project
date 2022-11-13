using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public static GameController instance;

    public PlayerAction inputAction;
    public CharController player;
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
        //Not ideal to init singleton in Awake(), but needs to run before any script runs Start()/OnEnable()
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

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharController>();

        CursorHidden(true);

        inputAction.Player.Menu.performed += cntxt => TogglePauseMenu();

        inputAction.Editor.Disable();
        inputAction.UI.Disable();
    }

    public void DoPause(bool isPaused) {
        gamePaused = isPaused;
        CursorHidden(!gamePaused);

        Time.timeScale = isPaused ? 0 : 1;
    }

    private void CursorHidden(bool isHidden) {
        CursorLocked = isHidden;
        Cursor.lockState = CursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !CursorLocked;
    }

    private void TogglePauseMenu() {
        //Dont run pause code if in editor mode
        if (EditorController.instance.editorMode)
            return;   

        DoPause(!gamePaused);
        pauseUI.enabled = gamePaused;
    }

    public void QuitGame() {
        Application.Quit();
    }
}
