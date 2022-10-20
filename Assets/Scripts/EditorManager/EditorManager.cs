using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EditorManager : MonoBehaviour 
{
    public static EditorManager instance;

    public PlayerAction inputAction;

    public Camera mainCam;
    public Camera editorCam;

    public bool editorMode = false;

    Vector3 mousePos;
    public GameObject prefab1;
    public GameObject prefab2;
    public GameObject item;
    public bool instantiated = false;

    // Will send notifications that something has happened to whoever is interested
    Subject subject = new Subject();

    // Command
    ICommand command;

    // Start is called before the first frame update
    void Start() 
    {
        if (instance != null) {
            Destroy(this.gameObject);
            return;
        }

        instance = this;

        inputAction = GameController.instance.playerInput;

        inputAction.Editor.EditorMode.performed += cntxt => ToggleEditorMode();

        inputAction.Editor.AddItem1.performed += cntxt => AddItem(1);
        inputAction.Editor.AddItem2.performed += cntxt => AddItem(2);
        inputAction.Editor.DropItem.performed += cntxt => DropItem();

        mainCam.enabled = true;
        editorCam.enabled = false;
    }

    // Update is called once per frame
    void Update() 
    {
        if (instantiated)
        {
            //Pretty sure there is a better built in way to read mouse pos, maybe not cant remember
            mousePos = Mouse.current.position.ReadValue();
            mousePos = new Vector3(mousePos.x, mousePos.y, 5f);

            item.transform.position = editorCam.ScreenToWorldPoint(mousePos);
        }
    }

    public void ToggleEditorMode()
    {
        if (editorMode != GameController.instance.gamePaused)
            return;

        editorMode = !editorMode;

        mainCam.enabled = !editorMode;
        editorCam.enabled = editorMode;

        //GameController.instance.editorUI.enabled = editorMode;
        GameController.instance.DoPause(editorMode);
    }

    public void AddItem(int itemId)
    {
        if (editorMode && !instantiated) 
        {
            switch (itemId)
            {
                case 1:
                    item = Instantiate(prefab1);
                    //Create boxes that can observe events and give them an event to do
                    SpikeBall spike1 = new SpikeBall(item, new GreenMat());
                    //Add the boxes to the list of objects waiting for something to happen
                    subject.AddObserver(spike1);
                    break;
                case 2:
                    item = Instantiate(prefab2);
                    //Create boxes that can observe events and give them an event to do
                    SpikeBall spike2 = new SpikeBall(item, new YellowMat());
                    //Add the boxes to the list of objects waiting for something to happen
                    subject.AddObserver(spike2);
                    break;
                default:
                    break;
            }
            subject.Notify();
            instantiated = true;
        }
    }

    public void DropItem() 
    {
        if (editorMode && instantiated) {
            item.GetComponent<Rigidbody>().useGravity = true;
            item.GetComponent<Collider>().enabled = true;

            // Add item transform to items list
            command = new PlaceItemCommand(item.transform.position, item.transform);
            CommandInvoker.AddCommand(command);

            instantiated = false;
        }
    }

}
