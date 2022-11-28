using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WbApp.Interfaces;

namespace WbApp.Services.GetPaymentDetails
{
    public class PaymentDetailsHandler : IRequestHandler<PaymentDetailsQuery, PaymentDetailsModel>
    {
        private readonly IWbReadOnlyRepository _readRepository;
      
        public PaymentDetailsHandler(IWbReadOnlyRepository readRepository)
        {
            _readRepository = readRepository;
           
        }
        public async Task<PaymentDetailsModel> Handle(PaymentDetailsQuery request, CancellationToken cancellationToken)
        {
            var res = await _readRepository.GetPaymentDetails(request);
            decimal? b = res.BankAmount == null ? 0 : res.BankAmount;
            decimal? c = res.CashAmount == null ? 0 : res.CashAmount;
            decimal? t = res.TerminalAmount == null ? 0 : res.TerminalAmount;
            res.CreateMonth = res.CreateDate.Month==01? res.CreateDate.Day+ "იანვარს":
               res.CreateDate.Month == 02 ? res.CreateDate.Day + " თებერვალს": 
               res.CreateDate.Month == 03 ? res.CreateDate.Day + " მარტს" :
               res.CreateDate.Month == 04 ? res.CreateDate.Day + " აპრილს" :
               res.CreateDate.Month == 05 ? res.CreateDate.Day + " მაისს" :
               res.CreateDate.Month == 06 ? res.CreateDate.Day + " ივნისს" :
               res.CreateDate.Month == 07 ? res.CreateDate.Day + " ივლისს" :
               res.CreateDate.Month == 08 ? res.CreateDate.Day + " აგვისტოს" :
               res.CreateDate.Month == 09 ? res.CreateDate.Day + " სექტემბერს" :
               res.CreateDate.Month == 10 ? res.CreateDate.Day + " ოქტომბერს" :
               res.CreateDate.Month == 11 ? res.CreateDate.Day + " ნოემბერს" :
               res.CreateDate.Month == 11 ? res.CreateDate.Day + " დეკემბერს" : "";

            res.TAmount = $"{res.TerminalAmount :C}".Replace("$", "").Replace("¤","");
            res.BAmount = $"{res.BankAmount  :C}".Replace("$", "");
            res.CAmount = $"{res.CashAmount  :C}".Replace("$", "");
            res.SAmount = $"{(b + c + t)  :C}".Replace("$", "");
            res.SumAmount = b + c + t;
            try
            {
                res.Count =   res.details.Count();
              
            }
            catch(Exception e)
            {
                res.Count = 0;
            
            }
            return res;
        }
    }
}
