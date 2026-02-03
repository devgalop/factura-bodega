using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Features.Users.Employees.EditEmployee;
using FluentValidation.TestHelper;

namespace devgalop.facturabodega.test.Features.Users.Employees.EditEmployee
{
    public class EditEmployeeRequestValidatorTests
    {
        private readonly EditEmployeeValidator _validator;

        public EditEmployeeRequestValidatorTests()
        {
            _validator = new EditEmployeeValidator();
        }

        [Fact]
        public void Should_HaveError_When_NameIsEmpty()
        {
            var model = new EditEmployeeRequest("test@test.com", "", "FULL_TIME", DateTime.UtcNow, "Admin", true);
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name); 
        }

        [Fact]
        public void Should_HaveError_When_NameExceedsMaxLength()
        {
            var invalidName = new string('a', 101);
            var model = new EditEmployeeRequest("test@test.com", invalidName, "FULL_TIME", DateTime.UtcNow, "Admin", true);
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }  
    }
}