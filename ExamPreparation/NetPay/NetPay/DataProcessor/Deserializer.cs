using NetPay.Data;
using System.ComponentModel.DataAnnotations;
using NetPay.Utilities;
using System.Text;
using NetPay.DataProcessor.ImportDtos;
using NetPay.Data.Models;
using Newtonsoft.Json;
using System.Globalization;
using NetPay.Data.Models.Enums;

namespace NetPay.DataProcessor
{
    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data format!";
        private const string DuplicationDataMessage = "Error! Data duplicated.";
        private const string SuccessfullyImportedHousehold = "Successfully imported household. Contact person: {0}";
        private const string SuccessfullyImportedExpense = "Successfully imported expense. {0}, Amount: {1}";

        public static string ImportHouseholds(NetPayContext context, string xmlString)
        {
            StringBuilder sb=new StringBuilder();
            ImportHouseholdDto[]? houseDtos = XmlHelper
                .Deserialize<ImportHouseholdDto[]>(xmlString, "Households");

            if(houseDtos!=null && houseDtos.Length>0)
            {
                ICollection<Household> households = new List<Household>();
                foreach (var houseDto in houseDtos)
                {
                    if (!IsValid(houseDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool isAlreadyImported = context.Households
                        .Any(h => h.ContactPerson == houseDto.ContactPerson ||
                        h.Email == houseDto.Email ||
                        h.PhoneNumber == houseDto.PhoneNumber);

                    bool isToBeImported = households
                        .Any(h => h.ContactPerson == houseDto.ContactPerson ||
                        h.Email == houseDto.Email ||
                        h.PhoneNumber == houseDto.PhoneNumber);

                    if (isAlreadyImported || isToBeImported)
                    {
                        sb.AppendLine(DuplicationDataMessage);
                        continue;
                    }

                    Household household = new Household()
                    {
                        ContactPerson = houseDto.ContactPerson,
                        Email = houseDto.Email,
                        PhoneNumber = houseDto.PhoneNumber
                    };
                    households.Add(household);

                    string successMessage=string.Format(SuccessfullyImportedHousehold, houseDto.ContactPerson);
                    sb.AppendLine(successMessage);
                }
                context.Households.AddRange(households);
                context.SaveChanges();
            }
            return sb.ToString().TrimEnd();
        }

        public static string ImportExpenses(NetPayContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            ImportExpenseDto[]? expenseDtos = JsonConvert
                .DeserializeObject<ImportExpenseDto[]>(jsonString);

            if (expenseDtos != null && expenseDtos.Length > 0)
            {
                ICollection<Expense> expenses = new List<Expense>();

                foreach (var expenseDto in expenseDtos)
                {
                    if(!IsValid(expenseDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool isHouseholdValid=context.Households
                        .Any(h=>h.Id==expenseDto.HouseholdId);

                    bool isServiceValid=context.Services
                        .Any(s=>s.Id==expenseDto.ServiceId);

                    if((!isHouseholdValid) || (!isServiceValid))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool isDueDateValid=DateTime.TryParseExact(expenseDto.DueDate, "yyyy-MM-dd",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dueDate);

                    bool isPaymentStatusValid=PaymentStatus
                        .TryParse(expenseDto.PaymentStatus, out PaymentStatus paymentStatus);

                    if((!isDueDateValid) || (!isPaymentStatusValid))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Expense expense = new Expense()
                    {
                        ExpenseName = expenseDto.ExpenseName,
                        Amount = expenseDto.Amount,
                        DueDate = dueDate,
                        PaymentStatus = paymentStatus,
                        HouseholdId = expenseDto.HouseholdId,
                        ServiceId = expenseDto.ServiceId
                    };
                    expenses.Add(expense);

                    string successMessage = string
                        .Format(SuccessfullyImportedExpense, expense.ExpenseName, expense.Amount.ToString("f2"));
                    sb.AppendLine(successMessage);
                }
                context.Expenses.AddRange(expenses);
                context.SaveChanges();
            }
            return sb.ToString().TrimEnd();
        }

        public static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            foreach(var result in validationResults)
            {
                string currvValidationMessage = result.ErrorMessage;
            }

            return isValid;
        }
    }
}
