using System;
using System.Collections.Generic;
using System.Text;

namespace LogListener.Exceptions
{
    class InvalidArguments : Exception
    {

        public InvalidArguments(string message) : base(message)
        {

        }
    }
}
