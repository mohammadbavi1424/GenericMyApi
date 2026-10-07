using Data.DTOs.Base;
using Entities.Entities.User;
using System.ComponentModel.DataAnnotations;

namespace Data.DTOs.Users
{
    public class UserDto : BaseDto<UserDto,User>, IValidatableObject
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string NationalCode { get; set; }

        public Gender Gender { get; set; }

        public string? SecurityStamp { get; set; }

        public int Age { get; set; } = 0;


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UserName.Equals("test", StringComparison.OrdinalIgnoreCase))
                yield return new ValidationResult("نام کاربری نمیتواند Test باشد", new[] { nameof(UserName) });
            if (Password.Equals("123456"))
                yield return new ValidationResult("رمز عبور نمیتواند 123456 باشد", new[] { nameof(Password) });
            if (Gender == Gender.Male && Age > 30)
                yield return new ValidationResult("آقایان بیشتر از 30 سال معتبر نیستند", new[] { nameof(Gender), nameof(Age) });
            if (Age < 18)
                yield return new ValidationResult("سن نمیتواند زیر 18 سال باشد", new[] { nameof(Age) });

        }
    }
}
