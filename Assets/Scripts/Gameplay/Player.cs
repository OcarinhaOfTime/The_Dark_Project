using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    private PlayerMovement controller;
    private Animator animator;

    private void Start() {
        controller = GetComponent<PlayerMovement>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update() {
        animator.SetBool("walk", Mathf.Abs(controller.velocity.x) > .1f);
    }
}
