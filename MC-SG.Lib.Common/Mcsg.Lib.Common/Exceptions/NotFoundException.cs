using Mcsg.Lib.Common.Constants;

namespace Mcsg.Lib.Common.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message) : base(ErrorCodes.NotFoundCode, message)
        {
        }

        public NotFoundException(string code, string message) : base(code, message)
        {
        }
    }
}