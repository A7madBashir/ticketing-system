using Riok.Mapperly.Abstractions;
using TicketingSystem.Models.DTO.Requests.Agency;
using TicketingSystem.Models.DTO.Requests.User;
using TicketingSystem.Models.DTO.Responses.User;
using TicketingSystem.Models.Entities.Agency;
using TicketingSystem.Models.Identity;

namespace TicketingSystem;

[Mapper(AllowNullPropertyAssignment = false)]
public partial class Mapper
{
    // User
    [MapperIgnoreTarget(nameof(User.Agency))]
    [MapperIgnoreTarget(nameof(User.AgencyId))]
    public partial User ToEntity(UserRequestDto dto);

    [MapPropertyFromSource(nameof(UserResponse.Name), Use = nameof(CombineNames))]
    public partial UserResponse ToResponse(User user);

    [MapperIgnoreTarget(nameof(Profile.Roles))]
    [MapPropertyFromSource(nameof(Profile.Name), Use = nameof(CombineNames))]
    public partial Profile ToUserProfile(User user);

    [UserMapping]
    private string CombineNames(User user)
    {
        string name = $"{user.FirstName} {user.LastName}";
        return (string.IsNullOrEmpty(name) ? user.UserName : name) ?? "";
    }

    // Agency
    [MapperIgnoreTarget(nameof(Agency.Id))]
    [MapperIgnoreTarget(nameof(Agency.Subscription))]
    [MapperIgnoreTarget(nameof(Agency.CreateTime))]
    [MapperIgnoreTarget(nameof(Agency.ModifiedTime))]
    [MapperIgnoreTarget(nameof(Agency.DeleteTime))]
    public partial Agency ToEntity(CreateAgency model);

    [MapperIgnoreTarget(nameof(Agency.Subscription))]
    [MapperIgnoreTarget(nameof(Agency.CreateTime))]
    [MapperIgnoreTarget(nameof(Agency.ModifiedTime))]
    [MapperIgnoreTarget(nameof(Agency.DeleteTime))]
    public partial Agency ToEntity(EditAgency model);

    [MapperIgnoreSource(nameof(Agency.SubscriptionId))]
    [MapperIgnoreSource(nameof(Agency.DeleteTime))]
    public partial Models.DTO.Responses.Agency.AgencyResponse ToResponse(Agency model);
}
