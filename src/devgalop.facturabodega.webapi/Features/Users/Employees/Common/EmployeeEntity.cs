using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.Common
{
    /// <summary>
    /// Estados disponibles para un empleado.
    /// </summary>
    public enum EmployeeStatus
    {
        INACTIVE = 0,
        ACTIVE = 1
    }

    /// <summary>
    /// Tipos de contratos disponibles para un empleado.
    /// </summary>
    public enum EmployeeContractType
    {
        FULL_TIME = 0,
        PART_TIME = 1,
        CONTRACTOR = 2
    }

    /// <summary>
    /// Entidad para representar a un empleado.
    /// </summary>
    public class EmployeeEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Document { get; set; }
        public string Email { get; private set; }
        public string PasswordHashed { get; set; }
        public DateTime HiringDate { get; set; }
        public EmployeeContractType ContractType { get; set; }
        public EmployeeStatus Status { get; set; }

        
        public Guid RoleId { get; set; }
        public RoleEntity Role { get; set; }

        public ICollection<EmployeeRefreshTokenEntity> RefreshTokens { get; set; }

        private EmployeeEntity()
        {
            Name = string.Empty;
            Email = string.Empty;
            PasswordHashed = string.Empty;
            Document = string.Empty;
            Role = null!;
            RefreshTokens = new List<EmployeeRefreshTokenEntity>();
        }

        public EmployeeEntity(string name, 
                              string email,
                              string document,
                              string passwordHashed, 
                              DateTime hiringDate, 
                              EmployeeContractType contractType,
                              RoleEntity role)
        {
            Id = Guid.CreateVersion7();
            Name = name;
            Email = email;
            Document = document;
            PasswordHashed = passwordHashed;
            HiringDate = hiringDate.ToUniversalTime();
            ContractType = contractType;
            Status = EmployeeStatus.ACTIVE;
            RoleId = role.Id;
            Role = role;
            RefreshTokens = new List<EmployeeRefreshTokenEntity>();
        }

        /// <summary>
        /// Cambia el estado del empleado a INACTIVE.
        /// </summary>
        public void Deactivate()
        {
            Status = EmployeeStatus.INACTIVE;
        }
    }
}