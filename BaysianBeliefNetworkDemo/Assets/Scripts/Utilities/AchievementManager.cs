using UnityEngine;
using Steamworks;
using Steamworks.Data;
using System.Collections.Generic;
using System.Linq; // for FirstOrDefault()

public static class Achievements
{
    public const string TUTORIAL = "ACH_TUTORIAL";
    public const string WITNESS  = "ACH_WITNESS";
    public const string WIN      = "ACH_WIN";
    public const string SECRET   = "ACH_SECRET";
}

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager I { get; private set; }
    private readonly Queue<string> pending = new();

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        ResetAllLocal();
    }

    void Update()
    {
        if (SteamBootstrap.Initialized && SteamBootstrap.StatsReady && pending.Count > 0)
            FlushPending();
    }

    public void Unlock(string id)
    {
        if (!SteamBootstrap.Initialized || !SteamBootstrap.StatsReady)
        {
            pending.Enqueue(id);
            return;
        }
        InternalUnlock(id);
    }

    private void FlushPending()
    {
        while (pending.Count > 0)
            InternalUnlock(pending.Dequeue());
    }

    private void InternalUnlock(string id)
    {
        if (!SteamBootstrap.Initialized) return;

        // Find the achievement by its API name (case-sensitive)
        var achievement = SteamUserStats.Achievements
            .FirstOrDefault(a => a.Identifier == id);

        if (achievement.Equals(default(Achievement)))
        {
            Debug.LogWarning($"Achievement {id} not found in SteamUserStats.Achievements 🫤");
            return;
        }

        if (achievement.State)
        {
            Debug.Log($"Achievement {id} already unlocked 💖");
            return;
        }

        achievement.Trigger();
        Debug.Log($"✨ Achievement {id} unlocked!");
    }

    [ContextMenu("Reset All Achievements (Local)")]
    public void ResetAllLocal()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        SteamUserStats.ResetAll(true);
        Debug.Log("Local Steam stats & achievements reset 💫");
#endif
    }
}
