using System;

namespace Frog.Orm
{
    public class InvalidRowCountException : Exception
    {
        public InvalidRowCountException(int expectedRowModificationCount, int actualModificationCount) 
            : base(String.Format("Expected modification of {0} row(s). Actual rows affected: {1}", expectedRowModificationCount, actualModificationCount))
        {

        }
    }
}
