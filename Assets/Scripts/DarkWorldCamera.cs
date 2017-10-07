using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DarkWorldCamera : MonoBehaviour {
    public Material m;

    private void OnRenderImage(RenderTexture src, RenderTexture dst) {
        m.SetTexture("_DarkWorld", src);
        Graphics.Blit(src, dst);
    }
}
