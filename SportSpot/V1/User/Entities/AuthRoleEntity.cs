using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace SportSpot.V1.User
{
    [CollectionName("col_role")]
    public class AuthRoleEntity : MongoIdentityRole<Guid>
    {
    }
}
