using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WbApp.Extentions;
using WbApp.Helpers.StoredProcedure;
using WbApp.Interfaces;
using WbApp.Models;
using WbApp.Services.Authorisation;
using WbApp.Services.CashDesk;
using WbApp.Services.CashDesk.FillCloseTransactions;
using WbApp.Services.CashDeskTransactions;
using WbApp.Services.CheckPersonalID;
using WbApp.Services.CheckVoucher;
using WbApp.Services.CloseFillCashDesk;
using WbApp.Services.ConfirmOperation;
using WbApp.Services.GetPaymentDetails;
using WbApp.Services.MakeOperation;
using WbApp.Services.MakeOperationDetails;
using WbApp.Services.VouchersTracking.AddUpdateVouchers;
using WbApp.Services.VouchersTracking.VouchersList;
using static WbApp.Services.GetPaymentDetails.PaymentDetailsModel;
using WbApp.Services.USDTRates;
using WbApp.Services.Branches;
using WbApp.Services.UserRegistration;
using WbApp.Services.GetUser;
using WbApp.Services.CalcUsdtPrice;
using WbApp.Services.BuyNewUsdt.MakeOperation;
using WbApp.Services.BuyNewUsdt.GetOperations;
using WbApp.Services.BuyNewUsdt.ConfirmUsdtCashIn;
using WbApp.Services.BuyNewUsdt.GetCode;
using WbApp.Services.CashDesk.VipCashDesk;

namespace WbApp.Services
{
    public class WbRepository : IWbWriteOnlyRepository, IWbReadOnlyRepository
    {


