using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WycademyV2.Commands.Entities
{
    public struct EvalResult
    {
        public bool IsSuccess { get; private set; }
        public string Output { get; private set; }
        public Exception Exception { get; private set; }

        public EvalResult(bool successful, string output, Exception exception = null)
        {
            IsSuccess = successful;
            Output = output;
            Exception = exception;
        }
    }
}
