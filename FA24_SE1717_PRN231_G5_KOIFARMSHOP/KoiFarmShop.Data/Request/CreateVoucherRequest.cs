using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Data.Request
{
    public class CreateVoucherRequest
    {
        public string VoucherId { get; set; }

        public string VoucherCode { get; set; }
        [Range(0.1, 1)]
        public double DiscountAmount { get; set; }
        [Range(0, 100)]
        public double MinOrderAmount { get; set; }

        public int Status { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ValidityStartDate { get; set; }

        [DataType(DataType.Date)]
        [EndDateGreaterThanStartDate("ValidityStartDate", ErrorMessage = "Validity end date must be greater than start date.")]
        public DateTime? ValidityEndDate { get; set; }
    }

    public class EndDateGreaterThanStartDate : ValidationAttribute
    {
        private readonly string _startDatePropertyName;

        public EndDateGreaterThanStartDate(string startDatePropertyName)
        {
            _startDatePropertyName = startDatePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var startDateProperty = validationContext.ObjectType.GetProperty(_startDatePropertyName);
            if (startDateProperty == null)
            {
                return new ValidationResult($"Unknown property: {_startDatePropertyName}");
            }

            var startDateValue = (DateTime?)startDateProperty.GetValue(validationContext.ObjectInstance);
            var endDateValue = (DateTime?)value;

            if (startDateValue.HasValue && endDateValue.HasValue && endDateValue <= startDateValue)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
