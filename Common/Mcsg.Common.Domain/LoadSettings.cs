using Mcsg.Common.Domain.Entities;
using Mcsg.Common.SeedWork.Dtos;
using System.Dynamic;
using static Mcsg.Common.SeedWork.Dtos.ConnectionDto;
using static Mcsg.Common.SeedWork.Dtos.StorageDto;

namespace Mcsg.Common.Domain;

public class LoadSettings
{
    public static void LoadSettingsFromDatabase(dynamic st, List<SystemConfig> configs)
    {
        st.Jwt ??= new JwtDto();
        st.Queue ??= new QueueDto();
        st.Minio ??= new MinioDto { Storages = new List<MinioInstanceDto>() };
        var cfgDict = configs.ToDictionary(c => c.Key, c => c);

        var jwtKeys = new[] { "Signing", "Issuer", "Audience", "TimeAt", "TimeRt" };
        foreach (var key in jwtKeys)
        {
            if (cfgDict.TryGetValue(key, out var cfg))
            {
                var val = ConvertByDataType(cfg.Value, cfg.DataType);
                SetPropertySafe(st.Jwt, key, val);
            }
        }

        var queueKeys = new[] { "Host", "Port", "VirtualHost", "UserName", "Password" };
        foreach (var key in queueKeys)
        {
            if (cfgDict.TryGetValue(key, out var cfg))
            {
                var val = ConvertByDataType(cfg.Value, cfg.DataType);
                SetPropertySafe(st.Queue, key, val);
            }
        }

        var storageTypes = new[] { "Default", "Story", "Comic" };
        foreach (var suffix in storageTypes)
        {
            if (!cfgDict.TryGetValue($"Instance_{suffix}", out var instanceKey))
                continue;

            var instanceValue = ConvertByDataType(instanceKey.Value, instanceKey.DataType);

            var storage = new MinioInstanceDto();
            SetPropertySafe(storage, "Instance", instanceValue);

            var fields = new[]
            {
                "EndPoint", "PublicUrl", "CdnImageUrl", "CdnVideoUrl",
                "BucketName", "Location", "AccessKey", "SecretKey"
            };

            foreach (var field in fields)
            {
                if (cfgDict.TryGetValue($"{field}_{suffix}", out var cfg))
                {
                    var val = Convert.ToString(ConvertByDataType(cfg.Value, cfg.DataType));
                    SetPropertySafe(storage, field, val);
                }
            }

            st.Minio.Storages.Add(storage);
        }
    }

    private static object? ConvertByDataType(string? value, string? dataType)
    {
        if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(dataType))
            return value;

        return dataType.ToLowerInvariant() switch
        {
            "int" or "integer" => int.TryParse(value, out var i) ? i : null,
            "ushort" => ushort.TryParse(value, out var us) ? us : null,
            "long" => long.TryParse(value, out var l) ? l : null,
            "bool" or "boolean" => bool.TryParse(value, out var b) ? b : null,
            "double" => double.TryParse(value, out var d) ? d : null,
            "decimal" => decimal.TryParse(value, out var dec) ? dec : null,
            "datetime" => DateTime.TryParse(value, out var dt) ? dt : null,
            _ => value
        };
    }

    private static void SetPropertySafe(object obj, string propertyName, object? value)
    {
        var prop = obj.GetType().GetProperty(propertyName);
        if (prop != null && prop.CanWrite)
        {
            try
            {
                var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                var safeValue = (value == null) ? null : Convert.ChangeType(value, targetType);
                prop.SetValue(obj, safeValue, null);
            }
            catch
            {
            }
        }
        else if (obj is ExpandoObject expando)
        {
            ((IDictionary<string, object?>)expando)[propertyName] = value;
        }
    }

}
