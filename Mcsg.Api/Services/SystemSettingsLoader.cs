namespace Mcsg.Api.Services;

using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Extensions;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Reads the system.SystemSettings table and applies it onto the singleton Setting instance.
/// Used once at boot (Program.cs), every 30s by SystemSettingsRefreshHostedService on every replica, and on demand by
/// POST api/identity/config/v1/Reload (admin), so a row edited in the database (e.g. RpcChatChat) takes effect
/// without restarting the pod.
/// Only SystemSettings is refreshed here. SystemConfigs (JWT, MinIO, email, Redis) is captured by other services
/// at startup, so reloading it at runtime would not propagate and is intentionally left out.
/// </summary>
public static class SystemSettingsLoader
{
    #region -- Methods --

    /// <summary>
    /// Loads the active rows (Key, Value, DataType only) from the database
    /// </summary>
    public static Task<List<SystemSetting>> LoadAsync(IMcsgContext context, CancellationToken ct = default)
    {
        return context.SystemSettings
            .AsNoTracking()
            .Where(p => !string.IsNullOrWhiteSpace(p.Key))
            .Select(p => new SystemSetting { Key = p.Key, Value = p.Value, DataType = p.DataType })
            .ToListAsync(ct);
    }

    /// <summary>
    /// Copies the rows onto the setting object: typed scalars, then the Api*/Rpc* URL maps
    /// </summary>
    public static void Apply(Setting st, List<SystemSetting> rows)
    {
        var set = rows.ToDictionary(p => p.Key + "", p => p);
        if (set.TryGetValue("XApiKey", out var ett)) Setting.XApiKey = ett.Value.Cast<string?>(ett.DataType) ?? "";
        if (set.TryGetValue(nameof(st.AccountDeletedAfter), out ett)) st.AccountDeletedAfter = ett.Value.Cast<uint?>(ett.DataType) ?? 0;
        if (set.TryGetValue(nameof(st.AccountCreatedAfter), out ett)) st.AccountCreatedAfter = ett.Value.Cast<uint?>(ett.DataType) ?? 0;
        if (set.TryGetValue(nameof(st.UserNameChangedInRemaining), out ett)) st.UserNameChangedInRemaining = ett.Value.Cast<double?>(ett.DataType) ?? 0;
        if (set.TryGetValue(nameof(st.UserNameWaitingChangedAfter), out ett)) st.UserNameWaitingChangedAfter = ett.Value.Cast<double?>(ett.DataType) ?? 0;
        if (set.TryGetValue(nameof(st.UsernameIsReserved), out ett)) st.UsernameIsReserved = ett.Value.Cast<string?>(ett.DataType) ?? "";
        if (set.TryGetValue(nameof(st.PercentFeed), out ett)) st.PercentFeed = ett.Value.Cast<double?>(ett.DataType) ?? 0;
        if (set.TryGetValue(nameof(st.PercentComic), out ett)) st.PercentComic = ett.Value.Cast<double?>(ett.DataType) ?? 0;
        if (set.TryGetValue(nameof(st.PercentDocument), out ett)) st.PercentDocument = ett.Value.Cast<double?>(ett.DataType) ?? 0;
        if (set.TryGetValue(nameof(st.PercentStory), out ett)) st.PercentStory = ett.Value.Cast<double?>(ett.DataType) ?? 0;
        if (set.TryGetValue(nameof(st.PercentTapShow), out ett)) st.PercentTapShow = ett.Value.Cast<double?>(ett.DataType) ?? 0;
        if (set.TryGetValue(nameof(st.NumberOfPosts), out ett)) st.NumberOfPosts = ett.Value.Cast<int?>(ett.DataType) ?? 0;

        var dic = rows.ToDictionary(p => p.Key + "", p => p.Value + "");
        st.LoadApiUrl(dic, st.IsLocal, !string.IsNullOrWhiteSpace(st.Protocols));
        st.LoadRpcUrl(dic, st.IsLocal);
    }

    /// <summary>
    /// Key→Value snapshot used by the refresh service to detect which rows changed between two loads
    /// </summary>
    public static Dictionary<string, string> Snapshot(List<SystemSetting> rows)
    {
        return rows.ToDictionary(p => p.Key + "", p => p.Value + "");
    }

    #endregion
}
