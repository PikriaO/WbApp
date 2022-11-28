using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WbApp.Interfaces;
using WbApp.Services.CashDesk.FillCloseTransactions;
using WbApp.Services.CashDeskTransactions;
using WbApp.Services.Report;

namespace WbApp.Services.CashDesk.CashDeskTransactions
{
    public class CashDeskTransactionHandler : IRequestHandler<CashDeskTransactionsQuery, CashDeskTransactionsModel>
    {
        private readonly IWbReadOnlyRepository _readRepository;
        public CashDeskTransactionHandler(IWbReadOnlyRepository readRepository)
        {
            _readRepository = readRepository;
        }
        public async Task<CashDeskTransactionsModel> Handle(CashDeskTransactionsQuery request, CancellationToken cancellationToken)
        {
            var datalist = new List<CashDeskTransactionsModel.CashTransaction>();
            var bal = await _readRepository.GetCashDeskBalance(request);
            var list = new List<CashDeskTransactionsModel.CashTransaction>();
            string[] opclass = { "Cashdesc.In", "Cashdesc.Out" };

            var cash = await _readRepository.GetTransactions(new FillCloseTransactions.TransactionsQuery()
            {
                BranchID = request.BranchID,
                EndDate = request.EndDate,
                StartDate = request.StartDate,
                 Lang=request.Lang,
            });

            var tran = await _readRepository.GetCashDeskTransactions(request);
            decimal sum = bal.BankAmount + bal.CashAmount + bal.TerminalAmount;
            try
            {
                list = cash.tran.Where(p => p.OpClass == "Cashdesc.In" || p.OpClass == "Cashdesc.Out").Select(a => new CashDeskTransactionsModel.CashTransaction
                {
                    Amount = a.Amount,
                    CAmount = $"{a.Amount:C}".Replace("$", "").Replace("¤", ""),
                    CreateDate = a.TranDate,
                    Description = a.Description,
                    OperatorName = a.Operator,
                     
                }).ToList();

            }
            catch (Exception e)
            {

            }
            return new CashDeskTransactionsModel()
            {
                BankAmount = bal.BankAmount,
                CountBank = bal.CountBank,
                BAmount = $"{bal.BankAmount:C}".Replace("$", "").Replace("¤", ""),
                CashAmount = bal.CashAmount,
                CAmount = $"{bal.CashAmount:C}".Replace("$", "").Replace("¤", ""),
                CountCash = bal.CountCash,
                CashDeskbalance = bal.CashDeskbalance,
                CBalance = $"{bal.CashDeskbalance:C}".Replace("$", "").Replace("¤", ""),
                TerminalAmount = bal.TerminalAmount,
                TAmount = $"{bal.TerminalAmount:C}".Replace("$", "").Replace("¤", ""),
                CountTerminal = bal.CountTerminal,
                transactions = tran.transactions,
                cashTransactions = list,
                CountSum = bal.CountSum,
                SAmount = $"{bal.SumAmount:C}".Replace("$", "").Replace("¤", ""),
                SumAmount = bal.SumAmount,
                TrAccount = bal.TrAccount,
                AccountNumber = bal.AccountNumber
            };

        }
    }
}
