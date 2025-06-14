using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TicketingSystem.Models.DTO.Requests.Agency;
using TicketingSystem.Models.DTO.Responses.Agency;
using TicketingSystem.Models.Entities.Agency;
using TicketingSystem.Services.Repositories;

namespace TicketingSystem.Controllers.Api;

public class AgencyController(IAgencyRepository repository, Mapper mapper)
    : CrudController<Agency, Ulid, AgencyResponse, CreateAgency, EditAgency>(repository, mapper) { }
