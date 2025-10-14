namespace Charts.Api.Middleware
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class RequireDbKeyAttribute : Attribute { }
}
