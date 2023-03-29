namespace shared.Common;

public class Routes
{
    public const string Hello = $"/project/hello";

    public static class Project
    {
        public const string Create = "/project/create";
        public const string Delete = "/project/delete/{id}";
        public const string Update = "/project/update";

        public const string Get = "/project/{id}";
        public const string GetAll = "/projects";

        public const string Leave = "/project/leave/{id}";
        public const string Join = "/project/join/{id}";
    }

    public static class User
    {
        public const string Get = "/user/{id}";
        public const string GetAll = "/users";
        public const string GetProjects = "/user/projects/{id}";
    }
}
