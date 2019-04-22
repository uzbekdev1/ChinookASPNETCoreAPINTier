using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chinook.Domain.Extensions;
using Chinook.Domain.ApiModels;
using Chinook.Domain.Converters;
using Chinook.Domain.Entities;

namespace Chinook.Domain.Supervisor
{
    public partial class ChinookSupervisor
    {
        public async Task<IEnumerable<EmployeeApiModel>> GetAllEmployeeAsync(
            CancellationToken ct = default)
        {
            var employees = await _employeeRepository.GetAllAsync(ct);
            return employees.ConvertAll();
        }

        public async Task<EmployeeApiModel> GetEmployeeByIdAsync(int id,
            CancellationToken ct = default)
        {
            var employeeApiModel = (await _employeeRepository.GetByIdAsync(id, ct)).Convert;
            employeeApiModel.Customers = (await GetCustomerBySupportRepIdAsync(employeeApiModel.EmployeeId, ct)).ToList();
            employeeApiModel.DirectReports = (await GetEmployeeDirectReportsAsync(employeeApiModel.EmployeeId, ct)).ToList();
            employeeApiModel.Manager = employeeApiModel.ReportsTo.HasValue
                ? await GetEmployeeReportsToAsync(employeeApiModel.ReportsTo.GetValueOrDefault(), ct)
                : null;
            employeeApiModel.ReportsToName = employeeApiModel.ReportsTo.HasValue
                ? $"{employeeApiModel.Manager.LastName}, {employeeApiModel.Manager.FirstName}"
                : string.Empty;
            return employeeApiModel;
        }

        public async Task<EmployeeApiModel> GetEmployeeReportsToAsync(int id,
            CancellationToken ct = default)
        {
            var employee = await _employeeRepository.GetReportsToAsync(id, ct);
            return employee.Convert;
        }

        public async Task<EmployeeApiModel> AddEmployeeAsync(EmployeeApiModel newEmployeeApiModel,
            CancellationToken ct = default)
        {
            var employee = newEmployeeApiModel.Convert;

            employee = await _employeeRepository.AddAsync(employee, ct);
            newEmployeeApiModel.EmployeeId = employee.EmployeeId;
            return newEmployeeApiModel;
        }

        public async Task<bool> UpdateEmployeeAsync(EmployeeApiModel employeeApiModel,
            CancellationToken ct = default)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeApiModel.EmployeeId, ct);

            if (employee == null) return false;
            employee.EmployeeId = employeeApiModel.EmployeeId;
            employee.LastName = employeeApiModel.LastName;
            employee.FirstName = employeeApiModel.FirstName;
            employee.Title = employeeApiModel.Title;
            employee.ReportsTo = employeeApiModel.ReportsTo;
            employee.BirthDate = employeeApiModel.BirthDate;
            employee.HireDate = employeeApiModel.HireDate;
            employee.Address = employeeApiModel.Address;
            employee.City = employeeApiModel.City;
            employee.State = employeeApiModel.State;
            employee.Country = employeeApiModel.Country;
            employee.PostalCode = employeeApiModel.PostalCode;
            employee.Phone = employeeApiModel.Phone;
            employee.Fax = employeeApiModel.Fax;
            employee.Email = employeeApiModel.Email;

            return await _employeeRepository.UpdateAsync(employee, ct);
        }

        public async Task<bool> DeleteEmployeeAsync(int id, CancellationToken ct = default) 
            => await _employeeRepository.DeleteAsync(id, ct);

        public async Task<IEnumerable<EmployeeApiModel>> GetEmployeeDirectReportsAsync(int id,
            CancellationToken ct = default)
        {
            var employees = await _employeeRepository.GetDirectReportsAsync(id, ct);
            return employees.ConvertAll();
        }

        public async Task<IEnumerable<EmployeeApiModel>> GetDirectReportsAsync(int id,
            CancellationToken ct = default)
        {
            var employees = await _employeeRepository.GetDirectReportsAsync(id, ct);
            return employees.ConvertAll();
        }
    }
}