        private OracleConnection CreateWhiteBitConnection()
        {
            var appSettingsJson = AppSettingsJson.GetAppSettings();
            return new OracleConnection(appSettingsJson["ConnectionStrings:WhiteBit"]); //.Unprotect("b9334858-9639-4a38-8cde-2bc81a6e03b3"));

        }
        public async Task<ActionResultModel> Authrisation(AuthorisationQuery a)
        {
            ActionResultModel res = new ActionResultModel();
            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 0;// 500;
                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.checkuser",
                    BindByName = true
                };
                try
                {

                    cmd.Parameters.Add("p_username", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_username"].Value = a.UserName;

                    cmd.Parameters.Add("p_password", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_password"].Value = a.Password;

                    cmd.Parameters.Add("p_serviscentre", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_serviscentre"].Value = a.BranchID;

                    cmd.Parameters.Add("p_lang", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_lang"].Value = a.Lang;

                    cmd.Parameters.Add("p_error_desc", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_family", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_name", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_error_code", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_id_user", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("P_Role", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;


                    cmd.Parameters.Add("result", OracleDbType.Int32, 256).Direction = ParameterDirection.ReturnValue;
                    cmd.Connection = con;

                    cmd.ExecuteReader();


                    res.ActionResultCode = Convert.ToInt32(cmd.Parameters["p_error_code"].Value.ToString());
                    res.ActionResultMsg = cmd.Parameters["p_error_desc"].Value.ToString();
                    try
                    {
                        res.UserID = cmd.Parameters["p_id_user"].Value is DBNull ? 0 : Convert.ToInt32(cmd.Parameters["p_id_user"].Value.ToString());
                    }
                    catch (Exception e)
                    {
                        res.UserID = 0;
                    }
                    try
                    {
                        res.RoleID = cmd.Parameters["P_Role"].Value is DBNull ? 0 : Convert.ToInt32(cmd.Parameters["P_Role"].Value.ToString());
                    }
                    catch (Exception e)
                    {
                        res.RoleID = 0;
                    }
                    res.OperatorName = cmd.Parameters["p_name"].Value.ToString() + " " + cmd.Parameters["p_family"].Value.ToString();



                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;

                }
                finally
                {
                    con.Close();
                }

            }
            return res;

        }

        public async Task<CheckVoucherModel> CheckVoucher(CheckVoucherQuery a)
        {
            CheckVoucherModel res = new CheckVoucherModel();
            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {

                try
                {
                    con.Open();
                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;
                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.checkwhitebitcode",
                    BindByName = true
                };
                try
                {

                    cmd.Parameters.Add("p_code", OracleDbType.NVarchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_code"].Value = a.VoucherCode;

                    cmd.Parameters.Add("p_lang", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_lang"].Value = a.Lang;

                    cmd.Parameters.Add("p_error_desc", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_error_code", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_amount", OracleDbType.Decimal, 512).Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("result", OracleDbType.Int32, 256).Direction = ParameterDirection.ReturnValue;

                    cmd.Connection = con;
                    cmd.ExecuteReader();

                    res.ActionResultCode = Convert.ToInt32(cmd.Parameters["p_error_code"].Value.ToString());
                    res.ActionResultMsg = cmd.Parameters["p_error_desc"].Value.ToString();
                    var am = cmd.Parameters["p_amount"].Value.ToString();
                    try
                    {
                        res.Amount = Convert.ToDecimal(am);
                    }
                    catch (Exception ex)
                    {
                        res.Amount = null;
                    }

                }
                catch (OracleException e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;
                    con.Close();
                }

                con.Close();
            }
            return res;

        }

        public async Task<PersonDataModel> CheckPersonalID(CheckPersonQuery a)
        {
            PersonDataModel res = new PersonDataModel();
            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {

                try
                {
                    con.Open();
                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 0;// 500;
                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.getclientparameters",
                    BindByName = true
                };
                try
                {

                    cmd.Parameters.Add("p_personalnumber", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_personalnumber"].Value = a.PersonalID;

                      cmd.Parameters.Add("p_inn", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_inn"].Value = a.Inn;

                    cmd.Parameters.Add("p_lang", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_lang"].Value = a.Lang;

                    cmd.Parameters.Add("p_error_desc", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_error_code", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_clientcategory", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_phone", OracleDbType.Varchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_name", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_surname", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_Email", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_org_name", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_phone1", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_Address", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_description", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_birthdate", OracleDbType.Date, 512).Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("result", OracleDbType.Int32, 256).Direction = ParameterDirection.ReturnValue;

                    cmd.Connection = con;
                    cmd.ExecuteReader();

                    res.ActionResultCode = Convert.ToInt32(cmd.Parameters["p_error_code"].Value.ToString());
                    res.ActionResultMsg = cmd.Parameters["p_error_desc"].Value.ToString();
                    res.Phone = cmd.Parameters["p_phone"].Value.ToString();
                    res.phone1 = cmd.Parameters["p_phone1"].Value.ToString();
                    res.Inn= cmd.Parameters["p_inn"].Value.ToString();
                    res.Address= cmd.Parameters["p_Address"].Value.ToString();
                    res.OrgName= cmd.Parameters["p_org_name"].Value.ToString();
                    res.Email= cmd.Parameters["p_email"].Value.ToString();
                    res.AdditionalInfo= cmd.Parameters["p_description"].Value.ToString();
                    try
                    {
                        var date = cmd.Parameters["p_birthdate"].ToNullableDateTime();
                        res.BirthDate = date?.ToString("yyyy-MM-dd");
                    }
                    catch(Exception e)
                    {

                    }
                    res.surname = cmd.Parameters["p_surname"].Value.ToString();
                    res.name = cmd.Parameters["p_name"].Value.ToString();



                }
                catch (OracleException e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;
                    con.Close();
                }

                con.Close();
            }
            return res;

        }
        //ვაუჩერის დიდი გატარება
        public async Task<MakeOperationModel> MakeOperation(MakeOperationCommand a)
        {
            var result = 0;
            MakeOperationModel res = new MakeOperationModel();
            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;
                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.addmultipayment",
                    BindByName = true
                };
                try
                {

                    //cmd.Parameters.Add("p_personalnumber", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    //cmd.Parameters["p_personalnumber"].Value = a.PersonalID;

                    cmd.Parameters.Add("p_lang", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_lang"].Value = a.Lang;

                    cmd.Parameters.Add("p_cash_amount", OracleDbType.Decimal, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_cash_amount"].Value = a.CashAmount;

                    cmd.Parameters.Add("p_terminal_amount", OracleDbType.Decimal, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_terminal_amount"].Value = a.TerminalAmount;

                    cmd.Parameters.Add("p_bankpm_amount", OracleDbType.Decimal, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_bankpm_amount"].Value = a.BankAmount;

                    cmd.Parameters.Add("p_userid", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_userid"].Value = a.OperatorID;

                    cmd.Parameters.Add("p_branchid", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_branchid"].Value = a.BranchID;

                    cmd.Parameters.Add("p_cardcount", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_cardcount"].Value = a.CardCount;

                    cmd.Parameters.Add("p_mode", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_mode"].Value = 0;

                    cmd.Parameters.Add("p_name", OracleDbType.NVarchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_name"].Value = a.Name;

                    cmd.Parameters.Add("p_surname", OracleDbType.NVarchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_surname"].Value = a.Surname;

                    cmd.Parameters.Add("p_personalid", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_personalid"].Value = a.PersonalID;

                    cmd.Parameters.Add("p_birthdate", OracleDbType.Date, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_birthdate"].Value = a.BirthDate.Date;

                    cmd.Parameters.Add("p_phone", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_phone"].Value = a.Phone;

                    cmd.Parameters.Add("p_error_desc", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_error_code", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_paymentid", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("result", OracleDbType.Int32, 256).Direction = ParameterDirection.ReturnValue;

                    cmd.Connection = con;
                    cmd.ExecuteReader();

                    res.PaymentID = Convert.ToInt32(cmd.Parameters["p_paymentid"].Value.ToString());
                    try
                    {
                        result = Convert.ToInt32(cmd.Parameters["result"].Value.ToString());
                        res.ActionResultCode = Convert.ToInt32(cmd.Parameters["p_error_code"].Value.ToString());
                    }
                    catch (Exception ex)
                    {
                        res.ActionResultCode = res.ActionResultCode == 0 ? result : res.ActionResultCode;
                    }

                    res.ActionResultMsg = cmd.Parameters["p_error_desc"].Value.ToString();





                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;
                    con.Close();
                }

                con.Close();
            }
            return res;

        }
        //ვაუჩერენის დამატება
        public async Task<MakeOperationDetailsModel> MakeOperationDetails(MakeOperationDetailsCommand a)
        {
            var result = 0;
            MakeOperationDetailsModel res = new MakeOperationDetailsModel();
            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;
                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.addmultipaymentdetails",
                    BindByName = true
                };
                try
                {


                    cmd.Parameters.Add("p_lang", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_lang"].Value = a.Lang;



                    cmd.Parameters.Add("p_paymentid", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_paymentid"].Value = a.PaymentID;


                    cmd.Parameters.Add("p_code", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_code"].Value = a.VoucherCode;



                    cmd.Parameters.Add("p_error_desc", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_error_code", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("result", OracleDbType.Int32, 256).Direction = ParameterDirection.ReturnValue;

                    cmd.Connection = con;
                    cmd.ExecuteReader();

                    try
                    {
                        result = Convert.ToInt32(cmd.Parameters["result"].Value.ToString());
                        res.ActionResultCode = Convert.ToInt32(cmd.Parameters["p_error_code"].Value.ToString());
                    }
                    catch (Exception ex)
                    {
                        res.ActionResultCode = res.ActionResultCode == 0 ? result : res.ActionResultCode;
                    }

                    res.ActionResultMsg = cmd.Parameters["p_error_desc"].Value.ToString();
                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;
                }
                finally
                {
                    con.Close();
                }
            }
            return res;

        }
        //ვაუჩერების დადასტურება
        public async Task<ConfirmOperationModel> ConfirmOperation(ConfirmOperationCommand a)
        {
            var result = 0;
            ConfirmOperationModel res = new ConfirmOperationModel();
            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;
                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.conformpayment",
                    BindByName = true
                };
                try
                {

                    cmd.Parameters.Add("p_lang", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_lang"].Value = a.Lang;


                    cmd.Parameters.Add("p_paymentid", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_paymentid"].Value = a.PaymentID;



                    cmd.Parameters.Add("p_error_desc", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_error_code", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("result", OracleDbType.Int32, 256).Direction = ParameterDirection.ReturnValue;

                    cmd.Connection = con;
                    cmd.ExecuteReader();

                    try
                    {
                        result = Convert.ToInt32(cmd.Parameters["result"].Value.ToString());
                        res.ActionResultCode = Convert.ToInt32(cmd.Parameters["p_error_code"].Value.ToString());
                    }
                    catch (Exception ex)
                    {
                        res.ActionResultCode = res.ActionResultCode == 0 ? result : res.ActionResultCode;
                    }

                    res.ActionResultMsg = cmd.Parameters["p_error_desc"].Value.ToString();
                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;
                }
                finally
                {
                    con.Close();
                }
            }
            return res;

        }
        //ვაუჩერის დეტალები 
        public async Task<PaymentDetailsModel> GetPaymentDetails(PaymentDetailsQuery a)
        {

            var c = 0;
            PaymentDetailsModel res = new PaymentDetailsModel();
            var datalist = new List<PaymentDetailsModel.Detail>();


            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {

                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.getpaymentsdetails",
                    BindByName = true
                };
                try
                {


                    cmd.Parameters.Add("p_lang", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_lang"].Value = a.Lang;


                    cmd.Parameters.Add("p_paymentid", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_paymentid"].Value = a.PaymentID;



                    cmd.Parameters.Add("result", OracleDbType.RefCursor, 256).Direction = ParameterDirection.ReturnValue;

                    cmd.Connection = con;

                    OracleDataReader myReader = default(OracleDataReader);
                    myReader = cmd.ExecuteReader();

                    while (myReader.Read())
                    {
                        res.Name = myReader["Name"].ToString();
                        res.Surname = myReader["Surname"].ToString();
                        res.PersonID = myReader["PersonID"].ToString();
                        res.Comment = myReader["Commentt"].ToString();
                        res.Phone = myReader["Phone"].ToString();
                        res.BirthDate = Convert.ToDateTime(myReader["BirthDate"]);
                        res.CreateDate = Convert.ToDateTime(myReader["CreateDate"]);
                        res.CashAmount = myReader["Cash_amount"] is DBNull ? null : Convert.ToDecimal(myReader["Cash_amount"]);
                        res.CAmount = $"{res.CashAmount:C}".Replace("$", "").Replace("¤", "");
                        res.BankAmount = myReader["BankPm_Amount"] is DBNull ? null : Convert.ToDecimal(myReader["BankPm_Amount"]);
                        res.BAmount = $"{res.BankAmount:C}".Replace("$", "").Replace("¤", "");
                        res.TerminalAmount = myReader["terminal_Amount"] is DBNull ? 0 : Convert.ToDecimal(myReader["terminal_Amount"]);
                        res.TAmount = $"{res.TerminalAmount:C}".Replace("$", "").Replace("¤", "");
                        res.BranchName = myReader["Branch_Name"].ToString();
                        res.BranchAddress = myReader["BLD"].ToString();
                        res.OperatorName = myReader["Oper_Name"].ToString();
                        res.SumAmount = res.BankAmount + res.CashAmount + res.TerminalAmount;
                        res.SAmount = $"{res.SumAmount:C}".Replace("$", "").Replace("¤", "");

                        PaymentDetailsModel.Detail detail = new PaymentDetailsModel.Detail();
                        c++;
                        detail.Code = myReader["Code"].ToString();
                        detail.Amount = Convert.ToDecimal(myReader["Amount"]);
                        detail.ConvertedAmount = $"{detail.Amount:C}".Replace("$", "").Replace("¤", "");
                        detail.Count += c;

                        datalist.Add(detail);

                    }
                    res.details = datalist;
                    myReader.Close();

                }
                catch (Exception e)
                {


                }
                finally
                {

                    con.Close();
                }
            }
            return res;

        }
        //სალაროს გვერდზე დახატული ტრანზაქციები
        public async Task<CashDeskTransactionsModel> GetCashDeskTransactions(CashDeskTransactionsQuery a)
        {
            CashDeskTransactionsModel res = new CashDeskTransactionsModel();
            var datalist = new List<CashDeskTransactionsModel.Transaction>();
            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {

                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.getmultipaymentdetails",
                    BindByName = true
                };
                try
                {


                    cmd.Parameters.Add("p_lang", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_lang"].Value = a.Lang;

                    cmd.Parameters.Add("p_branchid", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_branchid"].Value = a.BranchID;

                    cmd.Parameters.Add("p_stdate", OracleDbType.Date, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_stdate"].Value = a.StartDate;

                    cmd.Parameters.Add("p_enddate", OracleDbType.Date, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_enddate"].Value = a.EndDate;

                    cmd.Parameters.Add("result", OracleDbType.RefCursor, 256).Direction = ParameterDirection.ReturnValue;

                    cmd.Connection = con;

                    OracleDataReader myReader = default(OracleDataReader);
                    myReader = cmd.ExecuteReader();

                    while (myReader.Read())
                    {
                        CashDeskTransactionsModel.Transaction detail = new CashDeskTransactionsModel.Transaction();
                        detail.PaymentID = Convert.ToInt32(myReader["PaymentID"]);
                        detail.Name = myReader["Name"].ToString();
                        detail.Surname = myReader["Surname"].ToString();
                        detail.CreateDate = Convert.ToDateTime(myReader["CreateDate"]);
                        detail.BankAmount = myReader["Bankpm_Amount"] is DBNull ? 0 : Convert.ToDecimal(myReader["Bankpm_Amount"]);
                        detail.BAmount = $"{detail.BankAmount:C}".Replace("$", "").Replace("¤", "");
                        detail.TerminalAmount = myReader["Terminal_Amount"] is DBNull ? 0 : Convert.ToDecimal(myReader["Terminal_Amount"]);
                        detail.TAmount = $"{detail.TerminalAmount:C}".Replace("$", "").Replace("¤", "");
                        detail.CashAmount = myReader["Cash_amount"] is DBNull ? 0 : Convert.ToDecimal(myReader["Cash_amount"]);
                        detail.CAmount = $"{detail.CashAmount:C}".Replace("$", "").Replace("¤", "");
                        detail.OperatorName = myReader["Oper_Name"].ToString();
                        datalist.Add(detail);

                    }
                    res.transactions = datalist;
                    myReader.Close();

                }
                catch (Exception e)
                {


                }
                finally
                {

                    con.Close();
                }
            }
            return res;

        }
        //სალაროს ბალანსი და ბრუნვები
        public async Task<CashDeskTransactionsModel> GetCashDeskBalance(CashDeskTransactionsQuery a)
        {
            CashDeskTransactionsModel res = new CashDeskTransactionsModel();
            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {

                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.getsumamount",
                    BindByName = true
                };
                try
                {
                    cmd.Parameters.Add("p_lang", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_lang"].Value = a.Lang;


                    cmd.Parameters.Add("p_branchid", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_branchid"].Value = a.BranchID;

                    cmd.Parameters.Add("p_stdate", OracleDbType.Date, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_stdate"].Value = a.StartDate;

                    cmd.Parameters.Add("p_enddate", OracleDbType.Date, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_enddate"].Value = a.EndDate;

                    cmd.Parameters.Add("result", OracleDbType.RefCursor, 256).Direction = ParameterDirection.ReturnValue;

                    cmd.Connection = con;

                    OracleDataReader myReader = default(OracleDataReader);
                    myReader = cmd.ExecuteReader();

                    while (myReader.Read())
                    {
                        res.CashAmount = myReader["Cash_amount"] is DBNull ? 0 : Convert.ToDecimal(myReader["Cash_amount"]);
                        res.BankAmount = myReader["BankPm_Amount"] is DBNull ? 0 : Convert.ToDecimal(myReader["BankPm_Amount"]);
                        res.TerminalAmount = myReader["terminal_Amount"] is DBNull ? 0 : Convert.ToDecimal(myReader["terminal_Amount"]);
                        res.SumAmount = myReader["Amount"] is DBNull ? 0 : Convert.ToDecimal(myReader["Amount"]);
                        res.CountSum = myReader["Count"] is DBNull ? 0 : Convert.ToInt32(myReader["Count"]);
                        res.CountCash = myReader["Count_Cash_amount"] is DBNull ? 0 : Convert.ToInt32(myReader["Count_Cash_amount"]);
                        res.CardCount = myReader["CardCount"] is DBNull ? 0 : Convert.ToInt32(myReader["CardCount"]);
                        res.CountBank = myReader["Count_BankPm_Amount"] is DBNull ? 0 : Convert.ToInt32(myReader["Count_BankPm_Amount"]);
                        res.CountTerminal = myReader["Count_terminal_Amount"] is DBNull ? 0 : Convert.ToInt32(myReader["Count_terminal_Amount"]);
                        res.CashDeskbalance = myReader["Balance"] is DBNull ? 0 : Convert.ToDecimal(myReader["Balance"]);
                        res.BranchName = Convert.ToString(myReader["Name"]);
                        res.AccountNumber = Convert.ToString(myReader["AccountNumber"]);
                        res.TrAccount = Convert.ToString(myReader["TrAccount"]);
                    }
                    myReader.Close();

                }
                catch (Exception e)
                {


                }
                finally
                {

                    con.Close();
                }
            }
            return res;
        }
        //სალაროს გახსნა/დახურვა
        public async Task<CashDeskOperationModel> CloseFillCashDesk(CashDeskOperationCommand a)
        {

            CashDeskOperationModel res = new CashDeskOperationModel();
            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;
                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.cashdeskoperation",
                    BindByName = true
                };
                try
                {

                    cmd.Parameters.Add("p_lang", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_lang"].Value = a.Lang;

                    cmd.Parameters.Add("p_branchid", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_branchid"].Value = a.BranchID;

                    cmd.Parameters.Add("p_amount", OracleDbType.Decimal, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_amount"].Value = a.Amount;

                    cmd.Parameters.Add("p_userid", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_userid"].Value = a.OperatorID;

                    cmd.Parameters.Add("p_optype", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_optype"].Value = a.OpType;

                    cmd.Parameters.Add("p_error_desc", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_error_code", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_id", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("result", OracleDbType.Int32, 256).Direction = ParameterDirection.ReturnValue;

                    cmd.Connection = con;
                    cmd.ExecuteReader();


                    res.ActionResultCode = Convert.ToInt32(cmd.Parameters["p_error_code"].Value.ToString());
                    res.ActionResultMsg = cmd.Parameters["p_error_desc"].Value.ToString();





                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;
                    con.Close();
                }

                con.Close();
            }
            return res;

        }
        //ვაუჩერების ფაილის დამატება
        public async Task<AddVouchersModel> AddNewVouchers(VouchersList a)
        {
            var result = 0;
            AddVouchersModel res = new AddVouchersModel();
            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;
                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.AddUpdaWhiteBitCode",
                    BindByName = true
                };
                try
                {

                    cmd.Parameters.Add("p_lang", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_lang"].Value = a.Lang;

                    cmd.Parameters.Add("p_code", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_code"].Value = a.Code;

                    cmd.Parameters.Add("p_file_name", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_file_name"].Value = a.FileName;

                    cmd.Parameters.Add("p_price", OracleDbType.Decimal, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_price"].Value = a.Amount;

                    cmd.Parameters.Add("p_st", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_st"].Value = a.Mode;

                    cmd.Parameters.Add("p_sc", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_sc"].Value = a.BranchID;

                    cmd.Parameters.Add("p_usdt", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_usdt"].Value = a.Usdt;

                    cmd.Parameters.Add("p_isused", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_isused"].Value = 0;

                    cmd.Parameters.Add("p_id", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;


                    cmd.Parameters.Add("p_error_desc", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_error_code", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("result", OracleDbType.Int32, 256).Direction = ParameterDirection.ReturnValue;

                    cmd.Connection = con;
                    cmd.ExecuteReader();

                    try
                    {
                        result = Convert.ToInt32(cmd.Parameters["result"].Value.ToString());
                        res.ActionResultCode = Convert.ToInt32(cmd.Parameters["p_error_code"].Value.ToString());
                    }
                    catch (Exception ex)
                    {
                        res.ActionResultCode = res.ActionResultCode == 0 ? result : res.ActionResultCode;
                    }

                    res.ActionResultMsg = cmd.Parameters["p_error_desc"].Value.ToString();
                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;
                }
                finally
                {
                    con.Close();
                }
            }
            return res;

        }
        //გამოუყენებელი ვაუჩერების სია
        public async Task<GetVouchersListModel> GetVouchersList(GetVouchersQuery a)
        {

            GetVouchersListModel res = new GetVouchersListModel();
            var datalist = new List<GetVouchersListModel.NonUsedVoucher>();
            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {

                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.getvoucherlist",
                    BindByName = true
                };
                try
                {


                    cmd.Parameters.Add("p_lang", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_lang"].Value = a.Lang;

                    cmd.Parameters.Add("p_file_name", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_file_name"].Value = a.FileName;

                    cmd.Parameters.Add("p_sc", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_sc"].Value = a.BranchID;


                    cmd.Parameters.Add("result", OracleDbType.RefCursor, 256).Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add("p_error_desc", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_error_code", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;

                    cmd.Connection = con;

                    OracleDataReader myReader = default(OracleDataReader);
                    myReader = cmd.ExecuteReader();

                    while (myReader.Read())
                    {
                        GetVouchersListModel.NonUsedVoucher detail = new GetVouchersListModel.NonUsedVoucher();
                        detail.CreateDate = Convert.ToDateTime(myReader["CreateDate"]);
                        detail.Code = myReader["Code"].ToString();
                        detail.FIleName = myReader["FIle_Name"].ToString();
                        detail.BranchName = myReader["NAME"].ToString();
                        detail.Amount = myReader["price"] is DBNull ? 0 : Convert.ToDecimal(myReader["price"]);

                        datalist.Add(detail);

                    }
                    res.voucher = datalist;
                    myReader.Close();

                }
                catch (Exception e)
                {


                }
                finally
                {

                    con.Close();
                }
            }
            return res;

        }
        //ტრანზაქციები(გახსნა/დახურვის ტრანზაქცია)
        public async Task<TransactionsModel> GetTransactions(TransactionsQuery a)
        {


            TransactionsModel res = new TransactionsModel();
            var datalist = new List<TransactionsModel.Tran>();


            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {

                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.gettransactionlist",
                    BindByName = true
                };
                try
                {


                    cmd.Parameters.Add("p_lang", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_lang"].Value = a.Lang;

                    cmd.Parameters.Add("p_opclass", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;


                    cmd.Parameters.Add("p_branchid", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_branchid"].Value = a.BranchID;

                    cmd.Parameters.Add("p_stdate", OracleDbType.Date, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_stdate"].Value = a.StartDate;

                    cmd.Parameters.Add("p_enddate", OracleDbType.Date, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_enddate"].Value = a.EndDate;

                    cmd.Parameters.Add("result", OracleDbType.RefCursor, 256).Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add("p_error_desc", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_error_code", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;


                    cmd.Connection = con;

                    OracleDataReader myReader = default(OracleDataReader);
                    myReader = cmd.ExecuteReader();

                    while (myReader.Read())
                    {


                        TransactionsModel.Tran detail = new TransactionsModel.Tran();
                        detail.Operator = myReader["Name"].ToString() + " " + myReader["Family"].ToString();
                        detail.OpClass = myReader["OpClass"].ToString();
                        detail.Description = myReader["Description"].ToString();
                        detail.Amount = myReader["Amount"] is DBNull ? 0 : Convert.ToDecimal(myReader["Amount"]);
                        detail.TranDate = Convert.ToDateTime(myReader["Transactiondate"]);
                        detail.AmountC = $"{detail.Amount:C}".Replace("$", "").Replace("¤", "");


                        datalist.Add(detail);

                    }
                    res.tran = datalist;
                    myReader.Close();

                }
                catch (Exception e)
                {


                }
                finally
                {

                    con.Close();
                }
            }
            return res;

        }

        public async Task<string> GenerateQr(string Code)
        {

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(Code, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20, darkColor: Color.Black, lightColor: Color.White, drawQuietZones: false);
            MemoryStream memoryStream = new MemoryStream();
            qrCodeImage.Save(memoryStream, ImageFormat.Png);
            memoryStream.Position = 0;
            byte[] byteBuffer = memoryStream.ToArray();
            string base64String = Convert.ToBase64String(byteBuffer);
            return base64String;

        }

        public async Task<UsdtRatesModel> GetusdtRates(UsdtRatesCommand a)
        {
            int counter = 0;
            UsdtRatesModel res = new UsdtRatesModel();
            var datalist = new List<UsdtRatesModel.CurrentRate>();


            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {

                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.getsdtpriceratelist",
                    BindByName = true
                };
                try
                {
                    cmd.Parameters.Add("p_usdt", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters.Add("p_rateid", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;

                    cmd.Parameters.Add("p_lang", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_lang"].Value = a.Lang;

                    cmd.Parameters.Add("p_mode", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_mode"].Value = a.Mode;

                    cmd.Parameters.Add("p_dt", OracleDbType.Date, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_dt"].Value = a.Dt;

                    cmd.Parameters.Add("result", OracleDbType.RefCursor, 256).Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add("p_error_desc", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_error_code", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;

                    cmd.Connection = con;

                    OracleDataReader myReader = default(OracleDataReader);
                    myReader = cmd.ExecuteReader();

                    while (myReader.Read())
                    {

                        UsdtRatesModel.CurrentRate detail = new UsdtRatesModel.CurrentRate();
                        counter++;
                        detail.Id = counter;
                        detail.CreateDate = myReader["CreateDate"].ToNullableDateTime();
                        detail.Usdt = myReader["USDT"].ToNullableInt();
                        detail.Price = myReader["Price"].ToNullableDecimal();
                        try
                        {
                            detail.DT = myReader["dt"].ToNullableDateTime();
                            detail.DtFormatted = myReader["dt"].ToNullableDateTime() == null ? null : Convert.ToDateTime(myReader["dt"]).ToString("dd-MM-yyyy");
                        }
                        catch (Exception e)
                        {

                        }
                        detail.FormatedDate = myReader["CreateDate"].ToNullableDateTime() == null ? null : Convert.ToDateTime(myReader["CreateDate"]).ToString("dd-MM-yyyy HH:mm");
                        datalist.Add(detail);

                    }
                    res.currentRates = datalist;
                    myReader.Close();

                }
                catch (Exception e)
                {
                    res.ActionResultCode = 500;
                    res.ActionResultMsg = e.Message;
                }
                finally
                {

                    con.Close();
                }
            }
            return res;

        }

        public async Task<UsdtRatesModel> UpdateUsdtRate(UsdtRatesCommand a)
        {
            var result = 0;
            UsdtRatesModel res = new UsdtRatesModel();
            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;
                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.AddUPDusdtpricerate",
                    BindByName = true
                };
                try
                {

                    cmd.Parameters.Add("p_lang", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_lang"].Value = a.Lang;

                    cmd.Parameters.Add("p_dt", OracleDbType.Date, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_dt"].Value = a.Dt;

                    cmd.Parameters.Add("p_usdt", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_usdt"].Value = a.Usdt;

                    cmd.Parameters.Add("p_price", OracleDbType.Decimal, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_price"].Value = a.Price;

                    cmd.Parameters.Add("p_currency", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_currency"].Value = "GEL";

                    cmd.Parameters.Add("p_isActive", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_isActive"].Value = 1;


                    cmd.Parameters.Add("p_mode", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_mode"].Value = a.Mode;

                    cmd.Parameters.Add("result", OracleDbType.Int32, 256).Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add("p_RateId", OracleDbType.Int32, 256).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_error_desc", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_error_code", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;

                    cmd.Connection = con;
                    cmd.ExecuteReader();

                    try
                    {
                        res.ActionResultCode = Convert.ToInt32(cmd.Parameters["p_error_code"].Value.ToString());
                    }
                    catch (Exception ex)
                    {
                        res.ActionResultCode = res.ActionResultCode;
                    }

                    res.ActionResultMsg = cmd.Parameters["p_error_desc"].Value.ToString();
                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;
                }
                finally
                {
                    con.Close();
                }
            }
            return res;

        }

        public async Task<ActionResultModel> GetBranches(GetBranchesCommand a)
        {

            ActionResultModel res = new ActionResultModel();
            var datalist = new List<ActionResultModel.Branch>();
            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {

                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.getbranches",
                    BindByName = true
                };
                try
                {
                    cmd.Parameters.Add("result", OracleDbType.RefCursor, 256).Direction = ParameterDirection.ReturnValue;

                    cmd.Connection = con;

                    OracleDataReader myReader = default(OracleDataReader);
                    myReader = cmd.ExecuteReader();

                    while (myReader.Read())
                    {

                        ActionResultModel.Branch detail = new ActionResultModel.Branch();
                        detail.Name = myReader["Name"].ToNullableString();
                        detail.Address = myReader["BLD"].ToNullableString();
                        detail.Id = myReader["ID"].ToNullableInt();
                        datalist.Add(detail);
                    }
                    res.branches = datalist;
                    myReader.Close();
                }
                catch (Exception e)
                {
                    res.ActionResultCode = 500;
                    res.ActionResultMsg = e.Message;
                }
                finally
                {
                    con.Close();
                }
            }
            return res;

        }

        public async Task<UserRegistrationModel> UserRegistration(UserRegistrationQuery a)
        {
            UserRegistrationModel res = new UserRegistrationModel();
            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;
                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.ADDUPDCLIENTPARAMETERS",
                    BindByName = true
                };
                try
                {

                    cmd.Parameters.Add("P_LANG", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["P_LANG"].Value = a.Lang;

                    cmd.Parameters.Add("P_NAME", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["P_NAME"].Value = a.Name;

                    cmd.Parameters.Add("P_SURNAME", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["P_SURNAME"].Value = a.Surname;

                    cmd.Parameters.Add("P_PERSONAL_CODE", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["P_PERSONAL_CODE"].Value = a.PersonalID;

                    cmd.Parameters.Add("P_BIRTHDATE", OracleDbType.Date, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["P_BIRTHDATE"].Value = a.BDate;

                    cmd.Parameters.Add("P_PHONE", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["P_PHONE"].Value = a.Phone;

                    cmd.Parameters.Add("P_ISLEGAL", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["P_ISLEGAL"].Value = a.IsLegal;

                    cmd.Parameters.Add("P_ADDRESS", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["P_ADDRESS"].Value = a.Address;

                    cmd.Parameters.Add("P_DESCRIPTION", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["P_DESCRIPTION"].Value = a.Description;

                    cmd.Parameters.Add("P_PHONE1", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["P_PHONE1"].Value = a.Phone1;

                    cmd.Parameters.Add("P_EMAIL", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["P_EMAIL"].Value = a.Email;

                    cmd.Parameters.Add("P_ORG_NAME", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["P_ORG_NAME"].Value = a.CompanyName;

                    cmd.Parameters.Add("P_INN", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["P_INN"].Value = a.Inn;

                    cmd.Parameters.Add("RESULT", OracleDbType.Int32, 512).Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add("P_CLIENTID", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_error_desc", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_error_code", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;

                    cmd.Connection = con;
                    cmd.ExecuteReader();

                    res.ActionResultCode = Convert.ToInt32(cmd.Parameters["RESULT"].Value.ToString());
                    try
                    {
                        res.ClinetID = Convert.ToInt32(cmd.Parameters["p_clientid"].Value.ToString());
                    }
                    catch (Exception e)
                    {

                    }
                    res.ActionResultMsg = cmd.Parameters["p_error_desc"].Value.ToString();


                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;
                }
                finally
                {
                    con.Close();
                }
            }
            return res;

        }

        public async Task<GetUserModel> GetUser(GetUserCommand a)
        {
            GetUserModel res = new GetUserModel();
            var datalist = new List<GetUserModel.User>();
            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {

                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.GETCLIENTS",
                    BindByName = true
                };
                try
                {
                    cmd.Parameters.Add("p_personal_code", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_personal_code"].Value = a.PersonalID;

                    cmd.Parameters.Add("p_inn", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_inn"].Value = a.Inn;

                    cmd.Parameters.Add("result", OracleDbType.RefCursor, 256).Direction = ParameterDirection.ReturnValue;

                    cmd.Connection = con;

                    OracleDataReader myReader = default(OracleDataReader);
                    myReader = cmd.ExecuteReader();

                    while (myReader.Read())
                    {

                        GetUserModel.User detail = new GetUserModel.User();

                        detail.Name = myReader["Name"].ToNullableString();
                        detail.Address = myReader["Address"].ToNullableString();
                        detail.PersonalCode = myReader["Personal_Code"].ToNullableString();
                        detail.BirthDate = myReader["BirthDate"].ToNullableDateTime();
                        detail.BDate = detail.BirthDate == null ? null : Convert.ToDateTime(myReader["BirthDate"]).ToString("dd-MM-yyyy");
                        detail.Phone = myReader["Phone"].ToNullableString();
                        detail.IsLegal = Convert.ToInt32(myReader["IsLegal"]);
                        detail.ClientID = Convert.ToInt32(myReader["ClientID"]);
                        detail.Inn = myReader["Inn"].ToNullableString();
                        detail.Email = myReader["Email"].ToNullableString();
                        detail.Surname = myReader["Surname"].ToNullableString();
                        detail.CompanyName = myReader["Org_Name"].ToNullableString();
                        detail.Phone1 = myReader["Phone1"].ToNullableString();
                        detail.AdditionalInfo = myReader["Description"].ToNullableString();
                        detail.CategoryID = myReader["ClientCategory"].ToNullableInt();
                        datalist.Add(detail);
                    }
                    res.users = datalist;
                    myReader.Close();
                }
                catch (Exception e)
                {
                    res.ActionresultCode = 500;
                    res.ActionResultMsg = e.Message;
                }
                finally
                {
                    con.Close();
                }
            }
            return res;

        }

        public async Task<CalcResult> CalcPrice(CalcCommand a)
        {
            CalcResult res = new CalcResult();
            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;
                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.Calcbyusdt",
                    BindByName = true
                };
                try
                {

                    cmd.Parameters.Add("p_amount", OracleDbType.Decimal, 256).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters["p_amount"].Value = a.Amount;

                    cmd.Parameters.Add("p_clientcategory", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_clientcategory"].Value = a.Category == null ? 1 : a.Category;

                    cmd.Parameters.Add("p_usdt", OracleDbType.Decimal, 256).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters["p_usdt"].Value = a.usdt;

                    cmd.Parameters.Add("p_lang", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_lang"].Value = a.Lang;

                    cmd.Parameters.Add("p_error_desc", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_error_code", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_feeamount", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_feeprc", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_usdtcurs", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;
                    //  cmd.Parameters.Add("p_amount", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;
                    //    cmd.Parameters.Add("p_usdt", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;


                    cmd.Parameters.Add("result", OracleDbType.Int32, 256).Direction = ParameterDirection.ReturnValue;
                    cmd.Connection = con;

                    cmd.ExecuteReader();


                    res.ActionResultCode = Convert.ToInt32(cmd.Parameters["p_error_code"].Value.ToString());
                    res.ActionResultMsg = cmd.Parameters["p_error_desc"].Value.ToString();

                    res.Amount = Convert.ToDecimal(cmd.Parameters["p_amount"].Value.ToString());
                    res.Usdt = Convert.ToDecimal(cmd.Parameters["p_usdt"].Value.ToString());
                    res.Rate = Convert.ToDecimal(cmd.Parameters["p_usdtcurs"].Value.ToString());
                    res.FeePersent = Convert.ToDecimal(cmd.Parameters["p_feeprc"].Value.ToString());
                    res.FeeAmount = Convert.ToDecimal(cmd.Parameters["p_feeamount"].Value.ToString());
                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;

                }
                finally
                {
                    con.Close();
                }

            }
            return res;

        }

        public async Task<MakeNewUsdtOperationModel> BuyNewUsdt(MakeNewUsdtOperationQuery a)
        {
            MakeNewUsdtOperationModel res = new MakeNewUsdtOperationModel();
            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;
                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.buysdt",
                    BindByName = true
                };
                try
                {

                    cmd.Parameters.Add("p_amount", OracleDbType.Decimal, 256).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters["p_amount"].Value = a.Amount;

                    cmd.Parameters.Add("p_feeamount", OracleDbType.Decimal, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_feeamount"].Value = a.FeeAmount;

                    cmd.Parameters.Add("p_usdt", OracleDbType.Decimal, 256).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters["p_usdt"].Value = a.Usdt;

                    cmd.Parameters.Add("p_userid", OracleDbType.Int32, 256).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters["p_userid"].Value = a.OperatorID;

                    cmd.Parameters.Add("p_clientid", OracleDbType.Int32, 256).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters["p_clientid"].Value = a.ClientID;

                    cmd.Parameters.Add("p_branchid", OracleDbType.Int32, 256).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters["p_branchid"].Value = a.BranchID;

                    cmd.Parameters.Add("p_lang", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_lang"].Value = a.Lang;

                    cmd.Parameters.Add("p_error_desc", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_error_code", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_operationid", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("result", OracleDbType.Int32, 256).Direction = ParameterDirection.ReturnValue;
                    cmd.Connection = con;

                    cmd.ExecuteReader();


                    res.ActionResultCode = Convert.ToInt32(cmd.Parameters["p_error_code"].Value.ToString());
                    res.OperationID = Convert.ToInt32(cmd.Parameters["p_operationid"].Value.ToString());
                    res.ActionResultMsg = cmd.Parameters["p_error_desc"].Value.ToString();

                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;

                }
                finally
                {
                    con.Close();
                }

            }
            return res;

        }

        public async Task<GetOperationsModel> GetOperations(GetOperationsCommand a)
        {


            GetOperationsModel res = new GetOperationsModel();
            var datalist = new List<GetOperationsModel.Transaction>();


            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {

                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.Getoperations",
                    BindByName = true
                };
                try
                {

                    cmd.Parameters.Add("p_lang", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_lang"].Value = a.Lang;


                    cmd.Parameters.Add("p_branchid", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_branchid"].Value = a.BranchID;

                    cmd.Parameters.Add("p_status", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_status"].Value = a.Status;

                    cmd.Parameters.Add("p_personal_code", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_personal_code"].Value = a.PersonalID;

                    cmd.Parameters.Add("p_inn", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_inn"].Value = a.Inn;

                    cmd.Parameters.Add("p_error_desc", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_error_code", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("result", OracleDbType.RefCursor, 256).Direction = ParameterDirection.ReturnValue;

                    cmd.Connection = con;

                    OracleDataReader myReader = default(OracleDataReader);
                    myReader = cmd.ExecuteReader();

                    while (myReader.Read())
                    {
                        GetOperationsModel.Transaction detail = new GetOperationsModel.Transaction();

                        detail.StatusName = myReader["StatusName"].ToString();
                        detail.Name = myReader["Name"].ToString();
                        detail.Surname = myReader["Surname"].ToString();
                        detail.Amount = Convert.ToDecimal(myReader["Amount"]);
                        try
                        {
                            detail.ConvertedAmount = $"{detail.Amount:C}".Replace("$", "").Replace("¤", "");
                        }
                        catch (Exception ex)
                        {
                            detail.ConvertedAmount = detail.Amount.ToString();
                        }
                        detail.Status = Convert.ToInt32(myReader["Status"]);
                        detail.BranchName = myReader["BranchName"].ToString();
                        detail.ClientID = Convert.ToInt32(myReader["ClientID"]);
                        detail.Inn = myReader["Inn"].ToString();
                        detail.OperationDate = Convert.ToDateTime(myReader["OperationDate"]);
                        detail.OperationID = Convert.ToInt32(myReader["ID"]);
                        //       detail.OperatorName = myReader["Operator"].ToString();
                        detail.OrgName = myReader["Org_name"].ToString();
                        detail.PersonalID = myReader["Personal_code"].ToString();
                        datalist.Add(detail);

                    }
                    res.transactions = datalist;
                    myReader.Close();

                }
                catch (Exception e)
                {

                }
                finally
                {

                    con.Close();
                }
            }
            return res;

        }

        public async Task<ConfirmCashInModel> ConfirmUsdtCashIn(ConfirmCashInCommand a)
        {
            ConfirmCashInModel res = new ConfirmCashInModel();
            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;
                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.ConfirmOperation",
                    BindByName = true
                };
                try
                {

                    cmd.Parameters.Add("p_operationid", OracleDbType.Int32, 256).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters["p_operationid"].Value = a.OperationID;

                    cmd.Parameters.Add("p_userid", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_userid"].Value = a.OperatorID;

                    //cmd.Parameters.Add("p_branchid", OracleDbType.Int32, 256).Direction = ParameterDirection.InputOutput;
                    //cmd.Parameters["p_branchid"].Value = a.BranchID; 

                    cmd.Parameters.Add("p_lang", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_lang"].Value = a.Lang;

                    cmd.Parameters.Add("p_error_desc", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_error_code", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("result", OracleDbType.Int32, 256).Direction = ParameterDirection.ReturnValue;
                    cmd.Connection = con;
                    cmd.ExecuteReader();

                    res.ActionResultCode = Convert.ToInt32(cmd.Parameters["p_error_code"].Value.ToString());
                    res.ActionResultMsg = cmd.Parameters["p_error_desc"].Value.ToString();

                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;

                }
                finally
                {
                    con.Close();
                }

            }
            return res;

        }

        public async Task<GetCodeModel> GetCode(GetCodeCommand a)
        {
            GetCodeModel res = new GetCodeModel();
            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;
                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.get_operationcode",
                    BindByName = true
                };
                try
                {

                    cmd.Parameters.Add("p_operationid", OracleDbType.Int32, 256).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters["p_operationid"].Value = a.OperationID;

                    //cmd.Parameters.Add("p_error_desc", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    //cmd.Parameters.Add("p_error_code", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("p_usdtcode", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_usdt", OracleDbType.Decimal, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_createdate", OracleDbType.Date, 512).Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("result", OracleDbType.Int32, 256).Direction = ParameterDirection.ReturnValue;
                    cmd.Connection = con;
                    cmd.ExecuteReader();

                    //res.ActionResultCode = Convert.ToInt32(cmd.Parameters["p_error_code"].Value.ToString());
                    //res.ActionResultMsg = cmd.Parameters["p_error_desc"].Value.ToString();

                    res.CreateDate = Convert.ToDateTime(cmd.Parameters["p_createdate"].Value.ToString());
                    res.Usdt = Convert.ToDecimal(cmd.Parameters["p_usdt"].Value.ToString());
                    res.Code = cmd.Parameters["p_usdtcode"].Value.ToString();

                }
                catch (Exception e)
                {
                    res.ActionResultMsg = e.Message;
                    res.ActionResultCode = 500;

                }
                finally
                {
                    con.Close();
                }

            }
            return res;

        }

        public async Task<VipCashDeskModel> GetVipCashDesk(VipCashDeskCommand a)
        {
            VipCashDeskModel res = new VipCashDeskModel();
            var datalist = new List<VipCashDeskModel.VipTransaction>();
            var appSettingsJson = AppSettingsJson.GetAppSettings();
            using (var con = CreateWhiteBitConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {

                }
                var cmd = new OracleCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "operations.servicecentercashbalance",
                    BindByName = true
                };
                try
                {


                    cmd.Parameters.Add("p_lang", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_lang"].Value = a.Lang;

                    cmd.Parameters.Add("p_branchid", OracleDbType.Int32, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_branchid"].Value = a.BranchID;

                    cmd.Parameters.Add("p_startdt", OracleDbType.Date, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_startdt"].Value = a.StartDate;

                    cmd.Parameters.Add("p_enddate", OracleDbType.Date, 256).Direction = ParameterDirection.Input;
                    cmd.Parameters["p_enddate"].Value = a.EndDate;

                    cmd.Parameters.Add("result", OracleDbType.Int32, 256).Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add("p_tramounts", OracleDbType.RefCursor, 256).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("freturn", OracleDbType.RefCursor, 256).Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("p_error_desc", OracleDbType.NVarchar2, 512).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_error_code", OracleDbType.Int32, 512).Direction = ParameterDirection.Output;
                    cmd.Connection = con;

                    OracleDataReader myReader = default(OracleDataReader);
                    myReader = cmd.ExecuteReader();
                    while (myReader.Read())
                    {
                        VipCashDeskModel detail = new VipCashDeskModel();

                        res.Amount = myReader["Amount"] is DBNull ? 0 : Convert.ToDecimal(myReader["Amount"]);
                        res.FeeAmount = myReader["FeeAmount"] is DBNull ? 0 : Convert.ToDecimal(myReader["FeeAmount"]);
                        res.ToTalAmount = myReader["ToTalAmount"] is DBNull ? 0 : Convert.ToDecimal(myReader["ToTalAmount"]);
                        res.AccountNumber = myReader["Cashhescaccount1"].ToString();
                        res.TrAccount = myReader["TransitAccount1"].ToString();

                    }
                    myReader.NextResult();
                    while (myReader.Read())
                    {
                        VipCashDeskModel.VipTransaction detail = new VipCashDeskModel.VipTransaction();
                        detail.CashInID = Convert.ToInt32(myReader["CashInID"]);
                        detail.OperationID = Convert.ToInt32(myReader["OperationID"]);
                        detail.Name = myReader["Name"].ToString();
                        detail.Surname = myReader["Surname"].ToString();
                        detail.OperationDate = Convert.ToDateTime(myReader["OperationDate"]);
                        detail.Amount = myReader["Amount"] is DBNull ? 0 : Convert.ToDecimal(myReader["Amount"]);
                        detail.CAmount = $"{detail.Amount:C}".Replace("$", "").Replace("¤", "");
                        detail.FeeAmount = myReader["FeeAmount"] is DBNull ? 0 : Convert.ToDecimal(myReader["FeeAmount"]);
                        detail.CFeeAmount = $"{detail.FeeAmount:C}".Replace("$", "").Replace("¤", "");
                        detail.ToTalAmount = myReader["ToTalAmount"] is DBNull ? 0 : Convert.ToDecimal(myReader["ToTalAmount"]);
                        detail.CToTalAmount = $"{detail.ToTalAmount:C}".Replace("$", "").Replace("¤", "");
                        detail.AccountNumber = myReader["CashDeskAccount"].ToString();
                        detail.TrAccount = myReader["ToAccountNumber"].ToString();
                        datalist.Add(detail);
                    }
                    res.vipTrans = datalist;


                    myReader.Close();

                }
                catch (Exception e)
                {


                }
                finally
                {

                    con.Close();
                }
            }
            return res;

        }
    }
}
