using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DarkWorldCamera : MonoBehaviour {
    public Material m;

    private void OnEnable() {
        var cam = GetComponent<Camera>();

        if (cam.targetTexture != null) {
            var temp = cam.targetTexture;
            cam.targetTexture = null;
            DestroyImmediate(temp);
        }

        cam.targetTexture = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 16);
        cam.targetTexture.filterMode = FilterMode.Bilinear;
        m.SetTexture("_DarkWorld", cam.targetTexture);
    }
}
