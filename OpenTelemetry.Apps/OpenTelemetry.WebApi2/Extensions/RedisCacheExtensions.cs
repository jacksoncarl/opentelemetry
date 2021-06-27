using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTelemetry.WebApi2.Extensions
{
    //https://github.com/dotnet/aspnetcore/issues/28379#issuecomment-536659503
    public static class RedisCacheExtensions
    {
        /// <summary>
        /// The ASP.NET Core's RedisCache does not expose the ConnectionMultiplexer required for more advanced Redis scenarios
        /// and it is recommended to have just one ConnectionMultiplexer.
        /// We are left with no option but to use reflection to get a hold of the connection.
        /// </summary>
        public static async Task<ConnectionMultiplexer> GetConnectionAsync(this RedisCache cache, CancellationToken cancellationToken = default)
        {
            //ensure connection is established
            await ((Task)typeof(RedisCache).InvokeMember("ConnectAsync", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, cache, new object[] { cancellationToken }));

            //get connection multiplexer
            var fi = typeof(RedisCache).GetField("_connection", BindingFlags.Instance | BindingFlags.NonPublic);
            var connection = (ConnectionMultiplexer)fi.GetValue(cache);
            return connection;
        }

        public static ConnectionMultiplexer GetConnection(this RedisCache cache, CancellationToken cancellationToken = default)
        {
            //ensure connection is established
            typeof(RedisCache).InvokeMember("Connect", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, cache, new object[] { });

            //get connection multiplexer
            var fi = typeof(RedisCache).GetField("_connection", BindingFlags.Instance | BindingFlags.NonPublic);
            var connection = (ConnectionMultiplexer)fi.GetValue(cache);
            return connection;
        }
    }

}
