using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EditorManager : MonoBehaviour {
    public static EditorManager instance;

    public PlayerAction inputAction;

    public Camera mainCam;
    public Camera editorCam;
    public GameObject[] editorPrefabs;
    public GameObject instantiatedPrefab = null;
    public bool editorMode = false;
    public float editorMovement = 0f;

    private Vector3 mousePos = Vector3.zero;

    // Start is called before the first frame update
    void Start() {
        if (instance != null) {
            Destroy(this.gameObject);
            return;
        }

        instance = this;

        inputAction = GameController.instance.inputAction;

        inputAction.Player.EditorMode.performed += cntxt => ToggleEditorMode();
        inputAction.Editor.EditorMode.performed += cntxt => ToggleEditorMode();
        inputAction.Editor.AddItem1.performed += cntxt => AddItem(0);
        inputAction.Editor.AddItem2.performed += cntxt => AddItem(1);
        inputAction.Editor.DropItem.performed += cntxt => DropItem();

        mainCam.enabled = true;
        editorCam.enabled = false;

        inputAction.Editor.Disable();
    }

    // Update is called once per frame
    void Update() {
        if (instantiatedPrefab != null) {
            //Pretty sure there is a better built in way to read mouse pos, maybe not cant remember
            mousePos = Mouse.current.position.ReadValue();
            mousePos = new Vector3(mousePos.x, mousePos.y, 5f);

            instantiatedPrefab.transform.position = editorCam.ScreenToWorldPoint(mousePos);
        }

        editorCam.transform.position += Time.unscaledDeltaTime * editorMovement * inputAction.Editor.Move.ReadValue<Vector3>();
    }

    public void ToggleEditorMode() {
        if (editorMode != GameController.instance.gamePaused)
            return;

        editorMode = !editorMode;

        mainCam.enabled = !editorMode;
        editorCam.enabled = editorMode;

        //Why cant I just do inputAction.Actionmap.Enabled(bool) hnnngggggg
        if (editorMode) {
            inputAction.Editor.Enable();
            inputAction.Player.Disable();
        }
        else {
            inputAction.Editor.Disable();
            inputAction.Player.Enable();
        }

        //GameController.instance.editorUI.enabled = editorMode;
        GameController.instance.DoPause(editorMode);
    }

    public void AddItem(int itemId) {
        if (editorMode && instantiatedPrefab == null)
            CommandInvoker.AddCommand(new PlaceItemCommand(editorPrefabs[itemId]));
    }

    public void DropItem() {
        if (editorMode && instantiatedPrefab != null) {
            instantiatedPrefab.GetComponent<Collider>().enabled = true;
            instantiatedPrefab = null;
        }
    }

}
