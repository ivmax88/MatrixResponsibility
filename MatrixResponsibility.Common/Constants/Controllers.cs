namespace MatrixResponsibility.Common.Constants
{
    public abstract class StrControllerBase
    {
        public string Name { get; protected init; }
        protected StrControllerBase(string name) => Name = name;
        public override string ToString() => Name;
    }

    public static class Controllers
    {
        public static StrControllerBase AuthService { get; } = new AuthService();
    }

    internal class AuthService : StrControllerBase
    {
        public AuthService() : base(nameof(AuthService)) { }
    }

}
