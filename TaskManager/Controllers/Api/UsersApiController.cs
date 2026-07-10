using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using TaskManager.Data.Repositories;

namespace TaskManager.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/users")]
    public class UsersApiController : ApiController
    {
        private IUserRepository Users => DependencyConfig.Resolve<IUserRepository>();

        [HttpGet, Route("")]
        public async Task<IHttpActionResult> List()
        {
            var users = Users;
            using ((System.IDisposable)users)
            {
                var list = await users.ListActiveAsync();
                return Ok(list.Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.Department,
                    u.Role,
                    u.LastLoginAt
                }));
            }
        }
    }
}
