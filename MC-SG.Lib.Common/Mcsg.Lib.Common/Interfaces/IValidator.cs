namespace Mcsg.Lib.Common.Interfaces
{
    public interface IValidator<T>
    {
        Task OnValidate(T data);
    }
}
