using UnityEngine;

[ExecuteInEditMode]
public class CustomImageEffect : MonoBehaviour {
    public Material m;

    private void OnRenderImage(RenderTexture src, RenderTexture dst) {
        Graphics.Blit(src, dst, m);
    }
}
