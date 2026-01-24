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
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string PasswordHashed { get; set; }
        public DateTime HiringDate { get; private set; }
        public EmployeeContractType ContractType { get; private set; }
        public EmployeeStatus Status { get; set; }

        
        public Guid RoleId { get; set; }
        public RoleEntity Role { get; set; }

        private EmployeeEntity()
        {
            Name = string.Empty;
            Email = string.Empty;
            PasswordHashed = string.Empty;
            Role = null!;
        }

        public EmployeeEntity(string name, 
                              string email,
                              string passwordHashed, 
                              DateTime hiringDate, 
                              EmployeeContractType contractType,
                              Guid roleId)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            PasswordHashed = passwordHashed;
            HiringDate = hiringDate.ToUniversalTime();
            ContractType = contractType;
            Status = EmployeeStatus.ACTIVE;
            RoleId = roleId;
            Role = null!;
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