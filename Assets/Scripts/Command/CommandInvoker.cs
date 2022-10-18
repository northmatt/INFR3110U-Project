using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandInvoker : MonoBehaviour {
    PlayerAction inputAction;

    static Queue<ICommand> commandBuffer = new Queue<ICommand>();
    static List<ICommand> commandHistory = new List<ICommand>();

    static int counter = 0;

    private void Start() {
        inputAction = GameController.instance.playerInput;

        inputAction.Editor.Undo.performed += cntxt => UndoCommand();
        inputAction.Editor.Redo.performed += cntxt => RedoCommand();
    }

    private void Update() {
        if (commandBuffer.Count > 0) {
            ICommand e = commandBuffer.Dequeue();
            e.Execute();

            commandHistory.Add(e);
            ++counter;
            Debug.Log("Command History: " + commandHistory.Count);
        }
    }

    public static void AddCommand(ICommand command) {
        //If there are future actions in the history, remove them
        while (commandHistory.Count > counter)
            commandHistory.RemoveAt(counter);

        commandBuffer.Enqueue(command);
    }

    public static void UndoCommand() {
        if (commandBuffer.Count > 0 || counter <= 0)
            return;

        --counter;
        commandHistory[counter].Undo();
    }

    public void RedoCommand() {
        if (commandBuffer.Count > 0 || counter >= commandHistory.Count)
            return;

        commandHistory[counter].Redo();
        ++counter;
    }
}