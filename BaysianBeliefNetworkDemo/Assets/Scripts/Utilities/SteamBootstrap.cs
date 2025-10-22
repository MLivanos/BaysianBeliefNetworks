using UnityEngine;
using Steamworks;
using Steamworks.Data;
using System;
using System.Collections;
using System.Linq;

public class SteamBootstrap : MonoBehaviour
{
    public static bool Initialized { get; private set; }
    public static bool StatsReady  { get; private set; }

    [Header("Set this to 480 for Spacewar test, or your real AppID")]
    [SerializeField] private uint appIdOverride = 0;

    // keep a reference so we can unsubscribe safely (no event = null!)
    private static Action<SteamId, Result> _statsHandler;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStaticsBetweenRuns()
    {
        Initialized = false;
        StatsReady  = false;
        _statsHandler = null;
    }

    void Awake()
    {
        Debug.Log("[SteamBootstrap] Awake() — starting init");
        DontDestroyOnLoad(gameObject);

        try
        {
            // Prefer explicit override in Inspector; fallback is fine if you have steam_appid.txt
            uint appid = appIdOverride != 0 ? appIdOverride : SteamClient.AppId;
            Debug.Log($"[SteamBootstrap] Init with AppID={appid} | Platform={Application.platform} | OS='{SystemInfo.operatingSystem}'");

            SteamClient.Init(appid, true);
            Debug.Log(Steamworks.SteamClient.Name);
            Initialized = SteamClient.IsValid;
            Debug.Log($"[SteamBootstrap] SteamClient.IsValid={SteamClient.IsValid}, LoggedOn={SteamClient.IsLoggedOn}");
            Debug.Log($"[Steam] AppId={SteamClient.AppId}, IsValid={SteamClient.IsValid}, LoggedOn={SteamClient.IsLoggedOn}");
            Debug.Log($"[Steam] IsSubscribed={SteamApps.IsSubscribed}");

            // Unsubscribe any prior handler (from previous play session) before re-subscribing
            if (_statsHandler != null) SteamUserStats.OnUserStatsReceived -= _statsHandler;

            _statsHandler = (sid, result) =>
            {
                Debug.Log($"[Steam] StatsReceived: result={result} for={sid} (me={SteamClient.SteamId})");
                if (result == Result.OK && sid == SteamClient.SteamId)
                {
                    StatsReady = true;
                    Debug.Log("[SteamBootstrap] StatsReady = TRUE ✨");
                    DumpAchievements();
                }
                else
                {
                    Debug.LogWarning("[SteamBootstrap] StatsReceived but not OK or wrong SID");
                }
            };

            // Delay the first request slightly, then retry once if needed (stabilizes mac/Editor)
            StartCoroutine(RequestStatsWithRetry());
        }
        catch (System.DllNotFoundException e)
        {
            Debug.LogError("[SteamBootstrap] DllNotFoundException: " + e.Message);
        }
        catch (System.Exception e)
        {
            Debug.LogError("[SteamBootstrap] Exception during Init: " + e);
        }
    }

    void Update()
    {
        if (Initialized) SteamClient.RunCallbacks();
    }

    void Start()
    {
        Debug.Log("[SteamBootstrap] Start() — Initialized=" + Initialized);
    }

    //void OnDisable()         { Cleanup("[SteamBootstrap] OnDisable"); }
    //void OnDestroy()         { Cleanup("[SteamBootstrap] OnDestroy"); }
    void OnApplicationQuit() { Cleanup("[SteamBootstrap] OnApplicationQuit"); }
    //void ExitingPlayMode()   { Cleanup("[SteamBootstrap] ExitingPlayMode");}

    private IEnumerator RequestStatsWithRetry()
    {
        // wait ~0.25s letting callbacks spin; prevents occasional race
        for (int i = 0; i < 15; i++) { SteamClient.RunCallbacks(); yield return null; }

        Debug.Log("[Steam] RequestCurrentStats() #1");
        SteamUserStats.RequestCurrentStats();

        // retry once after ~2s if not ready
        float t = 0f;
        while (!StatsReady && (t += Time.unscaledDeltaTime) < 2f) yield return null;

        if (!StatsReady)
        {
            Debug.LogWarning("[Steam] Stats not ready after 2s; retrying RequestCurrentStats() #2");
            SteamUserStats.RequestCurrentStats();
        }
    }

    private void Cleanup(string where)
    {
        Debug.Log($"{where} → Cleanup");
        if (_statsHandler != null)
        {
            SteamUserStats.OnUserStatsReceived -= _statsHandler;
            _statsHandler = null;
        }
        SteamClient.Shutdown();
        Initialized = false;
        StatsReady  = false;
    }

    private void DumpAchievements()
    {
        var list = SteamUserStats.Achievements?.ToList();
        Debug.Log(list);
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning("[SteamBootstrap] No achievements returned. (Wrong AppID? Not Available/Published? No package license?)");
            return;
        }

        Debug.Log($"[SteamBootstrap] Found {list.Count} achievement(s):");
        foreach (var a in list)
            Debug.Log($"[SteamBootstrap] ACH id='{a.Identifier}' name='{a.Name}' unlocked={a.State}");
    }
}
