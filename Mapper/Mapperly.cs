using Riok.Mapperly.Abstractions;
using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.DTO.Requests;
using TicketingSystem.Models.DTO.Requests.Agency;
using TicketingSystem.Models.DTO.Requests.Category;
using TicketingSystem.Models.DTO.Requests.FAQ;
using TicketingSystem.Models.DTO.Requests.Subscriptions;
using TicketingSystem.Models.DTO.Requests.Ticket;
using TicketingSystem.Models.DTO.Requests.User;
using TicketingSystem.Models.DTO.Responses.Category;
using TicketingSystem.Models.DTO.Responses.FAQ;
using TicketingSystem.Models.DTO.Responses.Ticket;
using TicketingSystem.Models.DTO.Responses.User;
using TicketingSystem.Models.Entities.Agency;
using TicketingSystem.Models.Entities.Tickets;
using TicketingSystem.Models.FAQs;
using TicketingSystem.Models.Identity;
using TicketingSystem.Models.DTO.Responses.Replies;
using TicketingSystem.Models.DTO.Requests.Replies;




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
    [MapperIgnoreTarget(nameof(User.SecurityStamp))]
    [MapperIgnoreTarget(nameof(User.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(User.EmailConfirmed))]
    [MapperIgnoreTarget(nameof(User.AccessFailedCount))]
    [MapperIgnoreTarget(nameof(User.LastLoginAt))]
    [MapperIgnoreTarget(nameof(User.LastModifiedAt))]
    [MapperIgnoreTarget(nameof(User.LockoutEnd))]
    [MapperIgnoreTarget(nameof(User.LockoutEnabled))]
    [MapperIgnoreTarget(nameof(User.NormalizedUserName))]
    [MapperIgnoreTarget(nameof(User.NormalizedEmail))]
    [MapperIgnoreTarget(nameof(User.TwoFactorEnabled))]
    [MapperIgnoreTarget(nameof(User.PasswordHash))]
    [MapperIgnoreTarget(nameof(User.PhoneNumberConfirmed))]
    public partial User ToEntity(RegisterUser dto);

    [MapperIgnoreTarget(nameof(User.SecurityStamp))]
    [MapperIgnoreTarget(nameof(User.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(User.EmailConfirmed))]
    [MapperIgnoreTarget(nameof(User.AccessFailedCount))]
    [MapperIgnoreTarget(nameof(User.LastLoginAt))]
    [MapperIgnoreTarget(nameof(User.LockoutEnd))]
    [MapperIgnoreTarget(nameof(User.LastModifiedAt))]
    [MapperIgnoreTarget(nameof(User.LockoutEnabled))]
    [MapperIgnoreTarget(nameof(User.NormalizedUserName))]
    [MapperIgnoreTarget(nameof(User.NormalizedEmail))]
    [MapperIgnoreTarget(nameof(User.TwoFactorEnabled))]
    [MapperIgnoreTarget(nameof(User.PasswordHash))]
    [MapperIgnoreTarget(nameof(User.PhoneNumberConfirmed))]
    public partial void UpdateUserEntity(UpdateUserRequest dto, User entity);

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

    public partial Models.Entities.Agency.Subscription ToEntity(UpdateSubscriptionRequest source);

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

    // FAQ
    public partial FAQResponseDto ToResponse(FAQ source);

    [MapProperty(nameof(CreateFAQRequest.AgencyId), nameof(FAQ.AgencyId), Use = nameof(ParseUlid))]
    public partial FAQ ToEntity(CreateFAQRequest source);

    [MapProperty(nameof(EditFAQRequest.AgencyId), nameof(FAQ.AgencyId), Use = nameof(ParseUlid))]
    public partial FAQ ToEntity(EditFAQRequest source);

    //Reply
    public partial ReplyResponse ToResponse(Reply source);
    public partial Reply ToEntity(CreateReplyRequest source);
    public partial void UpdateReplyEntity(EditReplyRequest source, Reply target);
}
