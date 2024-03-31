using Architecture.Repositories.Interfaces;

namespace Architecture.Repositories;
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
}
