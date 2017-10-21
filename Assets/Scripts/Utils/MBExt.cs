using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MBExt {
    class WhenCondition {
        private bool condition;

        public WhenCondition(ref bool condition) {
            this.condition = condition;
        }

        public Func<bool> getFunc() {
            return () => { return condition; };
        }
    }
    public static void LerpRoutine(this MonoBehaviour b, float duration, Action<float> onEachIteration) {
        b.StartCoroutine(LerpRoutine(duration, onEachIteration));
    }

    public static void LerpRoutine(this MonoBehaviour b, float duration, Action<float> onEachIteration, System.Action onEnd) {
        b.StartCoroutine(LerpRoutine(duration, onEachIteration, onEnd));
    }

    public static void LerpRoutine(this MonoBehaviour b, float duration, Func<float, float> l, System.Action<float> onEachIteration, System.Action onEnd) {
        b.StartCoroutine(LerpRoutine(duration, l, onEachIteration, onEnd));
    }

    public static IEnumerator LerpRoutine(float duration, System.Action<float> onEachIteration) {
        float timer = 0;

        while(timer < duration) {
            yield return new WaitForEndOfFrame();
            onEachIteration.Invoke(timer / duration);
            timer += Time.deltaTime;
        }

        onEachIteration.Invoke(1);
    }

    public static void Log(string format, params object[] args) {
        Debug.Log(string.Format(format, args));
    }

    public static IEnumerator LerpRoutine(float duration, Action<float> onEachIteration, System.Action onEnd) {
        float timer = 0;

        while(timer < duration) {
            yield return new WaitForEndOfFrame();
            onEachIteration.Invoke(timer / duration);
            timer += Time.deltaTime;
        }

        onEachIteration.Invoke(1);
        onEnd.Invoke();
    }

    public static void LerpRoutine(this MonoBehaviour b, float duration, Func<float, float> l, System.Action<float> onEachIteration) {
        b.StartCoroutine(LerpRoutine(duration, l, onEachIteration));
    }

    public static void ModularLerp(this MonoBehaviour mb, float duration, float a, float b, float range, System.Action<float> onEachIteration) {
        mb.LerpRoutine(duration, (t) => onEachIteration.Invoke(ModularLerp(a, b, range, t)));
    }

    public static float ModularLerp(float a, float b, float range, float t) {
        a = Mathf.Repeat(a, range);
        b = Mathf.Repeat(b, range);
        var delta = a - b;
        var d = Mathf.Abs(delta);
        var dir = Mathf.Sign(delta);
        var nd = range - d;

        if (d < nd) {
            return Mathf.Lerp(a, b, t);
        } else {
            var disp = Mathf.Lerp(0, nd, t);
            return (a + disp * dir + range) % range;
        }
    }

    public static float ModularLerpUnclamped(float a, float b, float range, float t) {
        var ca = Mathf.Repeat(a, range);
        var cb = Mathf.Repeat(b, range);
        var delta = cb - ca;
        var d = Mathf.Abs(delta);
        var dir = Mathf.Sign(delta);
        var nd = range - d;

        var disp = Mathf.Lerp(0, Mathf.Min(d, nd), t);
        return a + disp * dir;
    }

    public static IEnumerator LerpRoutine(float duration, Func<float, float> l, Action<float> onEachIteration) {
        float timer = 0;

        while(timer < duration) {
            yield return new WaitForEndOfFrame();
            var t = l.Invoke(timer / duration);
            onEachIteration.Invoke(t);
            timer += Time.deltaTime;
        }

        onEachIteration.Invoke(1);
    }

    public static IEnumerator LerpRoutine(float duration, Func<float, float> l, Action<float> onEachIteration, Action onEnd) {
        float timer = 0;

        while(timer < duration) {
            yield return new WaitForEndOfFrame();
            var t = l.Invoke(timer / duration);
            onEachIteration.Invoke(t);
            timer += Time.deltaTime;
        }

        onEachIteration.Invoke(1);
        onEnd.Invoke();
    }

    public static float Coserp(float t) {
        return 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
    }

    public static float Sinerp(float t) {
        return Mathf.Sin(t * Mathf.PI * 0.5f); ;
    }

    public static float SmoothStep(float t) {
        return t * t * (3f - 2f * t);
    }

    public static float SmootherStep(float t) {
        return t * t * t * (t * (6f * t - 15f) + 10f);
    }

    public static float ExpStep(float t) {
        return t * t;
    }

    public static float ExpStep(float t, int p) {
        return Mathf.Pow(t, p);
    }

    public static void Wait(this MonoBehaviour b, float s, Action callback) {
        b.StartCoroutine(ExecAfterRoutine(s, callback));
    }   

    public static void ExecWhen(this MonoBehaviour b, Func<bool> test, Action callback) {
        b.StartCoroutine(ExecWhenRoutine(test, callback));
    }

    public static void ExecUntil(this MonoBehaviour b, Func<bool> test, Action callback) {
        b.StartCoroutine(ExecUntilRoutine(test, callback));
    }

    public static void ExecUntil(this MonoBehaviour b, Func<bool> test, Action callback, Action onEnd) {
        b.StartCoroutine(ExecUntilRoutine(test, callback, onEnd));
    }

    static IEnumerator ExecWhenRoutine(Func<bool> test, Action callback) {
        yield return new WaitUntil(test);
        callback();
    }

    static IEnumerator ExecUntilRoutine(Func<bool> test, Action callback) {
        while (!test()) {
            callback();
            yield return null;
        }        
    }

    static IEnumerator ExecUntilRoutine(Func<bool> test, Action callback, Action onEnd) {
        while (!test()) {
            callback();
            yield return null;
        }

        onEnd();
    }

    static IEnumerator ExecAfterRoutine(float s, Action callback) {
        yield return new WaitForSeconds(s);
        callback();
    }
}
