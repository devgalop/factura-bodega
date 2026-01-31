using System;
using FluentValidation.TestHelper;
using Xunit;
using devgalop.facturabodega.webapi.Features.Users.Employees.AddEmployee;

namespace devgalop.facturabodega.test.Features.Users.Employees.AddEmployee
{
    public class AddEmployeeRequestValidatorTests
    {
        private readonly AddEmployeeRequestValidator _validator;

        public AddEmployeeRequestValidatorTests()
        {
            _validator = new AddEmployeeRequestValidator();
        }

        [Fact]
        public void Should_HaveError_When_NameIsEmpty()
        {
            var model = new AddEmployeeRequest("", "test@example.com", "Password1!", DateTime.UtcNow, "FULL_TIME");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_HaveError_When_NameExceedsMaxLength()
        {
            var longName = new string('a', 101);
            var model = new AddEmployeeRequest(longName, "test@example.com", "Password1!", DateTime.UtcNow, "FULL_TIME");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_HaveError_When_EmailIsEmpty()
        {
            var model = new AddEmployeeRequest("John Doe", "", "Password1!", DateTime.UtcNow, "FULL_TIME");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_HaveError_When_EmailIsInvalid()
        {
            var model = new AddEmployeeRequest("John Doe", "invalid-email", "Password1!", DateTime.UtcNow, "FULL_TIME");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_HaveError_When_EmailExceedsMaxLength()
        {
            var longEmail = new string('a', 101) + "@example.com";
            var model = new AddEmployeeRequest("John Doe", longEmail, "Password1!", DateTime.UtcNow, "FULL_TIME");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_HaveError_When_HiringDateIsInFuture()
        {
            var futureDate = DateTime.UtcNow.AddDays(1);
            var model = new AddEmployeeRequest("John Doe", "test@example.com", "Password1!", futureDate, "FULL_TIME");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.HiringDate);
        }

        [Fact]
        public void Should_HaveError_When_ContractTypeIsEmpty()
        {
            var model = new AddEmployeeRequest("John Doe", "test@example.com", "Password1!", DateTime.UtcNow, "");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ContractType);
        }

        [Fact]
        public void Should_HaveError_When_PasswordIsEmpty()
        {
            var model = new AddEmployeeRequest("John Doe", "test@example.com", "", DateTime.UtcNow, "FULL_TIME");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_HaveError_When_PasswordIsTooShort()
        {
            var model = new AddEmployeeRequest("John Doe", "test@example.com", "Short1!", DateTime.UtcNow, "FULL_TIME");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_HaveError_When_PasswordIsTooLong()
        {
            var longPassword = new string('a', 17) + "1!";
            var model = new AddEmployeeRequest("John Doe", "test@example.com", longPassword, DateTime.UtcNow, "FULL_TIME");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_HaveError_When_PasswordLacksUppercase()
        {
            var model = new AddEmployeeRequest("John Doe", "test@example.com", "password1!", DateTime.UtcNow, "FULL_TIME");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_HaveError_When_PasswordLacksLowercase()
        {
            var model = new AddEmployeeRequest("John Doe", "test@example.com", "PASSWORD1!", DateTime.UtcNow, "FULL_TIME");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_HaveError_When_PasswordLacksNumber()
        {
            var model = new AddEmployeeRequest("John Doe", "test@example.com", "Password!", DateTime.UtcNow, "FULL_TIME");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_HaveError_When_PasswordLacksSymbol()
        {
            var model = new AddEmployeeRequest("John Doe", "test@example.com", "Password1", DateTime.UtcNow, "FULL_TIME");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_NotHaveError_When_RequestIsValid()
        {
            var model = new AddEmployeeRequest("John Doe", "test@example.com", "Password1!", DateTime.UtcNow, "FULL_TIME");
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}