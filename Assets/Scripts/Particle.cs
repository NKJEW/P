using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour {
    float startSize;

    public void Setup(float lifeTime, Sprite sprite, Color color) {
        SpriteRenderer srend = gameObject.AddComponent<SpriteRenderer>();
        srend.sprite = sprite;
        srend.color = color;
        startSize = transform.localScale.x;

        StartCoroutine(Shrink(lifeTime));
    }

    IEnumerator Shrink(float time) {
        float p = 0f;
        while (p < 1f) {
            transform.localScale = Vector3.Lerp(Vector3.one * startSize, Vector3.zero, p);
            yield return new WaitForEndOfFrame();
            p += Time.deltaTime / time;
        }

        Destroy(gameObject);
    }
}
