namespace Mcsg.Common.Interfaces;

public interface IValidator<T>
{
    Task OnValidate(T data);
}
