using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WbApp.Interfaces;

namespace WbApp.Services.UserRegistration
{
    public class UserRegistrationHandler : IRequestHandler<UserRegistrationQuery, UserRegistrationModel>
    {

        private readonly IWbWriteOnlyRepository _writeRepository;
        public UserRegistrationHandler(IWbWriteOnlyRepository writeRepository)
        {
            _writeRepository = writeRepository;
        }
        public async Task<UserRegistrationModel> Handle(UserRegistrationQuery request, CancellationToken cancellationToken)
        {
            var res = await _writeRepository.UserRegistration(request);

            return res;
        }
    }
}
