// SignalHub.cs – (Async + Scopes)  |  2025‑06‑17
// Modern, typesafe, GC‑friendly signal system for Unity 6+.
// Features: Global hub + unlimited scopes, Subscribe/Once, Async (UniTask), weak‑refs, thread‑safe.

// UniTask package
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;

namespace Plugins.SignalBus
{
    /* ──────────────────────────── Interfaces ──────────────────────────── */
    public interface ISignal { }

    /* ──────────────────────────── Hub ──────────────────────────── */
    public sealed class SignalHub
    {
        /* ---------- Static Global ---------- */
        private static readonly Lazy<SignalHub> _lazy = new(() => new SignalHub());
        public static SignalHub Global => _lazy.Value;

        /* ---------- Internal ---------- */
        private readonly ConcurrentDictionary<Type, DelegateBucket> _map = new();
        private readonly SignalHub? _parent; // null for root

        /* ---------- Ctor ---------- */
        public SignalHub(SignalHub? parent = null) => _parent = parent;

        /* ---------- Scopes ---------- */
        public SignalHub CreateScope() => new SignalHub(this);

        /* ****************************************************************** */
        /* --------------------- Subscribe  (Sync)  ------------------------- */
        public IDisposable Subscribe<T>(Action<T> handler, bool once = false) where T : ISignal
            => Bucket<T>().Add(handler, once);
        public IDisposable SubscribeOnce<T>(Action<T> handler) where T : ISignal
            => Subscribe(handler, true);

        /* --------------------- Subscribe  (Async) ------------------------- */
        public IDisposable SubscribeAsync<T>(Func<T, UniTask> handler, bool once = false) where T : ISignal
            => Bucket<T>().AddAsync(handler, once);
        public IDisposable SubscribeOnceAsync<T>(Func<T, UniTask> handler) where T : ISignal
            => SubscribeAsync(handler, true);

        /* --------------------- Publish  (Sync)  --------------------------- */
        public void Publish<T>(T signal) where T : ISignal
        {
            Bucket<T>(searchParents: true).Invoke(signal);
        }

        /* --------------------- Publish  (Async) --------------------------- */
        public UniTask PublishAsync<T>(T signal) where T : ISignal
        {
            return Bucket<T>(searchParents: true).InvokeAsync(signal);
        }

        /* --------------------- Helpers --------------------------- */
        public void Clear() => _map.Clear();

#if UNITY_EDITOR
        [MenuItem("Tools/SignalsPro/Clear Global Hub")]
        private static void ClearGlobalEditor() => Global.Clear();
#endif

        /* ****************************************************************** */
        /* ---------------------  Delegate Bucket  -------------------------- */
        private DelegateBucket Bucket<T>(bool searchParents = false)
        {
            var type = typeof(T);
            if (_map.TryGetValue(type, out var bucket)) return bucket;
            if (searchParents && _parent != null) return _parent.Bucket<T>(true);
            return _map.GetOrAdd(type, _ => new DelegateBucket());
        }

        private sealed class DelegateBucket
        {
            private readonly object _lock = new();
            private readonly List<WeakEntry> _sync = new();
            private readonly List<WeakEntry> _async = new();

            /* ---- Add ---- */
            public IDisposable Add<T>(Action<T> cb, bool once)
            {
                var e = new WeakEntry(cb, once);
                lock (_lock) _sync.Add(e);
                return new Disposer(() => Remove(cb));
            }
            public IDisposable AddAsync<T>(Func<T, UniTask> cb, bool once)
            {
                var e = new WeakEntry(cb, once);
                lock (_lock) _async.Add(e);
                return new Disposer(() => RemoveAsync(cb));
            }

            /* ---- Remove ---- */
            private void Remove(Delegate cb)
            {
                lock (_lock) _sync.RemoveAll(w => w.TargetEquals(cb));
            }
            private void RemoveAsync(Delegate cb)
            {
                lock (_lock) _async.RemoveAll(w => w.TargetEquals(cb));
            }

            /* ---- Invoke Sync ---- */
            public void Invoke<T>(T signal)
            {
                WeakEntry[] snap;
                lock (_lock) snap = _sync.ToArray();
                foreach (var w in snap)
                {
                    if (w.TryInvokeSync(signal) && w.Once) Remove(w.GetTarget()!);
                }
            }

            /* ---- Invoke Async ---- */
            public async UniTask InvokeAsync<T>(T signal)
            {
                List<UniTask> tasks = null;
                WeakEntry[] snap;
                lock (_lock) snap = _async.ToArray();
                foreach (var w in snap)
                {
                    if (w.TryGetAsync<T>(out var fn))
                    {
                        tasks ??= new List<UniTask>();
                        tasks.Add(fn(signal));
                        if (w.Once) RemoveAsync(fn);
                    }
                }
                if (tasks != null) await UniTask.WhenAll(tasks);
            }

            /* ---- Weak Entry ---- */
            private sealed class WeakEntry
            {
                private readonly WeakReference _ref;  // Action<T> или Func<T, UniTask>
                public readonly bool Once;
                public WeakEntry(Delegate d, bool once) { _ref = new WeakReference(d); Once = once; }

                public bool TargetEquals(Delegate d) => _ref.Target == (object)d;
                public Delegate? GetTarget() => _ref.Target as Delegate;

                public bool TryInvokeSync<T>(T s)
                {
                    if (_ref.Target is Action<T> a) { SafeInvoke(() => a(s)); return true; }
                    return false;
                }
                public bool TryGetAsync<T>(out Func<T, UniTask> fn)
                {
                    if (_ref.Target is Func<T, UniTask> f) { fn = f; return true; }
                    fn = null; return false;
                }
                private static void SafeInvoke(Action call)
                {
                    try { call(); }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR
                        UnityEngine.Debug.LogException(ex);
#else
                        System.Diagnostics.Debug.WriteLine(ex);
#endif
                    }
                }
            }
        }

        /* ---- Disposable helper ---- */
        private sealed class Disposer : IDisposable
        {
            private Action? _release;
            public Disposer(Action release) => _release = release;
            public void Dispose() { _release?.Invoke(); _release = null; }
        }
    }

    /* ──────────────────────────── Extension Sugar ───────────────────────── */
    public static class SignalHubExtensions
    {
        public static IDisposable Listen<T>(this object _, Action<T> h, bool once = false) where T : ISignal
            => SignalHub.Global.Subscribe(h, once);
        public static IDisposable ListenAsync<T>(this object _, Func<T, UniTask> h, bool once = false) where T : ISignal
            => SignalHub.Global.SubscribeAsync(h, once);
        public static void Fire<T>(this T signal) where T : ISignal
            => SignalHub.Global.Publish(signal);
        public static UniTask FireAsync<T>(this T signal) where T : ISignal
            => SignalHub.Global.PublishAsync(signal);
    }
}
