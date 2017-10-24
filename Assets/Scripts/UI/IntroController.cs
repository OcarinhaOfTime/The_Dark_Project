using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroController : MonoBehaviour {
    public GameObject anykey;
    public GameObject menu;

    public Button cont;
    public Button newGame;
    public Button quit;

    void Start() {
        cont.onClick.AddListener(() => SceneManager.LoadScene(1));
        newGame.onClick.AddListener(() => SceneManager.LoadScene(1));
        quit.onClick.AddListener(Application.Quit);
    }

    void Update() {
        if (Input.anyKeyDown) {
            anykey.SetActive(false);
            menu.SetActive(true);
        }
    }
}
