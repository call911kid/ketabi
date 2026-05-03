using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Interfaces.Repositories;
using Ketabi.Infrastructure.Persistence;

namespace Ketabi.Infrastructure.Repositories;

internal class RequestRepository : GenericRepository<Request>, IRequestRepository
{
    public RequestRepository(KetabiDbContext context) : base(context)
    {
    }
}
