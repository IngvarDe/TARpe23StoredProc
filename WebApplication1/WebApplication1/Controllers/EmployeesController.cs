using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StoredProc.Data;
using StoredProc.Models;


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
        public IActionResult DynamicSQL(string firstName, string lastName, string gender, int salary)
        {
            string connectionStr = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "dbo.spSearchEmployees";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                if(firstName != null)
                {
                    SqlParameter param_fn = new SqlParameter("@FirstName", firstName);
                    cmd.Parameters.Add(param_fn);
                }

                if (lastName != null)
                {
                    SqlParameter param_ln = new SqlParameter("@LastName", lastName);
                    cmd.Parameters.Add(param_ln);
                }

                if (gender != null)
                {
                    SqlParameter param_g = new SqlParameter("@Gender", gender);
                    cmd.Parameters.Add(param_g);
                }

                if (salary != null)
                {
                    SqlParameter param_s = new SqlParameter("@Salary", salary);
                    cmd.Parameters.Add(param_s);
                }
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

            return View();
        }
    }
}
