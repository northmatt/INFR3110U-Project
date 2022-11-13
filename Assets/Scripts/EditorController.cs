using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EditorController : MonoBehaviour {
    public static EditorController instance;

    public PlayerAction inputAction;

    public Camera mainCam;
    public Camera editorCam;
    public GameObject[] editorPrefabs;
    public Transform instantiateParent = null;
    public GameObject instantiatedPrefab = null;
    public bool editorMode = false;
    public float editorMovement = 0f;

    private Vector3 mousePos = Vector3.zero;
    [HideInInspector] public Subject observers = new Subject();

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
    }

    // Update is called once per frame
    void Update() {
        if (instantiatedPrefab != null) {
            Ray camRay = editorCam.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit rayHit;

            if (Physics.Raycast(camRay, out rayHit)) {
                mousePos = rayHit.point;
            }
            else {
                mousePos = Mouse.current.position.ReadValue();
                mousePos = editorCam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 5f));
            }

            instantiatedPrefab.transform.position = mousePos;
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

    //High level AddItem call
    public void AddItem(int itemId) {
        if (editorMode && instantiatedPrefab == null)
            CommandInvoker.AddCommand(new PlaceItemCommand(editorPrefabs[itemId]));
    }

    public void DropItem() {
        if (!editorMode || instantiatedPrefab == null)
            return;

        if (Random.value > 0.5f)
            observers.AddObserver(new InstantiatedObject1(instantiatedPrefab, new InstantiatedObjectEvent1()));
        else
            observers.AddObserver(new InstantiatedObject1(instantiatedPrefab, new InstantiatedObjectEvent2()));

        instantiatedPrefab = null;
    }

}
