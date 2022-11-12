using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindController : MonoBehaviour {
    private InputActionRebindingExtensions.RebindingOperation rebindOperation;

    public void OnEnable() {
        string rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            GameController.instance.inputAction.asset.LoadBindingOverridesFromJson(rebinds);
    }

    public void OnDisable() {
        string rebinds = GameController.instance.inputAction.asset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }

    public void StartRebind(InputAction affectedInputAction, int bindingIndex) {
        if (rebindOperation != null)
            rebindOperation.Cancel();

        if (affectedInputAction.bindings[bindingIndex].isComposite)
            return;

        affectedInputAction.Disable();

        rebindOperation = affectedInputAction.PerformInteractiveRebinding(bindingIndex).OnCancel(operation => EndRebind(affectedInputAction)).OnComplete(operation => EndRebind(affectedInputAction));

        rebindOperation.Start();
    }

    private void EndRebind(InputAction affectedInputAction) {
        rebindOperation.Dispose();
        rebindOperation = null;

        affectedInputAction.Enable();
    }

    public void ResetBindings() {
        foreach (InputActionMap map in GameController.instance.inputAction.asset.actionMaps) {
            map.RemoveAllBindingOverrides();
            PlayerPrefs.DeleteKey("rebinds");
        }
    }
}
