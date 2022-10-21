﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour {
    public float moveForce = 0f;
    public float moveForceSprintMultiplier = 0f;
    public float jumpForce = 0f;
    public float crouchSize = 0f;
    public float groundedDot = 0.7f;
    public float gravMult = 1f;

    public GameObject projectile;
    public Transform projectilePos;

    private PlayerAction inputAction;
    private Rigidbody rBody;
    private Collider coll;
    private List<GameObject> groundedObjects = new List<GameObject>();
    private Vector2 inputs = Vector2.zero;
    private byte jump = 0;
    private bool grounded = false;
    private bool sprint = false;
    private bool crouch = false;

    private void Start() {
        inputAction = GameController.instance.inputAction;

        inputAction.Player.Move.performed += cntxt => inputs = cntxt.ReadValue<Vector2>();
        inputAction.Player.Move.canceled += cntxt => inputs = Vector2.zero;

        inputAction.Player.Jump.performed += cntxt => TryJump();

        inputAction.Player.Sprint.performed += cntxt => sprint = true;
        inputAction.Player.Sprint.canceled += cntxt => sprint = false;

        inputAction.Player.Crouch.performed += cntxt => ToggleCrouch();

        //inputAction.Player.Shoot.performed += cntxt => Shoot();

        rBody = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
    }

    private void FixedUpdate() {
        grounded = groundedObjects.Count > 0;

        float currentMoveForce = Time.fixedDeltaTime * moveForce * (sprint ? moveForceSprintMultiplier : 1f);
        rBody.AddForce(currentMoveForce * inputs.x * transform.right + currentMoveForce * inputs.y * transform.forward);

        if (jump == 4) {
            rBody.AddForce(0f, jumpForce, 0f, ForceMode.VelocityChange);
            --jump;
        }

        //reduce jump cooldown if grounded
        if (jump > 0 && grounded)
            --jump;

        //Add downwards force if ungrounded (RB3D has drag)
        if (!grounded)
            rBody.AddForce(Physics.gravity * gravMult, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision) {
        CheckCollisionNormal(collision);
    }

    private void OnCollisionStay(Collision collision) {
        CheckCollisionNormal(collision);
    }

    private void OnCollisionExit(Collision collision) {
        //Takes a few frames to call OnColExit
        groundedObjects.Remove(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.gameObject.CompareTag("Finish"))
            return;

        ++GameController.instance.collectables;
        Destroy(other.gameObject);
    }

    private void CheckCollisionNormal(Collision collision) {
        //upto 64 contact points, unlikely to ever need >20. List has dynamic scaling
        //List<ContactPoint> contactPoints = new List<ContactPoint>();
        ContactPoint[] contactPoints = new ContactPoint[20];
        int contactPointsCount = collision.GetContacts(contactPoints);

        //loop through all contacts and compare contact normal to a specific range
        for (int i = 0; i < contactPointsCount; ++i)
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > groundedDot) {
                if (!groundedObjects.Contains(collision.gameObject))
                    groundedObjects.Add(collision.gameObject);

                return;
            }

        groundedObjects.Remove(collision.gameObject);
    }

    private void TryJump() {
        if (jump == 0 && grounded)
            jump = 4;
    }

    private void Shoot() {
        if (EditorController.instance.editorMode)
            return;

        Rigidbody bulletRb = Instantiate(projectile, projectilePos.transform.position, Quaternion.identity).GetComponent<Rigidbody>();
        bulletRb.AddForce(transform.forward * 32f + transform.up * 1f, ForceMode.Impulse);
    }

    //Should prolly have an FSM at this point, crouchSize should be done by anim
    private void ToggleCrouch() {
        Vector3 startCheck = transform.position - (coll.bounds.extents.y - coll.bounds.extents.z - 0.02f) * Vector3.up;
        Vector3 endCheck = transform.position + (coll.bounds.size.y / crouchSize - coll.bounds.size.z) * Vector3.up;
        if (crouch && Physics.CheckCapsule(startCheck, endCheck, coll.bounds.extents.z, ~(1 << LayerMask.NameToLayer("Player")), QueryTriggerInteraction.Ignore))
            return;

        crouch = !crouch;

        transform.localScale = new Vector3(1f, crouch ? crouchSize : 1f, 1f);
        transform.position -= new Vector3(0f, crouch ? coll.bounds.size.y * crouchSize * 0.5f : coll.bounds.size.y * crouchSize * -0.5f, 0f);
    }
}
