using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HoleCamera : MonoBehaviour {
    public Material mat;

	void OnEnable () {
        var cam = GetComponent<Camera>();

        if (cam.targetTexture != null) {
            var temp = cam.targetTexture;
            cam.targetTexture = null;
            DestroyImmediate(temp);
        }

        cam.targetTexture = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 16);
        cam.targetTexture.filterMode = FilterMode.Bilinear;
        mat.SetTexture("_Mask", cam.targetTexture);
    }
}
