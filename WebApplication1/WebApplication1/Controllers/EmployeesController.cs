using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StoredProc.Data;
using StoredProc.Models;
using System.Data;
using System.Text;


namespace StoredProc.Controllers
{
    public class EmployeesController : Controller
    {
        public StoredProcDbContext _context;
        public IConfiguration _config { get; }

        public EmployeesController
            (
            StoredProcDbContext context,
            IConfiguration config
            )
        {
            _context = context;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IEnumerable<Employee> SearchResult()
        {
            var result = _context.Employees
                .FromSqlRaw<Employee>("spSearchEmployees")
                .ToList();

            return result;
        }

        [HttpGet]
        public IActionResult DynamicSql()
        {
            string connectionStr = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "dbo.spSearchEmployees";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                List<Employee> model = new List<Employee>();
                while (sdr.Read())
                {
                    var details = new Employee();
                    details.FirstName = sdr["FirstName"].ToString();
                    details.LastName = sdr["LastName"].ToString();
                    details.Gender = sdr["Gender"].ToString();
                    details.Salary = Convert.ToInt32(sdr["Salary"]);
                    model.Add(details);
                }
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult DynamicSql(string firstName, string lastname, string gender, int salary)
        {
            string conntectionStr = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(conntectionStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                StringBuilder sbCommand = new StringBuilder("Select * from Employees where 1 = 1");

                if (firstName != null)
                {
                    sbCommand.Append(" AND FirstName=@FirstName");
                    SqlParameter param = new
                    SqlParameter("@FirstName", firstName);
                    cmd.Parameters.Add(param);
                }
                if (lastname != null)
                {
                    sbCommand.Append(" AND LastName=@LastName");
                    SqlParameter param = new
                    SqlParameter("@LastName", lastname);
                    cmd.Parameters.Add(param);
                }
                if (gender != null)
                {
                    sbCommand.Append(" AND Gender=@Gender");
                    SqlParameter param = new
                    SqlParameter("@Gender", gender);
                    cmd.Parameters.Add(param);
                }
                if (salary != 0)
                {
                    sbCommand.Append(" AND Salary=@Salary");
                    SqlParameter param = new
                    SqlParameter("@Salary", salary);
                    cmd.Parameters.Add(param);
                }
                cmd.CommandText = sbCommand.ToString();
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                List<Employee> model = new List<Employee>();
                while (sdr.Read())
                {
                    var details = new Employee();
                    details.FirstName = sdr["FirstName"].ToString();
                    details.LastName = sdr["LastName"].ToString();
                    details.Gender = sdr["Gender"].ToString();
                    details.Salary = Convert.ToInt32(sdr["Salary"]);
                    model.Add(details);
                }

                return View(model);
            }
        }
    }
}
