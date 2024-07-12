namespace Mcsg.Lib.Common.Models;

public class SyncData
{
    public IDictionary<object, object> Data { get; set; }
    public SyncTargetDb TargetDb { get; set; }
    public SyncTargetEntity TargetEntity { get; set; }
}

public enum SyncTargetDb
{
    WALLETDB,
    MAINDB
}

public enum SyncTargetEntity
{
    WALLET_USER_INFO,
    WALLET_USER_REWARD,
    WALLET_USER_BUY_PREMIUM,
    WALLET_USER_BUY_CHAPTER,
    WALLET_USER_BUY_SERIES,
}
