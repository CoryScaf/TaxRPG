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
    private GameObject parent;
    private bool isSelected = false;
    private bool defaultState = true;
    private Sprite disabledSprite = null;

    public void Start() {
        if(startDisabled) {
            Button btn = gameObject.GetComponent<Button>();
            if(btn != null) btn.interactable = false;
        }
    }

    public void parentUnlocked(GameObject newParent) {
        parent = newParent;
        gameObject.GetComponent<Button>().interactable = true;
        defaultState = false;
    }

    public void lockChildren() {
        foreach(GameObject child in children) {
            child.GetComponent<Button>().interactable = false;
        }
    }

    public void onClick() {
        isSelected = true;
        defaultState = false;
        Button btn = gameObject.GetComponent<Button>();
        SpriteState tmpState = btn.spriteState;
        tmpState.disabledSprite = btn.spriteState.selectedSprite;
        disabledSprite = btn.spriteState.disabledSprite;
        btn.spriteState = tmpState;
        if(parent != null)
            parent.GetComponent<MapInteraction>().lockChildren();
        else
            btn.interactable = false;
        foreach(GameObject child in children) {
            child.GetComponent<MapInteraction>().parentUnlocked(gameObject);
        }
        if(enableSceneSwitch)
            SceneManager.LoadScene(sceneName);
        else
            Debug.Log(sceneName + " Activated");
    }

    public void resetTree() {
        if(defaultState) return;
        Button btn = gameObject.GetComponent<Button>();
        if(isSelected) {
            isSelected = false;
            SpriteState tmpState = btn.spriteState;
            tmpState.disabledSprite = disabledSprite;
            btn.spriteState = tmpState;
        }
        if(startDisabled) {
            btn.interactable = false;
        }
        else {
            btn.interactable = true;
        }
        foreach(GameObject child in children) {
            child.GetComponent<MapInteraction>().resetTree();
        }
        defaultState = true;
    }
}
