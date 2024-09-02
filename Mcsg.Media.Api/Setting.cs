using Newtonsoft.Json;

namespace Mcsg.Media.Api;

using Common.SeedWork;
using Interfaces;
using static Common.SeedWork.Dtos.StorageDto;

/// <summary>
/// Setting
/// </summary>
public class Setting : SettingBase, ISetting
{
    #region -- Implements --

    //TODO

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public Setting()
    {
    }

    /// <summary>
    /// Load storages
    /// </summary>
    public void LoadStorages()
    {
        Minio.Storages = JsonConvert.DeserializeObject<List<MinioInstanceDto>>(Storage!)!;
    }

    #endregion
}
