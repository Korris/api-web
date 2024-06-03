using Mcsg.Lib.Data.Wallet.Enums;
using System.ComponentModel.DataAnnotations;

namespace Mcsg.Lib.Data.Wallet.Entities
{
    public class WalletSettingDetail : BaseWalletEntity
    {
        [MaxLength(50)]
        public string Name { get; set; }
        public WalletSettingDetailType Type { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }
}
