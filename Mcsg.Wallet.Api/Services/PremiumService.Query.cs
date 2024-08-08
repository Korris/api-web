namespace Mcsg.Wallet.Api.Services
{
    public partial class PremiumService
    {
        private string UpdatePremiumDate
        {
            get
            {
                return @"UPDATE identity.""Sessions""
                                    SET ""PremiumDate"" = @PremiumDate
                                    WHERE ""UserId"" = @UserId AND ""ExpiredDateUtc"" > @DateTimeNow;
                        UPDATE identity.""Users""
                                    SET ""PremiumDate"" = @PremiumDate
                                    WHERE ""Id"" = @UserId;";
            }
        }
    }
}
