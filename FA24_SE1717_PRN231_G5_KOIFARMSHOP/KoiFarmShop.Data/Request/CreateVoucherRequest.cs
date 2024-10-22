using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Data.Request
{
    public class CreateVoucherRequest
    {
        public string VoucherId { get; set; }

        public string VoucherCode { get; set; }

        public string VoucherName { get; set; }

        public int? ApplyMethod { get; set; } // 1 online 2 offline 3 both

        [Range(0, 100)]
        public double DiscountAmount { get; set; }

        [Range(0, double.MaxValue)]
        public double MinOrderAmount { get; set; }
        
        [Range(0, int.MaxValue)]
        public int? Quantity { get; set; }

        public int Status { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? ValidityStartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [EndDateGreaterThanStartDate("ValidityStartDate", ErrorMessage = "Validity end date must be greater than start date.")]
        public DateTime? ValidityEndDate { get; set; }


        public string? Note { get; set; }
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
