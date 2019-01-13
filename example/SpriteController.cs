using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour {

    public SpriteRenderer spr;

    Tween tweenPos;
    Tween tweenHue;

    // Start is called before the first frame update
    void Start() {
        tweenHue = new Tween(0, 1, 5,Tween.EndAction.LOOP ,Tween.EasingCurve.sineWave);
        tweenHue.StartTween();
        tweenPos = new Tween(transform.position.x, transform.position.x+10, 5, Tween.EndAction.LOOP, Tween.EasingCurve.sineWave);
        tweenPos.StartTween();
    }

    // Update is called once per frame
    void Update() {
        tweenHue.UpdateTween();
        tweenPos.UpdateTween();
        transform.position = new Vector3(tweenPos.TweenValue, 0, 0);
        spr.color = Color.HSVToRGB(tweenHue.TweenValue, 1, 1);
    }
}
