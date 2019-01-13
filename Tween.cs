using UnityEngine;

public class Tween {
    public float startValue;
    public float destValue;
    public float duration;
    public EndAction endAction = EndAction.STOP;
    public EasingCurve curve = EasingCurve.linear;

    /// <summary>
    /// if true, then the reversal of the animation will be reversed by doing "f(1-t)" like hitting rewind on a tape,
    /// if false, then the reversal of the animation will be reversed by doing "1-f(t)" which makes the easing curve go forward but in opposite direction
    /// </summary>
    public bool reverseXorY = true;

    /// <summary>
    /// True when the tween is currently animating
    /// </summary>
    public bool Running { get => runTween; }
    /// <summary>
    /// The value you will use for animating your stuff
    /// </summary>
    public float TweenValue { get => tweenValue; }
    /// <summary>
    /// The Amount of times the animation has reached the end
    /// </summary>
    public int EndHits { get => endHits; }

    int endHits = 0;
    float tweenValue;
    bool runTween = false;
    float startTime;
    bool reverse = false;

    public enum EndAction {
        STOP,
        RESET,
        RETURN,
        LOOP,
        PINGPONG,
    }

    public Tween(float start, float dest, float duration,
    EndAction endAction = EndAction.STOP,
    EasingCurve curve = EasingCurve.linear) {
        startValue = start;
        destValue = dest;
        this.duration = duration;
        this.endAction = endAction;
        this.curve = curve;
    }

    public void StartTween(bool reverse = false) {
        endHits = 0;
        runTween = true;
        this.reverse = reverse;
        startTime = Time.time;
    }

    public void StopTween() {
        // stops the animation, no shits given
        runTween = false;
    }

    // call this function ONLY once, preferably in the Update function of your MonoBehavior
    public void UpdateTween() {
        if(runTween) {
            float t = Mathf.Min((Time.time - startTime) / duration, 1);
            // confused by this line? google "ternary operator" they are quite nifty.
            float x = reverse ? (reverseXorY ? applyCurve(1 - t) : 1 - applyCurve(t)) : applyCurve(t);
            tweenValue = Mathf.Lerp(startValue, destValue, x);
            if(t >= 1) evaluateEnd();
        }
    }

    // CREATE YOUR OWN EASING CURVE!!!!!

    // add your own curve enum
    public enum EasingCurve {
        linear,
        quadratic,
        sineWave
    }

    private const float TWO_PI = Mathf.PI * 2;

    // add your own curve functions here
    private float curveLinear(float t) { return t; }
    private float curveQuadratic(float t) { return t * t; }
    private float curveSineWave(float t) { return 0.5f*Mathf.Sin(TWO_PI*(t-0.25f))+0.5f; } // this equation naturally returns to zero

    // then setup the easing curve function with its coresponding enum
    private float applyCurve(float t) {
        switch(curve) {
            case EasingCurve.linear: return curveLinear(t);
            case EasingCurve.quadratic: return curveQuadratic(t);
            case EasingCurve.sineWave: return curveSineWave(t);
            default: return t;
        }
    }

    private void evaluateEnd() {
        endHits++;
        switch(endAction) {
            case EndAction.STOP: {
                runTween = false; break;
            }
            case EndAction.RESET: {
                tweenValue = Mathf.LerpUnclamped(startValue, destValue, reverse ? 1 : 0);
                runTween = false; break;
            }
            case EndAction.LOOP: {
                startTime = Time.time; break;
            }
            case EndAction.PINGPONG: {
                startTime = Time.time;
                reverse = !reverse; break;
            }
            case EndAction.RETURN: {
                startTime = Time.time;
                if(endHits < 2) reverse = !reverse;
                else runTween = false; break;
            }
        }
    }
}