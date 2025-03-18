namespace MatrixResponsibility.Common.Constants
{
    public class str
    {
        public const string jwttoken = nameof(jwttoken);
        public const string login = nameof(login);
        public const string authenticationType = "jwt";
        public const string access_token = nameof(access_token);

        //main hub methods
        public const string ProjectChanged = nameof(ProjectChanged);
        public const string ChangeProjectInfo = nameof(ChangeProjectInfo);
        public const string GetAllProjects = nameof(GetAllProjects);
        public const string ConnectedClientsCount = nameof(ConnectedClientsCount);
    }
}
