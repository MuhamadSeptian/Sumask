using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Memastikan script ini bangun paling awal
[DefaultExecutionOrder(-50)]
public class ActionSystem : MonoBehaviour
{
    public static ActionSystem Instance { get; private set; }

    public bool IsPerforming { get; private set; }

    private void Awake()
    {
        // Singleton standar
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // ========================================================================
    // 1. PERFORMER LOGIC
    // ========================================================================
    private Dictionary<Type, object> performers = new Dictionary<Type, object>();

    public static void AttachPerformer<T>(Func<T, IEnumerator> performer)
    {
        // ANTI-CRASH: Cek apakah Instance ada?
        if (Instance == null) return;

        var type = typeof(T);
        if (Instance.performers.ContainsKey(type)) Instance.performers[type] = performer;
        else Instance.performers.Add(type, performer);
    }

    public static void DetachPerformer<T>(Func<T, IEnumerator> performer = null)
    {
        // ANTI-CRASH: Jika game sudah stop dan Instance null, abaikan saja.
        if (Instance == null) return;

        var type = typeof(T);
        if (Instance.performers.ContainsKey(type)) Instance.performers.Remove(type);
    }

    // ========================================================================
    // 2. REACTION LOGIC
    // ========================================================================
    private Dictionary<Type, List<object>> preReactions = new Dictionary<Type, List<object>>();
    private Dictionary<Type, List<object>> postReactions = new Dictionary<Type, List<object>>();

    public static void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing)
    {
        if (Instance == null) return; // Anti-Crash

        var targetDict = (timing == ReactionTiming.PRE) ? Instance.preReactions : Instance.postReactions;
        var type = typeof(T);
        if (!targetDict.ContainsKey(type)) targetDict[type] = new List<object>();
        targetDict[type].Add(reaction);
    }

    public static void UnsubscribeReaction<T>(Action<T> reaction, ReactionTiming timing)
    {
        if (Instance == null) return; // Anti-Crash

        var targetDict = (timing == ReactionTiming.PRE) ? Instance.preReactions : Instance.postReactions;
        var type = typeof(T);
        if (targetDict.ContainsKey(type)) targetDict[type].Remove(reaction);
    }

    // ========================================================================
    // 3. EXECUTION LOGIC
    // ========================================================================

    private Queue<IEnumerator> actionQueue = new Queue<IEnumerator>();
    private bool isProcessingQueue = false;

    public void AddReaction<T>(T action)
    {
        // Dapatkan tipe runtime sebenarnya (bukan compile-time type)
        var actualType = action.GetType();
        
        // Jika tipe runtime berbeda dari tipe generic, gunakan reflection
        if (actualType != typeof(T))
        {
            AddReactionDynamic(action, actualType);
            return;
        }

        // Enqueue action untuk diproses secara berurutan
        actionQueue.Enqueue(ProcessAction(action));
        
        // Jika belum ada yang memproses queue, mulai proses
        if (!isProcessingQueue)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    // Method helper untuk memanggil AddReaction dengan tipe runtime yang benar
    private void AddReactionDynamic(object action, Type actualType)
    {
        var method = typeof(ActionSystem).GetMethod(nameof(AddReaction));
        var genericMethod = method.MakeGenericMethod(actualType);
        genericMethod.Invoke(this, new[] { action });
    }

    private IEnumerator ProcessQueue()
    {
        isProcessingQueue = true;

        while (actionQueue.Count > 0)
        {
            var action = actionQueue.Dequeue();
            yield return action;
        }

        isProcessingQueue = false;
    }

    private IEnumerator ProcessAction<T>(T action)
    {
        IsPerforming = true;
        var type = typeof(T);

        Debug.Log($"[ActionSystem] Processing action: {type.Name}");

        // A. PRE-Reactions
        if (preReactions.ContainsKey(type))
        {
            var list = new List<object>(preReactions[type]);
            foreach (Action<T> reaction in list) reaction?.Invoke(action);
        }

        // B. Performer Utama
        if (performers.ContainsKey(type))
        {
            yield return ((Func<T, IEnumerator>)performers[type])(action);
        }
        else
        {
            Debug.LogWarning($"[ActionSystem] Tidak ada Performer untuk: {type.Name}");
        }

        // C. POST-Reactions
        if (postReactions.ContainsKey(type))
        {
            var list = new List<object>(postReactions[type]);
            foreach (Action<T> reaction in list) reaction?.Invoke(action);
        }

        IsPerforming = false;
        Debug.Log($"[ActionSystem] Selesai processing: {type.Name}");
    }
}