using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Common.Exceptions
{
    public class UnauthorizedException  :Exception
    {
        public UnauthorizedException(string message) : base(message) { }

    }
}
