namespace Mcsg.Wallet.Api.Models;

public class BankListFromApiResp
{
    public List<BankFromApiResp> Data { get; set; }
}

public class BankFromApiResp
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Bin { get; set; }
    public string ShortName { get; set; }
    public string Logo { get; set; }
    public bool TransferSupported { get; set; }
    public bool LookupSupported { get; set; }
    public string Short_Name { get; set; }
    public int Support { get; set; }
    public bool IsTransfer { get; set; }
    public string Swift_Code { get; set; }
}
