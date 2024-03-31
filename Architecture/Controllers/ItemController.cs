using Architecture.Repositories;
using Architecture.Services;
using Architecture.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Architecture.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemController : ControllerBase
{
    private readonly IItemService _itemService;
}
