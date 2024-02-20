
using System;
using System.Net;
using System.Net.Http;
using System.Text;

using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class EmployeeControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        // TODO: Add tests to validate > double nested DirectReports loading properly for EmployeeService.GetById();

        [TestMethod]
        public void CreateEmployee_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newEmployee = response.DeserializeContent<Employee>();
            Assert.IsNotNull(newEmployee.EmployeeId);
            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.Department, newEmployee.Department);
            Assert.AreEqual(employee.Position, newEmployee.Position);
        }

        [TestMethod]
        public void GetEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";
            var expectedPosition = "Development Manager";
            var expectedDepartment = "Engineering";
            var expectedDirectReportsCount = 2;
            

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);

            // Adding additional fields to validate because they were missing and I was running into
            // issues with directReports being null for the employee being tested here - D.F. 2/19/2024
            Assert.AreEqual(expectedPosition, employee.Position);
            Assert.AreEqual(expectedDepartment, employee.Department);

            // Adding validation for DirectReports per the issue mentioned above - D.F. 2/19/2024
            Assert.IsNotNull(employee.DirectReports);
            Assert.AreEqual(expectedDirectReportsCount, employee.DirectReports.Count);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_Ok()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            var newEmployee = putResponse.DeserializeContent<Employee>();

            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Music",
                FirstName = "Sunny",
                LastName = "Bono",
                Position = "Singer/Song Writer",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void GetReportingStructureById_Returns_Ok()
        {
            // Arrange
            var expectedEmployee = new Employee()
            {
                EmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f",
                FirstName = "John",
                LastName = "Lennon",
                Department = "Engineering",
                Position = "Development Manager",
            };
            int expectedDirectReports = 2;
            int expectedReports = 4;

            // Execute

            var getRequestTask = _httpClient.GetAsync($"api/employee/{expectedEmployee.EmployeeId}/reporting-structure");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            // Validate base employee information if correct / accurate
            var reportingStructure = response.DeserializeContent<ReportingStructure>();
            Assert.AreEqual(expectedEmployee.FirstName, reportingStructure.Employee.FirstName);
            Assert.AreEqual(expectedEmployee.LastName, reportingStructure.Employee.LastName);
            Assert.AreEqual(expectedEmployee.Department, reportingStructure.Employee.Department);
            Assert.AreEqual(expectedEmployee.Position, reportingStructure.Employee.Position);
            Assert.IsNotNull(reportingStructure.Employee.DirectReports);
            Assert.AreEqual(expectedDirectReports, reportingStructure.Employee.DirectReports.Count);

            // Validate ReportingStructure information is correct / accurate
            Assert.AreEqual(expectedReports, reportingStructure.NumberOfReports);


        }
        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {
            // Arrange
            var compensation = new Compensation()
            {
                EmployeeId = "1234",
                Employee = new Employee()
                {
                    EmployeeId = "1234",
                    Department = "Complaints",
                    FirstName = "Debbie",
                    LastName = "Downer",
                    Position = "Receiver",
                },
                Salary = new decimal(70000.00),
                EffectiveDate = new DateTime(2024, 2, 20, 10, 30, 0),
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync($"api/employee/{compensation.EmployeeId}/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newCompensation.EmployeeId);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
            Assert.AreEqual(compensation.Employee.FirstName, newCompensation.Employee.FirstName);
            Assert.AreEqual(compensation.Employee.LastName, newCompensation.Employee.LastName);
            Assert.AreEqual(compensation.Employee.Position, newCompensation.Employee.Position);
            Assert.AreEqual(compensation.Employee.Department, newCompensation.Employee.Department);
            Assert.AreEqual(compensation.Employee.EmployeeId, newCompensation.Employee.EmployeeId);
        }
        [TestMethod]
        public void GetCompensationById_Returns_Ok()
        {
            // Arrange
            var expectedCompensation = new Compensation()
            {
                EmployeeId = "1234",
                Employee = new Employee()
                {
                    EmployeeId = "1234",
                    Department = "Complaints",
                    FirstName = "Debbie",
                    LastName = "Downer",
                    Position = "Receiver",
                },
                Salary = new decimal(70000.00),
                EffectiveDate = new DateTime(2024, 2, 20, 10, 30, 0),
            };

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{expectedCompensation.EmployeeId}/compensation");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var actualCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(actualCompensation.EmployeeId);
            Assert.AreEqual(expectedCompensation.Salary, actualCompensation.Salary);
            Assert.AreEqual(expectedCompensation.EffectiveDate, actualCompensation.EffectiveDate);
            Assert.AreEqual(expectedCompensation.Employee.FirstName, actualCompensation.Employee.FirstName);
            Assert.AreEqual(expectedCompensation.Employee.LastName, actualCompensation.Employee.LastName);
            Assert.AreEqual(expectedCompensation.Employee.Position, actualCompensation.Employee.Position);
            Assert.AreEqual(expectedCompensation.Employee.Department, actualCompensation.Employee.Department);
            Assert.AreEqual(expectedCompensation.Employee.EmployeeId, actualCompensation.Employee.EmployeeId);
        }
    }
}
