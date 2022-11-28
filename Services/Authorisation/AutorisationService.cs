using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WbApp.Interfaces;
using WbApp.Models;

namespace WbApp.Services.Authorisation
{
    public class AutorisationService : IRequestHandler<AuthorisationQuery, ActionResultModel>
    {
        private readonly IWbReadOnlyRepository _readRepository;
        public AutorisationService(IWbReadOnlyRepository readRepository)
        {
            _readRepository = readRepository;
        }
        public async  Task<ActionResultModel> Handle(AuthorisationQuery request, CancellationToken cancellationToken)
        {
            ActionResultModel res = new ActionResultModel();
            res = await _readRepository.Authrisation(request);
            return res;
        }
    }
}
