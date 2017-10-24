using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleFader : MonoBehaviour {
    public float duration = 1;
    public Material m;
    
	void Start () {
        this.LerpRoutine(duration, (t) => m.SetFloat("_Cutoff", 1 - t));
	}
}
