using Microsoft.EntityFrameworkCore;
using SmartFood.Domain.Models.Interfaces;

namespace SmartFood.Domain.Utils;

public static class EntityFrameworkHelpers
{
    public static void DetachLocal<T>(this ApplicationDbContext context, T t, Guid entryId)
        where T : class, IODataEntity
    {
        var local = context.Set<T>()
            .Local
            .FirstOrDefault(entry => entry.Id.Equals(entryId));
        if (!(local is null)) context.Entry(local).State = EntityState.Detached;
        context.Entry(t).State = EntityState.Modified;
    }
}