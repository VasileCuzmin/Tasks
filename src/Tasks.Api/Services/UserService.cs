using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Tasks.Api.Services
{
    internal class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpAccessor;

        public UserService(IHttpContextAccessor httpAccessor)
        {
            _httpAccessor = httpAccessor;
        }

        public string GetUserId()
        {
            var id = _httpAccessor.HttpContext.User?.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;

            //if we don't have id -> get client default id
            if (string.IsNullOrWhiteSpace(id))
            {
                var client = _httpAccessor.HttpContext.User?.Claims.FirstOrDefault(x => x.Type == "client_id")?.Value;
                if (!string.IsNullOrWhiteSpace(client))
                {
                    //TODO
                    //get value from config or db
                    id = "C8124881-AD67-443E-9473-08D5777D1BA8";
                }
            }
            return id;
        }

        public string GetLanguage()
        {
            var language = _httpAccessor.HttpContext.Request.Headers["Content-Language"];
            return language;
        }
    }

    public interface IUserService
    {
        string GetUserId();
        string GetLanguage();
    }
}
