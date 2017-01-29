﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WycademyV2.Commands.Utilities
{
    public struct EvalResult
    {
        public bool IsSuccess { get; private set; }
        public string Output { get; set; }

        public EvalResult(bool successful, string output)
        {
            IsSuccess = successful;
            Output = output;
        }
    }
}
