using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
namespace SignalR_Tests
{
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.BearerToken)]
    public class ServerHub : Hub<IClient>
    {
        [HubMethodName("PaymentsUpdate")]
        public async Task PaymentsUpdate(Payment payment)
        {
            await Clients.All.paymentsUpdate(payment);
        }
    }

    public interface IClient
    {
        Task paymentsUpdate(Payment payment);
    }
}
