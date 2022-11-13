using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindController : MonoBehaviour {
    [SerializeField] private GameObject RebindingPanelOverlay;
    [SerializeField] private Transform rebindPanelArray;
    [SerializeField] private GameObject rebindButtonPanelPrefab;

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

    private void Start() {
        PlayerAction inputAction = GameController.instance.inputAction;

        InstantiateButtonPrefab("Forwards", inputAction.Player.Move, 1);
        InstantiateButtonPrefab("Backwards", inputAction.Player.Move, 2);
        InstantiateButtonPrefab("Left", inputAction.Player.Move, 3);
        InstantiateButtonPrefab("Right", inputAction.Player.Move, 4);
        InstantiateButtonPrefab("Jump", inputAction.Player.Jump, 0);
        InstantiateButtonPrefab("Sprint", inputAction.Player.Sprint, 0);
        InstantiateButtonPrefab("Crouch", inputAction.Player.Crouch, 0);
        InstantiateButtonPrefab("Menu", inputAction.Player.Menu, 0);
    }

    public void InstantiateButtonPrefab(string buttonName, InputAction affectedInputAction, int bindingIndex) {
        GameObject panelObject = Instantiate(rebindButtonPanelPrefab, rebindPanelArray);

        Text panelText = panelObject.transform.GetChild(0).GetComponent<Text>();
        Button rebindButton = panelObject.transform.GetChild(1).GetComponent<Button>();
        Text rebindButtonText = rebindButton.transform.GetChild(0).GetComponent<Text>();
        Button resetButton = panelObject.transform.GetChild(2).GetComponent<Button>();

        panelText.text = buttonName + ": ";
        rebindButtonText.text = SanatizeString(affectedInputAction.bindings[bindingIndex].effectivePath);

        rebindButton.onClick.AddListener(delegate { StartRebind(affectedInputAction, bindingIndex, rebindButtonText); });
        resetButton.onClick.AddListener(delegate { ResetBinding(affectedInputAction, bindingIndex, rebindButtonText); });
    }

    public void StartRebind(InputAction affectedInputAction, int bindingIndex, Text UItext) {
        if (rebindOperation != null)
            rebindOperation.Cancel();

        if (affectedInputAction.bindings[bindingIndex].isComposite)
            return;

        RebindingPanelOverlay.SetActive(true);
        affectedInputAction.Disable();

        rebindOperation = affectedInputAction.PerformInteractiveRebinding(bindingIndex)
            .OnCancel(operation => EndRebind(affectedInputAction, bindingIndex, UItext))
            .OnComplete(operation => EndRebind(affectedInputAction, bindingIndex, UItext));

        rebindOperation.Start();
    }

    private void EndRebind(InputAction affectedInputAction, int bindingIndex, Text UItext) {
        rebindOperation.Dispose();
        rebindOperation = null;

        UItext.text = SanatizeString(affectedInputAction.bindings[bindingIndex].effectivePath);

        affectedInputAction.Enable();
        RebindingPanelOverlay.SetActive(false);
    }

    public void ResetBinding(InputAction affectedInputAction, int bindingIndex, Text UItext) {
        affectedInputAction.RemoveAllBindingOverrides();

        UItext.text = SanatizeString(affectedInputAction.bindings[bindingIndex].effectivePath);
    }

    public void ResetBindings() {
        //Deletes saved rebinds, Removes all rebinds from PlayerAction
        PlayerPrefs.DeleteKey("rebinds");
        foreach (InputActionMap map in GameController.instance.inputAction.asset.actionMaps)
            map.RemoveAllBindingOverrides();

        //What a terrible way to update UI.... I like it :D
        foreach (Button curButton in rebindPanelArray.GetComponentsInChildren<Button>())
            if (curButton.gameObject.name == "Reset Button")
                curButton.onClick.Invoke();
    }

    private string SanatizeString(string input) {
        return input.ToLower().Replace("<keyboard>/", "").Replace("<mouse>/", "").Replace("scroll/", "scroll ").ToUpper();
    }
}
