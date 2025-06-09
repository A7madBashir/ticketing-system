using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TicketingSystem.Models.DTO.Requests.Agency;
using TicketingSystem.Models.DTO.Responses.Agency;
using TicketingSystem.Models.Entities.Agency;
using TicketingSystem.Services.Repositories;

namespace TicketingSystem.Controllers.Api;

public class AgencyController
    : CrudController<Agency, Ulid, AgencyResponse, CreateAgency, EditAgency>
{
    public AgencyController(IAgencyRepository repository, Mapper mapper)
        : base(repository, mapper) { }

    protected override string[] GetSearchableProperties()
    {
        throw new NotImplementedException();
    }

    protected override string[] IncludeNavigation()
    {
        throw new NotImplementedException();
    }
}
