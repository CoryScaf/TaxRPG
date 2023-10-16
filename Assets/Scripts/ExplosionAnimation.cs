using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionAnimation : MonoBehaviour
{
    public List<Sprite> frames;
    private SpriteRenderer spriteRenderer;
    private int curFrame = 0;
    private AudioSource explosionSound;
    private float lastFrame;
    private bool finishing = false;
    // Start is called before the first frame update
    void Start()
    {
        explosionSound = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        explosionSound.Play();
        lastFrame = Time.time;
        spriteRenderer.sprite = frames[curFrame];
    }

    // Update is called once per frame
    void Update()
    {
        if(finishing) return;
        if(Time.time - lastFrame > 0.1f) {
            curFrame++;
             if(curFrame >= frames.Count) {
                StartCoroutine(destroyWhenDone());
                finishing =  true;
                return;
             }
            spriteRenderer.sprite = frames[curFrame];
        }
    }

    private IEnumerator destroyWhenDone() {
        while(explosionSound.isPlaying) {
            yield return null;
        }
        Destroy(gameObject);
    }
}
