using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class VietnamTimeInterceptor : ISaveChangesInterceptor
    {
        private static readonly TimeZoneInfo vietnamZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        public ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var dbContext = eventData.Context;
            if (dbContext == null) return new ValueTask<InterceptionResult<int>>(result);

            foreach (var entry in dbContext.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    foreach (var property in entry.Properties)
                    {
                        if (property.Metadata.ClrType == typeof(DateTime) && property.CurrentValue != null)
                        {
                            var dateTime = (DateTime)property.CurrentValue;
                            if (dateTime != DateTime.MinValue)
                            {
                                if (dateTime.Kind == DateTimeKind.Utc || dateTime.ToString().EndsWith("Z"))
                                {
                                    var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, vietnamZone);
                                    property.CurrentValue = DateTime.SpecifyKind(vietnamTime, DateTimeKind.Unspecified);
                                }
                            }
                        }
                    }
                }
            }

            return new ValueTask<InterceptionResult<int>>(result);
        }
        public ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            return new ValueTask<int>(result);
        }

        public void SaveChangesFailed(DbContextErrorEventData eventData)
        {
            // Xử lý lỗi nếu cần
        }
    }
}