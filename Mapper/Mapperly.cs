using Riok.Mapperly.Abstractions;
using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.DTO.Requests;
using TicketingSystem.Models.DTO.Requests.Agency;
using TicketingSystem.Models.DTO.Requests.Category;
using TicketingSystem.Models.DTO.Requests.Subscriptions;
using TicketingSystem.Models.DTO.Requests.Ticket;
using TicketingSystem.Models.DTO.Requests.User;
using TicketingSystem.Models.DTO.Responses.Category;
using TicketingSystem.Models.DTO.Responses.Ticket;
using TicketingSystem.Models.DTO.Responses.User;
using TicketingSystem.Models.Entities.Agency;
using TicketingSystem.Models.Identity;
using TicketingSystem.Models.Entities.Tickets;

namespace TicketingSystem;

[Mapper(AllowNullPropertyAssignment = false)]
public partial class Mapper
{
    // generic mapping
    public partial TTarget ToResponse<TTarget, T>(IEntity<T> source)
        where T : IEquatable<T>;

    public partial TTarget ToEditModel<TTarget, T>(IEntity<T> source)
        where T : IEquatable<T>;

    public partial TEntity ToEntity<TEntity, T>(ICreateRequest source)
        where TEntity : class, IEntity<T>
        where T : IEquatable<T>;

    public partial TEntity ToEntity<TEntity, T>(IEditRequest<T> source)
        where TEntity : class, IEntity<T>
        where T : IEquatable<T>;

    // User
    [MapperIgnoreTarget(nameof(User.Agency))]
    [MapperIgnoreTarget(nameof(User.AgencyId))]
    public partial User ToEntity(RegisterUser dto);

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

    // Subscription
    public partial Models.Entities.Agency.Subscription ToEntity(CreateSubscriptionRequest source);

    public partial Models.DTO.Responses.Subscriptions.SubscriptionResponse ToResponse(
        Subscription source
    );

    // Ticket
    public partial TicketResponse ToResponse(Ticket source);

    [MapProperty(
        nameof(CreateTicketRequest.AgencyId),
        nameof(Ticket.AgencyId),
        Use = nameof(ParseUlid)
    )]
    [MapProperty(
        nameof(CreateTicketRequest.CategoryId),
        nameof(Ticket.CategoryId),
        Use = nameof(ParseUlid)
    )]
    public partial Ticket ToEntity(CreateTicketRequest source);

    [MapProperty(
        nameof(CreateTicketRequest.AgencyId),
        nameof(Ticket.AgencyId),
        Use = nameof(ParseUlid)
    )]
    [MapProperty(
        nameof(EditTicketRequest.CategoryId),
        nameof(Ticket.CategoryId),
        Use = nameof(ParseUlid)
    )]
    public partial Ticket ToEntity(EditTicketRequest source);

    [UserMapping]
    public Ulid ParseUlid(string id)
    {
        return Ulid.Parse(id);
    }

    // Category
    public partial CategoryResponseDto ToResponse(Category source);

    [MapProperty(
        nameof(CreateCategoryRequest.AgencyId),
        nameof(Category.AgencyId),
        Use = nameof(ParseUlid)
    )]
    public partial Category ToEntity(CreateCategoryRequest source);

    [MapProperty(
        nameof(EditCategoryRequest.AgencyId),
        nameof(Category.AgencyId),
        Use = nameof(ParseUlid)
    )]
    public partial Category ToEntity(EditCategoryRequest source);
}
