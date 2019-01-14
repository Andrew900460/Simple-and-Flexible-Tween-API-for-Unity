// Unity Tween API
// v1.1
// by Andrew900460

// Can now tween common unity data types (float, Vector2, Vector3, Color, Quaternion)
// "Tween" animates a float byu default, where Tween<Vector3> animates a vector
// Modified some of the existing easing curves and added a reverse curve for quadratic
// Added regions to organise things
// Tween enums are now outside of the class at the bottom of the file
// The easing curve functions are also now at bottom of class

using UnityEngine;

public class Tween<T> {
    #region public interface
    public T startValue;
    public T destValue;
    public float duration;
    public TweenEndAction endAction = TweenEndAction.STOP;
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
    public T TweenValue { get => tweenValue; }
    /// <summary>
    /// The Amount of times the animation has reached the end
    /// </summary>
    public int EndHits { get => endHits; }
    #endregion
    #region private interface
    int endHits = 0;
    T tweenValue;
    bool runTween = false;
    float startTime;
    bool reverse = false;
    #endregion
    
    #region constructors
    public Tween() { }

    public Tween(float duration) {
        this.duration = duration;
    }

    public Tween(T start, T dest, float duration,
    TweenEndAction endAction = TweenEndAction.STOP,
    EasingCurve curve = EasingCurve.linear) {
        startValue = start;
        destValue = dest;
        this.duration = duration;
        this.endAction = endAction;
        this.curve = curve;
    }

    #endregion

    public void setValues(T start, T dest) {
        startValue = start;
        destValue = dest;
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
            tweenValue = (T)evaluateLerp(startValue, destValue, x); //Mathf.Lerp(startValue, destValue, x);
            if(t >= 1) evaluateEnd();
        }
    }

    private void evaluateEnd() {
        endHits++;
        switch(endAction) {
            case TweenEndAction.STOP: {
                runTween = false; break;
            }
            case TweenEndAction.RESET: {
                tweenValue = (T)evaluateLerp(startValue, destValue, reverse ? 1 : 0);
                runTween = false; break;
            }
            case TweenEndAction.LOOP: {
                startTime = Time.time; break;
            }
            case TweenEndAction.PINGPONG: {
                startTime = Time.time;
                reverse = !reverse; break;
            }
            case TweenEndAction.RETURN: {
                startTime = Time.time;
                if(endHits < 2) reverse = !reverse;
                else runTween = false; break;
            }
        }
    }

    private object evaluateLerp(T start,T stop, float t) {
        // as you can see here, there is some wierd shit going on
        // with generics and thse "object" classes. But it works!
        System.Type type = typeof(T);
        object _start = start;
        object _stop = stop;
        if(type == typeof(float)) {
            return Mathf.LerpUnclamped((float)_start, (float)_stop, t);
        } else if(type == typeof(Vector2)) {
            return Vector2.LerpUnclamped((Vector2)_start, (Vector2)_stop, t);
        } else if(type == typeof(Vector3)) {
            return Vector3.LerpUnclamped((Vector3)_start, (Vector3)_stop, t);
        } else if(type == typeof(Color)) {
            return Color.LerpUnclamped((Color)_start, (Color)_stop, t);
        } else if(type == typeof(Quaternion)) {
            return Quaternion.LerpUnclamped((Quaternion)_start, (Quaternion)_stop, t);
        } else {
            return Mathf.LerpUnclamped((float)_start, (float)_stop, t);
        }
    }

    #region custom easing curves
    // CREATE YOUR OWN EASING CURVE!!!!!

    // add your own curve 
    // hint for curve namings: "In" means forwards, "Out" means backwards
    // also good reference for easing curves https://easings.net/

    private const float TWO_PI = Mathf.PI * 2;
    private float squared(float v) { return v * v; }

    // add your own curve functions here
    private float curveLinear(float t) { return t; }
    private float curveQuadIn(float t) { return squared(t); }
    private float curveQuadOut(float t) { return -squared(t - 1) + 1; }
    private float curveSineWave(float t) { return 0.5f * Mathf.Sin(TWO_PI * (t - 0.25f)) + 0.5f; } // this equation naturally returns to zero

    // then setup the easing curve function with its coresponding enum
    private float applyCurve(float t) {
        switch(curve) {
            case EasingCurve.linear:
                return curveLinear(t);
            case EasingCurve.quadIn:
                return curveQuadIn(t);
            case EasingCurve.quadOut:
                return curveQuadOut(t);
            case EasingCurve.sineWave:
                return curveSineWave(t);
            default:
                return t;
        }
    }
    #endregion

}

public enum EasingCurve {
    linear,
    quadIn,
    quadOut,
    sineWave
}

public enum TweenEndAction {
    STOP,
    RESET,
    RETURN,
    LOOP,
    PINGPONG,
}

public class Tween : Tween<float> { public Tween() : base() { } } // default type