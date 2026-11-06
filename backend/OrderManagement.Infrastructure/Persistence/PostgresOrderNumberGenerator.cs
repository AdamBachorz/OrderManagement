using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Abstractions;

namespace OrderManagement.Infrastructure.Persistence
{
    public sealed class PostgresOrderNumberGenerator(AppDbContext db) : IOrderNumberGenerator
    {
        public const string SequenceName = "order_number_seq";

        public async Task<string> NextAsync(CancellationToken cancellationToken = default)
        {
            // For scalar queries, EF requires a column named "Value".
            // Name of the sequence in SQL must match SequenceName.
            var next = await db.Database
                .SqlQuery<long>($"""SELECT nextval('order_number_seq') AS "Value" """)
                .SingleAsync(cancellationToken);

            return $"ORD-{next:D6}";
        }
    }
}
