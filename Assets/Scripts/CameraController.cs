using System;
using UnityEngine;
//Used in MM2/MM from JayPEG/JayPOG, am dev of it

public class CameraController : MonoBehaviour {
    public Transform following;
    public Vector3 orginOffset = Vector3.zero;
    public Vector3 orginOffsetWithRotation = Vector3.zero;
    public Vector2 clipPlaneOffset = new Vector2(0.2f, 0.1f);
    public float distance = 2f;
    public float sensitivity = 1f;
    public Vector2 rotXMinMax = new Vector2(310f, 450f);

    private PlayerAction playerInput;
    private Camera cam;
    private Vector2 rotationInput = Vector2.zero;
    private Vector3 orginOffsetWithRotationCurrent = Vector3.zero;
    private Vector3 orginOffsetWithRotationVel = Vector3.zero;

    // Start is called before the first frame update
    void Start() {
        playerInput = GameController.instance.playerInput;

        playerInput.Player.Look.performed += cntxt => rotationInput = cntxt.ReadValue<Vector2>();
        playerInput.Player.Look.canceled += cntxt => rotationInput = Vector2.zero;

        cam = GetComponent<Camera>();
        orginOffsetWithRotationCurrent = transform.rotation * orginOffsetWithRotation;
    }

    // LateUpdate is called once per frame at the end of everything
    void LateUpdate() {
        if (following == null)
            return;

        if (!GameController.instance.gamePaused || GameController.instance.CursorLocked) {
            float rotMod = transform.rotation.eulerAngles.x <= 90f ? transform.rotation.eulerAngles.x + 360f : transform.rotation.eulerAngles.x;
            transform.rotation = Quaternion.Euler(Mathf.Clamp(rotMod - rotationInput.y * sensitivity, rotXMinMax.x, rotXMinMax.y),
                transform.rotation.eulerAngles.y + rotationInput.x * sensitivity, 0);
        }

        //Info provided is Camera FoV/Aspect Ratio/Near Clip Plane, get topleft of Near Clip Plane's local position in rectagular cordinates
        float calc1 = Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * cam.nearClipPlane + clipPlaneOffset.x;    //see if this can be called only if smth changes to save on calcs
        float calc2 = calc1 * cam.aspect + clipPlaneOffset.x;
        float calc3 = cam.nearClipPlane - clipPlaneOffset.y;
        float calcDist = distance;
        int layer = ~(1 << LayerMask.NameToLayer("Player"));
        RaycastHit ray;

        Vector3 orginOfAll = following.position + orginOffset;
        Vector3 orginOffsetWithRotationTemp = transform.rotation * orginOffsetWithRotation;

        foreach (int dir in new int[3] { 1, -1, 0 })
            if (dir == 0 || !Physics.Linecast(orginOfAll, orginOfAll + (orginOffsetWithRotationTemp + transform.right * calc2 * 0.5f) * dir, out ray, layer)) {
                //remove local rotation from offset and apply direction modifier, local rotaion was originally applied to not do the math multiple times
                orginOffsetWithRotationTemp = orginOffsetWithRotation * dir;
                break;
            }

        orginOffsetWithRotationCurrent = Vector3.SmoothDamp(orginOffsetWithRotationCurrent, orginOffsetWithRotationTemp, ref orginOffsetWithRotationVel, 0.1f, 5f, Time.fixedDeltaTime);
        orginOfAll += transform.rotation * orginOffsetWithRotationCurrent;

        //Checking from viewing orgin to each corner of camera's "sensor" for any obstruction
        foreach (Vector3 vec in new Vector3[4] { new Vector3(calc2, calc1, calc3), new Vector3(-calc2, calc1, calc3), new Vector3(calc2, -calc1, calc3), new Vector3(-calc2, -calc1, calc3) })
            if (Physics.Linecast(orginOfAll, orginOfAll + transform.rotation * (Vector3.back * distance + vec), out ray, layer) && ray.distance < calcDist)
                calcDist = ray.distance;

        following.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        transform.position = orginOfAll - transform.forward * (Mathf.Cos(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * calcDist);
    }
}
