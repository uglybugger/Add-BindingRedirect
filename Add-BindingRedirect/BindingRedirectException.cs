using System;
using System.Runtime.Serialization;

namespace Add_BindingRedirect
{
    [Serializable]
    public class BindingRedirectException : Exception
    {
        public BindingRedirectException()
        {
        }

        public BindingRedirectException(string message) : base(message)
        {
        }

        public BindingRedirectException(string message, Exception inner) : base(message, inner)
        {
        }

        protected BindingRedirectException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}