using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform target;

    private void LateUpdate() {
        var pos = transform.position;
        pos.x = target.position.x;
        transform.position = pos;
    }
}
