using Riok.Mapperly.Abstractions;
using TicketingSystem.Models.DTO.Requests.Agency;
using TicketingSystem.Models.DTO.Requests.User;
using TicketingSystem.Models.DTO.Responses.User;
using TicketingSystem.Models.DTO.User;
using TicketingSystem.Models.Entities.Agency;
using TicketingSystem.Models.Identity;

namespace TicketingSystem.Mapper;

[Mapper(AllowNullPropertyAssignment = false)]
public partial class AppMapper
{
    [MapPropertyFromSource(nameof(UserProfile.Name), Use = nameof(CombineNames))]
    public partial UserProfile UserResponse(User user);

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
    public partial Models.DTO.Responses.Agency.Agency ToResponse(Agency model);
}

[Mapper]
public partial class UserMapper
{
    public partial User ToEntity(UserRequestDto dto);

    public partial UserResponseDto ToResponseDto(User user);
}
