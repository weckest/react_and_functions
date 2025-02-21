using System.Net;
using Functions2025.Models.School;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SchoolLibrary;

namespace Snoopy.Function;

public class HttpWebAPI
{
    private readonly ILogger<HttpWebAPI> _logger;
    private readonly SchoolContext _context;

    public HttpWebAPI(ILogger<HttpWebAPI> logger, SchoolContext context)
    {
        _logger = logger;
        _context = context;
    }

    [Function("HttpWebAPI")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }

    [Function("GetStudents")]
    public HttpResponseData GetStudents(
[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "students")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP GET/posts trigger function processed a request in GetStudents().");

        var students = _context.Students.ToArray();

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");

        response.WriteStringAsync(JsonConvert.SerializeObject(students));

        return response;
    }

    [Function("GetStudentById")]
    public HttpResponseData GetStudentById
    (
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "students/{id}")] HttpRequestData req,
        int id
    )
    {
        _logger.LogInformation("C# HTTP GET/posts trigger function processed a request.");
        var student = _context.Students.FindAsync(id).Result;
        if (student == null)
        {
            var response = req.CreateResponse(HttpStatusCode.NotFound);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteStringAsync("Not Found");
            return response;
        }
        var response2 = req.CreateResponse(HttpStatusCode.OK);
        response2.Headers.Add("Content-Type", "application/json");
        response2.WriteStringAsync(JsonConvert.SerializeObject(student));
        return response2;
    }

    [Function("GetStudentsBySchool")]
    public HttpResponseData GetStudentsBySchool
    (
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "school/{school}")] HttpRequestData req,
        string school
    )
    {
        _logger.LogInformation("C# HTTP GET/posts trigger function processed a request.");
        var students = _context.Students.Where(s => s.School == school).ToArray();
        if (students.Length == 0)
        {
            var response = req.CreateResponse(HttpStatusCode.NotFound);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteStringAsync("Not Found");
            return response;
        }
        var response2 = req.CreateResponse(HttpStatusCode.OK);
        response2.Headers.Add("Content-Type", "application/json");
        response2.WriteStringAsync(JsonConvert.SerializeObject(students));
        return response2;
    }

    [Function("GetCountBySchool")]
    public HttpResponseData GetCountBySchool
    (
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "students/count-by-school")] HttpRequestData req
    )
    {
        _logger.LogInformation("C# HTTP GET/posts trigger function processed a request.");
        var students = _context.Students.GroupBy(s => s.School).Select(g => new { School = g.Key, Count = g.Count() }).ToArray();
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");
        response.WriteStringAsync(JsonConvert.SerializeObject(students));
        return response;
    }

    [Function("CreateStudent")]
    public HttpResponseData CreateStudent
    (
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "students")] HttpRequestData req
    )
    {
        _logger.LogInformation("C# HTTP POST/posts trigger function processed a request.");
        var student = JsonConvert.DeserializeObject<Student>(req.ReadAsStringAsync().Result);
        _context.Students.Add(student);
        _context.SaveChanges();
        var response = req.CreateResponse(HttpStatusCode.Created);
        response.Headers.Add("Content-Type", "application/json");
        response.WriteStringAsync(JsonConvert.SerializeObject(student));
        return response;
    }

    [Function("UpdateStudent")]
    public HttpResponseData UpdateStudent
    (
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "students/{id}")] HttpRequestData req,
        int id
    )
    {
        _logger.LogInformation("C# HTTP PUT/posts trigger function processed a request.");
        var student = _context.Students.FindAsync(id).Result;
        if (student == null)
        {
            var response = req.CreateResponse(HttpStatusCode.NotFound);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteStringAsync("Not Found");
            return response;
        }
        var student2 = JsonConvert.DeserializeObject<Student>(req.ReadAsStringAsync().Result);
        student.FirstName = student2.FirstName;
        student.LastName = student2.LastName;
        student.School = student2.School;
        _context.SaveChanges();
        var response2 = req.CreateResponse(HttpStatusCode.OK);
        response2.Headers.Add("Content-Type", "application/json");
        response2.WriteStringAsync(JsonConvert.SerializeObject(student));
        return response2;
    }

    [Function("DeleteStudent")]
    public HttpResponseData DeleteStudent
    (
      [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "students/{id}")] HttpRequestData req,
      int id
    )
    {
        _logger.LogInformation("C# HTTP DELETE/posts trigger function processed a request.");
        var student = _context.Students.FindAsync(id).Result;
        if (student == null)
        {
            var response = req.CreateResponse(HttpStatusCode.NotFound);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteStringAsync("Not Found");
            return response;
        }
        _context.Students.Remove(student);
        _context.SaveChanges();
        var response2 = req.CreateResponse(HttpStatusCode.OK);
        response2.Headers.Add("Content-Type", "application/json");
        response2.WriteStringAsync(JsonConvert.SerializeObject(student));
        return response2;
    }

}

