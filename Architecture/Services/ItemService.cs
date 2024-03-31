using Architecture.Repositories;
using Architecture.Repositories.Interfaces;
using Architecture.Services.Interfaces;

namespace Architecture.Services;

public class ItemService : IItemService, INewService
{
    private readonly IItemRepository _repository;

    public ItemService(IItemRepository repository)
    {
        _repository = repository;
    }
}
