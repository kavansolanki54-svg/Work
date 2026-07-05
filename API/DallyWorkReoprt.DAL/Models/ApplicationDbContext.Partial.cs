using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DallyWorkReoprt.DAL.Models
{
    public partial class ApplicationDbContext
    {
        public override int SaveChanges()
        {
            HandleSoftDelete();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            HandleSoftDelete();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void HandleSoftDelete()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Deleted);

            foreach (var entry in entries)
            {
                var activeStatusProperty = entry.Metadata.FindProperty("ActiveStatus");
                if (activeStatusProperty != null)
                {
                    entry.State = EntityState.Modified;

                    if (activeStatusProperty.ClrType == typeof(byte))
                        entry.Property("ActiveStatus").CurrentValue = (byte)0;
                    else if (activeStatusProperty.ClrType == typeof(bool))
                        entry.Property("ActiveStatus").CurrentValue = false;
                    else if (activeStatusProperty.ClrType == typeof(int))
                        entry.Property("ActiveStatus").CurrentValue = 0;
                }
            }
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var activeStatusProperty = entityType.FindProperty("ActiveStatus");
                if (activeStatusProperty != null)
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var propertyMethodInfo = typeof(EF).GetMethod("Property")!.MakeGenericMethod(activeStatusProperty.ClrType);
                    var efPropertyCall = Expression.Call(null, propertyMethodInfo, parameter, Expression.Constant("ActiveStatus"));

                    Expression constant;
                    if (activeStatusProperty.ClrType == typeof(byte))
                        constant = Expression.Constant((byte)0, typeof(byte));
                    else if (activeStatusProperty.ClrType == typeof(bool))
                        constant = Expression.Constant(false, typeof(bool));
                    else if (activeStatusProperty.ClrType == typeof(int))
                        constant = Expression.Constant(0, typeof(int));
                    else
                        continue;

                    var body = Expression.NotEqual(efPropertyCall, constant);
                    var lambda = Expression.Lambda(body, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }
    }
}
