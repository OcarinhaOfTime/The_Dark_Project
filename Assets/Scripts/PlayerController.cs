using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float speed = 10;
    public float jumpForce = 20;
    private Rigidbody2D rb2d;

    private void Start() {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        var vel = rb2d.velocity;
        vel.x = speed * Time.fixedDeltaTime * Input.GetAxis("Horizontal");
        rb2d.velocity = vel;
        if (Input.GetButtonDown("Jump")) {
            rb2d.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
        }
    }
}
