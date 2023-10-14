using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFade : MonoBehaviour
{
    public AnimationCurve fadeCurve = new AnimationCurve(new Keyframe(0, 1),
        new Keyframe(0.5f, 0.5f, -1.5f, -1.5f), new Keyframe(1, 0));

    public bool startFadedOut = false;
    public Color fadeColor = Color.black;

    private float alpha = 0.0f;
    private Texture2D texture;
    private int dir = 0;
    private float time = 0.0f;
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        if(startFadedOut) 
            alpha = 1.0f;        
        texture = new Texture2D(1,1);
        texture.SetPixel(0, 0, new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha));
        texture.Apply();
    }

    public void OnGUI() {
        if(alpha > 0.0f) GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);
        if(dir != 0) {
            time += dir * Time.unscaledDeltaTime * speed;
            alpha = fadeCurve.Evaluate(time);
            texture.SetPixel(0, 0, new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha));
            texture.Apply();
            if(alpha <= 0.0f || alpha >= 1.0f) dir = 0;
        }
    }

    public void fadeOut(float speedScale) {
        if(dir == 0) {
            if(alpha <= 0.0f) {
                alpha = 0.0f;
                time = 1.0f;
                dir = -1;
                speed = speedScale;
            }
            else {
                Debug.Log("Tried to fade out while already faded out.");
            }
        }
        else {
            Debug.Log("Tried to fade while still fading.");
        }
    }

    public void fadeIn(float speedScale) {
        if(dir == 0) {
            if(alpha >= 1.0f) {
                alpha = 1.0f;
                time = 0.0f;
                dir = 1;
                speed = speedScale;
            }
            else {
                Debug.Log("Tried to fade in while already faded in.");
            }
        }
        else {
            Debug.Log("Tried to fade while still fading.");
        }
    }
}
