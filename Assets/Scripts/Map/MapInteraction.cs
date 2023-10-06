using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapInteraction : MonoBehaviour
{
    public List<GameObject> children;
    public string sceneName;
    public bool enableSceneSwitch = true;
    public bool startDisabled = true;
    private GameObject level;

    public void Start() {
        if(startDisabled) {
            Button btn = gameObject.GetComponent<Button>();
            if(btn != null) btn.interactable = false;
        }
        level = gameObject.transform.parent.gameObject;
    }

    public void parentUnlocked() {
        gameObject.GetComponent<Button>().interactable = true;
    }

    public void onClick() {
        Button btn = gameObject.GetComponent<Button>();
        SpriteState tmpState = btn.spriteState;
        tmpState.disabledSprite = btn.spriteState.selectedSprite;
        btn.spriteState = tmpState;
        foreach(Transform child in level.transform) {
            child.gameObject.GetComponent<Button>().interactable = false;
        }
        foreach(GameObject child in children) {
            child.GetComponent<MapInteraction>().parentUnlocked();
        }
        if(enableSceneSwitch)
            SceneManager.LoadScene(sceneName);
        else
            Debug.Log(sceneName + " Activated");
    }
}
