using System;

namespace ExceptionNS.Resource
{
    public class ResourceNotAvailableException : Exception
    {
        public ResourceNotAvailableException()
        {
            
        }
        
        public ResourceNotAvailableException(string message) : base(message)
        {
            
        }        
        
        public ResourceNotAvailableException(string message, Exception inner) : base(message, inner)
        {
            
        }
    }
}