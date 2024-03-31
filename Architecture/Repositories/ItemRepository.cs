using Architecture.Controllers;
using Architecture.Repositories.Interfaces;
using Architecture.Services;

namespace Architecture.Repositories;
public class ItemRepository : GenericRepository<ItemRepository>, IItemRepository
{
    private ItemRepository() { }
}
