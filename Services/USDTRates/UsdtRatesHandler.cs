using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WbApp.Interfaces;

namespace WbApp.Services.USDTRates
{
    public class UsdtRatesHandler : IRequestHandler<UsdtRatesCommand, UsdtRatesModel>
    {
        private readonly IWbReadOnlyRepository _readRepository;
        private readonly IWbWriteOnlyRepository _writeRepository;
        public UsdtRatesHandler(IWbReadOnlyRepository readRepository, IWbWriteOnlyRepository writeRepository)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
        }
        public async Task<UsdtRatesModel> Handle(UsdtRatesCommand request, CancellationToken cancellationToken)
        {
            var data = new UsdtRatesModel();
            if (request.ActionType == 1)
            {
                data = await _readRepository.GetusdtRates(request);

            }
            else
            {
                if (request.Mode == 2)
                {
                    foreach (var a in request.usdtarray)
                    {
                        data = await _writeRepository.UpdateUsdtRate(new UsdtRatesCommand
                        {
                            Dt = request.Dt == null ? DateTime.Now : request.Dt,
                            Mode = request.Mode,
                            Price = a.Price,
                            Lang=a.Lang,
                            Usdt = a.Usdt, 
                        });
                    }
                }
                else
                {
                    data = await _writeRepository.UpdateUsdtRate(new UsdtRatesCommand
                    {
                        Dt = request.Dt == null ? DateTime.Now : request.Dt,
                        Mode = request.Mode,
                        Price = request.Price,
                        Usdt = request.Usdt,

                    });
                }
            }
            return data;
        }
    }
}
