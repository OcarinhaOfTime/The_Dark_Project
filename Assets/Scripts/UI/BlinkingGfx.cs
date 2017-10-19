using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingGfx : MonoBehaviour {
    public float delay = 2;
    public float rate = .5f;

    private float timer = 0;
    private Graphic gfx;

    private void Start() {
        gfx = GetComponent<Graphic>();
    }

    private void Update() {
        while(timer < delay) {
            timer += Time.deltaTime;
            return;
        }

        var col = gfx.color;
        var t = Mathf.PingPong(Time.time - timer, 1);
        col.a = t * t;
        gfx.color = col;
    }
}
