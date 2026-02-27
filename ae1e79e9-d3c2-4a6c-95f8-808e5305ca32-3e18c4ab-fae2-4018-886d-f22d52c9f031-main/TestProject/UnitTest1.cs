using dotnetapp.Exceptions;
using dotnetapp.Models;
using dotnetapp.Data;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Linq;
using System.Reflection;
using dotnetapp.Services;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;

namespace dotnetapp.Tests
{
    [TestFixture]
    public class Tests
    {

        private ApplicationDbContext _context; 
        private HttpClient _httpClient;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "TestDatabase").Options;
            _context = new ApplicationDbContext(options);
           
             _httpClient = new HttpClient();
             _httpClient.BaseAddress = new Uri("http://localhost:8080");

        }

        [TearDown]
        public void TearDown()
        {
             _context.Dispose();
        }

   [Test, Order(1)]
    public async Task Backend_Test_Post_Method_Register_Admin_Returns_HttpStatusCode_OK()
    {
        ClearDatabase();
        string uniqueId = Guid.NewGuid().ToString();

        // Generate a unique userName based on a timestamp
        string uniqueUsername = $"abcd_{uniqueId}";
        string uniqueEmail = $"abcd{uniqueId}@gmail.com";

        string requestBody = $"{{\"Username\": \"{uniqueUsername}\", \"Password\": \"abc@123A\", \"Email\": \"{uniqueEmail}\", \"MobileNumber\": \"1234567890\", \"UserRole\": \"Admin\"}}";
        HttpResponseMessage response = await _httpClient.PostAsync("/api/register", new StringContent(requestBody, Encoding.UTF8, "application/json"));

        Console.WriteLine(response.StatusCode);
        string responseString = await response.Content.ReadAsStringAsync();

        Console.WriteLine(responseString);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
  
   [Test, Order(2)]
    public async Task Backend_Test_Post_Method_Login_Admin_Returns_HttpStatusCode_OK()
    {
        ClearDatabase();

        string uniqueId = Guid.NewGuid().ToString();

        // Generate a unique userName based on a timestamp
        string uniqueUsername = $"abcd_{uniqueId}";
        string uniqueEmail = $"abcd{uniqueId}@gmail.com";

        string requestBody = $"{{\"Username\": \"{uniqueUsername}\", \"Password\": \"abc@123A\", \"Email\": \"{uniqueEmail}\", \"MobileNumber\": \"1234567890\", \"UserRole\": \"Admin\"}}";
        HttpResponseMessage response = await _httpClient.PostAsync("/api/register", new StringContent(requestBody, Encoding.UTF8, "application/json"));

        // Print registration response
        string registerResponseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Registration Response: " + registerResponseBody);

        // Login with the registered user
        string loginRequestBody = $"{{\"Email\" : \"{uniqueEmail}\",\"Password\" : \"abc@123A\"}}"; // Updated variable names
        HttpResponseMessage loginResponse = await _httpClient.PostAsync("/api/login", new StringContent(loginRequestBody, Encoding.UTF8, "application/json"));

        // Print login response
        string loginResponseBody = await loginResponse.Content.ReadAsStringAsync();
        Console.WriteLine("Login Response: " + loginResponseBody);

        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);
    }


    [Test, Order(3)]
    public async Task Backend_Test_Post_CookingClass_With_Token_By_Admin_Returns_HttpStatusCode_OK()
    {
        ClearDatabase();
        string uniqueId = Guid.NewGuid().ToString();

        // Generate a unique userName based on a timestamp
        string uniqueUsername = $"abcd_{uniqueId}";
        string uniqueEmail = $"abcd{uniqueId}@gmail.com";

        string requestBody = $"{{\"Username\": \"{uniqueUsername}\", \"Password\": \"abc@123A\", \"Email\": \"{uniqueEmail}\", \"MobileNumber\": \"1234567890\", \"UserRole\": \"Admin\"}}";
        HttpResponseMessage response = await _httpClient.PostAsync("/api/register", new StringContent(requestBody, Encoding.UTF8, "application/json"));

        // Print registration response
        string registerResponseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Registration Response: " + registerResponseBody);

        // Login with the registered user
        string loginRequestBody = $"{{\"Email\" : \"{uniqueEmail}\",\"Password\" : \"abc@123A\"}}";
        HttpResponseMessage loginResponse = await _httpClient.PostAsync("/api/login", new StringContent(loginRequestBody, Encoding.UTF8, "application/json"));

        // Print login response
        string loginResponseBody = await loginResponse.Content.ReadAsStringAsync();
        Console.WriteLine("Login Response: " + loginResponseBody);

        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);
        string responseBody = await loginResponse.Content.ReadAsStringAsync();

        dynamic responseMap = JsonConvert.DeserializeObject(responseBody);
        string token = responseMap.token;

        Assert.IsNotNull(token);

        string uniqueClassName = $"cooking_{Guid.NewGuid().ToString()}";

        // Create a unique CookingClass JSON payload
        string cookingClassJson = $"{{\"ClassName\":\"{uniqueClassName}\",\"CuisineType\":\"Italian\",\"ChefName\":\"Gordon Ramsay\",\"Location\":\"Kitchen Studio\",\"DurationInHours\":4,\"Fee\":150.00,\"IngredientsProvided\":\"Tomatoes, Cheese, Pasta\",\"SkillLevel\":\"Beginner\",\"SpecialRequirements\":\"Ability to stand for long hours\"}}";
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        
        HttpResponseMessage cookingResponse = await _httpClient.PostAsync("/api/cookingClass",
            new StringContent(cookingClassJson, Encoding.UTF8, "application/json"));

        Console.WriteLine("Cooking Response: " + cookingResponse);

        Assert.AreEqual(HttpStatusCode.OK, cookingResponse.StatusCode);
    }

    
    [Test, Order(4)]

    public async Task Backend_Test_Post_CookingClass_Without_Token_By_Admin_Returns_HttpStatusCode_Unauthorized()
    {
        ClearDatabase();
        string uniqueId = Guid.NewGuid().ToString();

        // Generate a unique userName based on a timestamp
        string uniqueUsername = $"abcd_{uniqueId}";
        string uniqueEmail = $"abcd{uniqueId}@gmail.com";

        string requestBody = $"{{\"Username\": \"{uniqueUsername}\", \"Password\": \"abc@123A\", \"Email\": \"{uniqueEmail}\", \"MobileNumber\": \"1234567890\", \"UserRole\": \"Admin\"}}";
        HttpResponseMessage response = await _httpClient.PostAsync("/api/register", new StringContent(requestBody, Encoding.UTF8, "application/json"));

        // Print registration response
        string registerResponseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Registration Response: " + registerResponseBody);

        // Login with the registered user
        string loginRequestBody = $"{{\"Email\" : \"{uniqueEmail}\",\"Password\" : \"abc@123A\"}}";
        HttpResponseMessage loginResponse = await _httpClient.PostAsync("/api/login", new StringContent(loginRequestBody, Encoding.UTF8, "application/json"));

        // Print login response
        string loginResponseBody = await loginResponse.Content.ReadAsStringAsync();
        Console.WriteLine("Login Response: " + loginResponseBody);

        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

        string uniqueClassName = $"cooking_{Guid.NewGuid().ToString()}";

        // Create a unique CookingClass JSON payload
        string cookingClassJson = $"{{\"ClassName\":\"{uniqueClassName}\",\"CuisineType\":\"Italian\",\"ChefName\":\"Gordon Ramsay\",\"Location\":\"Kitchen Studio\",\"DurationInHours\":4,\"Fee\":150.00,\"IngredientsProvided\":\"Tomatoes, Cheese, Pasta\",\"SkillLevel\":\"Beginner\",\"SpecialRequirements\":\"Ability to stand for long hours\"}}";

        HttpResponseMessage cookingResponse = await _httpClient.PostAsync("/api/cookingClass",
            new StringContent(cookingClassJson, Encoding.UTF8, "application/json"));

        Console.WriteLine("Cooking Response: " + cookingResponse);

        Assert.AreEqual(HttpStatusCode.Unauthorized, cookingResponse.StatusCode);
    }


    [Test, Order(5)]
    public async Task Backend_Test_Get_Method_Get_CookingClassById_In_CookingClass_Service_Fetches_CookingClass_Successfully()
    {
        ClearDatabase();

        var cookingData = new Dictionary<string, object>
        {
            { "CookingClassId", 20 }, // Unique identifier for the cooking class
            { "ClassName", "Italian Cooking Basics" }, // Name of the cooking class
            { "CuisineType", "Italian" }, // Type of cuisine covered in the class
            { "ChefName", "Chef Mario" }, // Name of the chef conducting the class
            { "Location", "Kitchen Studio" }, // Venue of the class
            { "DurationInHours", 3 }, // Duration of the class in hours
            { "Fee", 100m }, // Cost for attending the class
            { "IngredientsProvided", "Pasta, Olive Oil, Tomatoes, Cheese" }, // Ingredients provided during the class
            { "SkillLevel", "Beginner" }, // Skill level required
            { "SpecialRequirements", "Ability to stand for 3 hours" }
        };

        var cooking = new CookingClass();
        foreach (var kvp in cookingData)
        {
            var propertyInfo = typeof(CookingClass).GetProperty(kvp.Key);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(cooking, kvp.Value);
            }
        }
        _context.CookingClasses.Add(cooking);
        _context.SaveChanges();

        string assemblyName = "dotnetapp";
        Assembly assembly = Assembly.Load(assemblyName);
        string ServiceName = "dotnetapp.Services.CookingClassService";
        string typeName = "dotnetapp.Models.CookingClass";

        Type serviceType = assembly.GetType(ServiceName);
        Type modelType = assembly.GetType(typeName);

        MethodInfo getCookingMethod = serviceType.GetMethod("GetCookingClassById");

        if (getCookingMethod != null)
        {
            var service = Activator.CreateInstance(serviceType, _context);
            var retrievedCooking = (Task<CookingClass>)getCookingMethod.Invoke(service, new object[] { 20 });

            Assert.IsNotNull(retrievedCooking);
            Assert.AreEqual(cooking.ClassName, retrievedCooking.Result.ClassName);
        }
        else
        {
            Assert.Fail();
        }
    }

    [Test, Order(6)]
    public async Task Backend_Test_Put_Method_UpdateCookingClass_In_CookingClass_Service_Updates_CookingClass_Successfully()
    {
        ClearDatabase();

        var cookingData = new Dictionary<string, object>
        {
            { "CookingClassId", 20 }, // Unique identifier for the cooking class
            { "ClassName", "Italian Cooking Basics" }, // Name of the cooking class
            { "CuisineType", "Italian" }, // Type of cuisine covered in the class
            { "ChefName", "Chef Mario" }, // Name of the chef conducting the class
            { "Location", "Kitchen Studio" }, // Venue of the class
            { "DurationInHours", 3 }, // Duration of the class in hours
            { "Fee", 100m }, // Cost for attending the class
            { "IngredientsProvided", "Pasta, Olive Oil, Tomatoes, Cheese" }, // Ingredients provided during the class
            { "SkillLevel", "Beginner" }, // Skill level required
            { "SpecialRequirements", "Ability to stand for 3 hours" }
        };

        var cooking = new CookingClass();
        foreach (var kvp in cookingData)
        {
            var propertyInfo = typeof(CookingClass).GetProperty(kvp.Key);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(cooking, kvp.Value);
            }
        }
        _context.CookingClasses.Add(cooking);
        _context.SaveChanges();

        string assemblyName = "dotnetapp";
        Assembly assembly = Assembly.Load(assemblyName);
        string ServiceName = "dotnetapp.Services.CookingClassService";
        string typeName = "dotnetapp.Models.CookingClass";

        Type serviceType = assembly.GetType(ServiceName);
        Type modelType = assembly.GetType(typeName);

        MethodInfo updateMethod = serviceType.GetMethod("UpdateCookingClass", new[] { typeof(int), modelType });

        if (updateMethod != null)
        {
            var service = Activator.CreateInstance(serviceType, _context);

            var updatedCookingData = new Dictionary<string, object>
            {
                { "CookingClassId", 20 }, // Unique identifier for the cooking class
                { "ClassName", "Advanced Strength Cooking" }, // Name of the cooking class
                { "CuisineType", "Italian" }, // Type of cuisine covered in the class
                { "ChefName", "Chef Mario" }, // Name of the chef conducting the class
                { "Location", "Kitchen Studio" }, // Venue of the class
                { "DurationInHours", 3 }, // Duration of the class in hours
                { "Fee", 100m }, // Cost for attending the class
                { "IngredientsProvided", "Pasta, Olive Oil, Tomatoes, Cheese" }, // Ingredients provided during the class
                { "SkillLevel", "Beginner" }, // Skill level required
                { "SpecialRequirements", "Ability to stand for 3 hours" }
            };

            var updatedCooking = Activator.CreateInstance(modelType);
            foreach (var kvp in updatedCookingData)
            {
                var propertyInfo = modelType.GetProperty(kvp.Key);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(updatedCooking, kvp.Value);
                }
            }

            var updateResult = (Task<bool>)updateMethod.Invoke(service, new object[] { 20, updatedCooking });

            var updatedCookingFromDb = await _context.CookingClasses.FindAsync(20);
            Assert.IsNotNull(updatedCookingFromDb);
            Assert.AreEqual("Advanced Strength Cooking", updatedCookingFromDb.ClassName);
        }
        else
        {
            Assert.Fail();
        }
    }

    [Test, Order(7)]
    public async Task Backend_Test_Delete_Method_DeleteCookingClass_In_CookingClass_Service_Deletes_CookingClass_Successfully()
    {
        ClearDatabase();

        var cookingData = new Dictionary<string, object>
        {
           { "CookingClassId", 4 }, // Unique identifier for the cooking class
            { "ClassName", "Italian Cooking Basics" }, // Name of the cooking class
            { "CuisineType", "Italian" }, // Type of cuisine covered in the class
            { "ChefName", "Chef Mario" }, // Name of the chef conducting the class
            { "Location", "Kitchen Studio" }, // Venue of the class
            { "DurationInHours", 3 }, // Duration of the class in hours
            { "Fee", 100m }, // Cost for attending the class
            { "IngredientsProvided", "Pasta, Olive Oil, Tomatoes, Cheese" }, // Ingredients provided during the class
            { "SkillLevel", "Beginner" }, // Skill level required
            { "SpecialRequirements", "Ability to stand for 3 hours" }
        };

        var cooking = new CookingClass();
        foreach (var kvp in cookingData)
        {
            var propertyInfo = typeof(CookingClass).GetProperty(kvp.Key);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(cooking, kvp.Value);
            }
        }

        _context.CookingClasses.Add(cooking);
        _context.SaveChanges();

        string assemblyName = "dotnetapp";
        Assembly assembly = Assembly.Load(assemblyName);
        string ServiceName = "dotnetapp.Services.CookingClassService";
        string typeName = "dotnetapp.Models.CookingClass";

        Type serviceType = assembly.GetType(ServiceName);
        Type modelType = assembly.GetType(typeName);

        MethodInfo deleteMethod = serviceType.GetMethod("DeleteCookingClass", new[] { typeof(int) });

        if (deleteMethod != null)
        {
            var service = Activator.CreateInstance(serviceType, _context);
            var deleteResult = (Task<bool>)deleteMethod.Invoke(service, new object[] { 4 });

            var deletedCookingFromDb = await _context.CookingClasses.FindAsync(4);
            Assert.IsNull(deletedCookingFromDb);
        }
        else
        {
            Assert.Fail();
        }
    }

    [Test, Order(8)]
    public async Task Backend_Test_Post_Method_AddCookingClassRequest_In_CookingClassRequest_Service_Posts_Successfully()
    {
        ClearDatabase();

        // Add user
        var userData = new Dictionary<string, object>
        {
            { "UserId", 400 },
            { "Username", "testuser" },
            { "Password", "testpassword" },
            { "Email", "test@example.com" },
            { "MobileNumber", "1234567890" },
            { "UserRole", "User" }
        };

        var user = new User();
        foreach (var kvp in userData)
        {
            var propertyInfo = typeof(User).GetProperty(kvp.Key);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(user, kvp.Value);
            }
        }
        _context.Users.Add(user);
        _context.SaveChanges();

        // Add  cooking
        var cookingData = new Dictionary<string, object>
        {
            { "CookingClassId", 100 }, // Unique identifier for the cooking class
            { "ClassName", "Italian Cooking Basics" }, // Name of the cooking class
            { "CuisineType", "Italian" }, // Type of cuisine covered in the class
            { "ChefName", "Chef Mario" }, // Name of the chef conducting the class
            { "Location", "Kitchen Studio" }, // Venue of the class
            { "DurationInHours", 3 }, // Duration of the class in hours
            { "Fee", 100m }, // Cost for attending the class
            { "IngredientsProvided", "Pasta, Olive Oil, Tomatoes, Cheese" }, // Ingredients provided during the class
            { "SkillLevel", "Beginner" }, // Skill level required
            { "SpecialRequirements", "Ability to stand for 3 hours" }
        };

        var cooking = new CookingClass();
        foreach (var kvp in cookingData)
        {
            var propertyInfo = typeof(CookingClass).GetProperty(kvp.Key);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(cooking, kvp.Value);
            }
        }
        _context.CookingClasses.Add(cooking);
        _context.SaveChanges();

        // Add  cooking request
        string assemblyName = "dotnetapp";
        Assembly assembly = Assembly.Load(assemblyName);
        string ServiceName = "dotnetapp.Services.CookingClassRequestService";
        string typeName = "dotnetapp.Models.CookingClassRequest";

        Type serviceType = assembly.GetType(ServiceName);
        Type modelType = assembly.GetType(typeName);

        MethodInfo method = serviceType.GetMethod("AddCookingClassRequest", new[] { modelType });

        if (method != null)
        {
            var cookingRequestData = new Dictionary<string, object>
            {
                { "CookingClassRequestId", 200 }, // Unique identifier for the cooking class request
                { "UserId", 400 }, // ID of the user requesting the cooking class
                { "CookingClassId", 100 }, // ID of the cooking class being requested
                { "RequestDate", DateTime.Now.ToString("yyyy-MM-dd") }, // Date of the request, formatted as a date
                { "Status", "Pending" }, // Status of the request (e.g., Pending, Approved, Rejected)
                { "DietaryPreferences", "Vegetarian" }, // User's dietary preferences
                { "CookingGoals", "Learn Italian recipes" }, // User's goals for the cooking class
                { "Comments", "Excited to learn pasta-making techniques" }
            };

            var cookingRequest = Activator.CreateInstance(modelType);
            foreach (var kvp in cookingRequestData)
            {
                var propertyInfo = modelType.GetProperty(kvp.Key);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(cookingRequest, kvp.Value);
                }
            }
            var service = Activator.CreateInstance(serviceType, _context);
            var result = (Task<bool>)method.Invoke(service, new object[] { cookingRequest });
        
            var addedCookingRequest = await _context.CookingClassRequests.FindAsync(200);
            Assert.IsNotNull(addedCookingRequest);
            Assert.AreEqual("Pending", addedCookingRequest.Status);
            Assert.AreEqual("Vegetarian", addedCookingRequest.DietaryPreferences);
            Assert.AreEqual("Learn Italian recipes", addedCookingRequest.CookingGoals);
            Assert.AreEqual("Excited to learn pasta-making techniques", addedCookingRequest.Comments);
        }
        else
        {  
            Assert.Fail();
        }
    }

    [Test, Order(9)]
    public async Task Backend_Test_Get_Method_GetCookingClassRequestByUserId_In_CookingClassRequest_Fetches_Successfully()
    {
        ClearDatabase();

        // Add user
        var userData = new Dictionary<string, object>
        {
            { "UserId", 400 },
            { "Username", "testuser" },
            { "Password", "testpassword" },
            { "Email", "test@example.com" },
            { "MobileNumber", "1234567890" },
            { "UserRole", "User" }
        };

        var user = new User();
        foreach (var kvp in userData)
        {
            var propertyInfo = typeof(User).GetProperty(kvp.Key);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(user, kvp.Value);
            }
        }
        _context.Users.Add(user);
        _context.SaveChanges();

        // Add  cooking
        var cookingData = new Dictionary<string, object>
        {
            { "CookingClassId", 100 }, // Unique identifier for the cooking class
            { "ClassName", "Italian Cooking Basics" }, // Name of the cooking class
            { "CuisineType", "Italian" }, // Type of cuisine covered in the class
            { "ChefName", "Chef Mario" }, // Name of the chef conducting the class
            { "Location", "Kitchen Studio" }, // Venue of the class
            { "DurationInHours", 3 }, // Duration of the class in hours
            { "Fee", 100m }, // Cost for attending the class
            { "IngredientsProvided", "Pasta, Olive Oil, Tomatoes, Cheese" }, // Ingredients provided during the class
            { "SkillLevel", "Beginner" }, // Skill level required
            { "SpecialRequirements", "Ability to stand for 3 hours" }
        };

        var cooking = new CookingClass();
        foreach (var kvp in cookingData)
        {
            var propertyInfo = typeof(CookingClass).GetProperty(kvp.Key);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(cooking, kvp.Value);
            }
        }
        _context.CookingClasses.Add(cooking);
        _context.SaveChanges();

        // Add  cooking request
        var requestData = new Dictionary<string, object>
        {
            { "CookingClassRequestId", 200 }, // Unique identifier for the cooking class request
                { "UserId", 400 }, // ID of the user requesting the cooking class
                { "CookingClassId", 100 }, // ID of the cooking class being requested
                { "RequestDate", DateTime.Now.ToString("yyyy-MM-dd") }, // Date of the request, formatted as a date
                { "Status", "Pending" }, // Status of the request (e.g., Pending, Approved, Rejected)
                { "DietaryPreferences", "Vegetarian" }, // User's dietary preferences
                { "CookingGoals", "Learn Italian recipes" }, // User's goals for the cooking class
                { "Comments", "Excited to learn pasta-making techniques" }
        };

        var request = new CookingClassRequest();
        foreach (var kvp in requestData)
        {
            var propertyInfo = typeof(CookingClassRequest).GetProperty(kvp.Key);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(request, kvp.Value);
            }
        }
        _context.CookingClassRequests.Add(request);
        _context.SaveChanges();

        string assemblyName = "dotnetapp";
        Assembly assembly = Assembly.Load(assemblyName);
        string ServiceName = "dotnetapp.Services.CookingClassRequestService";
        string typeName = "dotnetapp.Models.CookingClassRequest";

        Type serviceType = assembly.GetType(ServiceName);
        Type modelType = assembly.GetType(typeName);

        MethodInfo method = serviceType.GetMethod("GetCookingClassRequestsByUserId");

        if (method != null)
        {
            var service = Activator.CreateInstance(serviceType, _context);
            var result = (Task<IEnumerable<CookingClassRequest>>)method.Invoke(service, new object[] { 400 });
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Result.Any(item => item.Comments == "Excited to learn pasta-making techniques"));
        }
        else
        {
            Assert.Fail();
        }
    }

    [Test, Order(10)]
    public async Task Backend_Test_Put_Method_Update_In_CookingClassRequest_Service_Updates_Successfully()
    {
        ClearDatabase();

        // Add user
        var userData = new Dictionary<string, object>
        {
            { "UserId", 400 },
            { "Username", "testuser" },
            { "Password", "testpassword" },
            { "Email", "test@example.com" },
            { "MobileNumber", "1234567890" },
            { "UserRole", "User" }
        };

        var user = new User();
        foreach (var kvp in userData)
        {
            var propertyInfo = typeof(User).GetProperty(kvp.Key);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(user, kvp.Value);
            }
        }
        _context.Users.Add(user);
        _context.SaveChanges();

        // Add  cooking
        var cookingData = new Dictionary<string, object>
        {
            { "CookingClassId", 100 }, // Unique identifier for the cooking class
            { "ClassName", "Italian Cooking Basics" }, // Name of the cooking class
            { "CuisineType", "Italian" }, // Type of cuisine covered in the class
            { "ChefName", "Chef Mario" }, // Name of the chef conducting the class
            { "Location", "Kitchen Studio" }, // Venue of the class
            { "DurationInHours", 3 }, // Duration of the class in hours
            { "Fee", 100m }, // Cost for attending the class
            { "IngredientsProvided", "Pasta, Olive Oil, Tomatoes, Cheese" }, // Ingredients provided during the class
            { "SkillLevel", "Beginner" }, // Skill level required
            { "SpecialRequirements", "Ability to stand for 3 hours" }
        };

        var cooking = new CookingClass();
        foreach (var kvp in cookingData)
        {
            var propertyInfo = typeof(CookingClass).GetProperty(kvp.Key);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(cooking, kvp.Value);
            }
        }
        _context.CookingClasses.Add(cooking);
        _context.SaveChanges();

        // Add initial  cooking request
        var requestData = new Dictionary<string, object>
        {
            { "CookingClassRequestId", 200 }, // Unique identifier for the cooking class request
                { "UserId", 400 }, // ID of the user requesting the cooking class
                { "CookingClassId", 100 }, // ID of the cooking class being requested
                { "RequestDate", DateTime.Now.ToString("yyyy-MM-dd") }, // Date of the request, formatted as a date
                { "Status", "Pending" }, // Status of the request (e.g., Pending, Approved, Rejected)
                { "DietaryPreferences", "Vegetarian" }, // User's dietary preferences
                { "CookingGoals", "Learn Italian recipes" }, // User's goals for the cooking class
                { "Comments", "Excited to learn pasta" }
        };

        var request = new CookingClassRequest();
        foreach (var kvp in requestData)
        {
            var propertyInfo = typeof(CookingClassRequest).GetProperty(kvp.Key);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(request, kvp.Value);
            }
        }
        _context.CookingClassRequests.Add(request);
        _context.SaveChanges();

        // Update request data
        var updatedRequestData = new Dictionary<string, object>
        {
            { "CookingClassRequestId", 200 }, // Unique identifier for the cooking class request
                { "UserId", 400 }, // ID of the user requesting the cooking class
                { "CookingClassId", 100 }, // ID of the cooking class being requested
                { "RequestDate", DateTime.Now.ToString("yyyy-MM-dd") }, // Date of the request, formatted as a date
                { "Status", "Approved" }, // Status of the request (e.g., Pending, Approved, Rejected)
                { "DietaryPreferences", "Vegetarian" }, // User's dietary preferences
                { "CookingGoals", "Learn Italian recipes" }, // User's goals for the cooking class
                { "Comments", "Updated request comments" }
        };

        string assemblyName = "dotnetapp";
        Assembly assembly = Assembly.Load(assemblyName);
        string ServiceName = "dotnetapp.Services.CookingClassRequestService";
        string typeName = "dotnetapp.Models.CookingClassRequest";

        Type serviceType = assembly.GetType(ServiceName);
        Type modelType = assembly.GetType(typeName);

        MethodInfo method = serviceType.GetMethod("UpdateCookingClassRequest", new[] { typeof(int), modelType });

        if (method != null)
        {
            var updatedRequest = Activator.CreateInstance(modelType);
            foreach (var kvp in updatedRequestData)
            {
                var propertyInfo = typeof(CookingClassRequest).GetProperty(kvp.Key);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(updatedRequest, kvp.Value);
                }
            }

            var service = Activator.CreateInstance(serviceType, _context);
            var updateResult = (Task<bool>)method.Invoke(service, new object[] { 200, updatedRequest });
            var updatedRequestFromDb = await _context.CookingClassRequests.FindAsync(200);
            Assert.IsNotNull(updatedRequestFromDb);
            Assert.AreEqual("Approved", updatedRequestFromDb.Status);
            Assert.AreEqual("Updated request comments", updatedRequestFromDb.Comments);
        }
        else
        {
            Assert.Fail();
        }
    }


    [Test, Order(11)]
    public async Task Backend_Test_Delete_Method_DeleteCookingClassRequest_Service_Deletes_CookingClassRequest_Successfully()
    {
        ClearDatabase();

        // Add user
        var userData = new Dictionary<string, object>
        {
            { "UserId", 32 },
            { "Username", "testuser" },
            { "Password", "testpassword" },
            { "Email", "test@example.com" },
            { "MobileNumber", "1234567890" },
            { "UserRole", "User" }
        };

        var user = new User();
        foreach (var kvp in userData)
        {
            var propertyInfo = typeof(User).GetProperty(kvp.Key);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(user, kvp.Value);
            }
        }
        _context.Users.Add(user);
        _context.SaveChanges();

        // Add  cooking request
        var cookingRequestData = new Dictionary<string, object>
        {

            { "CookingClassRequestId", 200 }, // Unique identifier for the cooking class request
                { "UserId", 32 }, // ID of the user requesting the cooking class
                { "CookingClassId", 100 }, // ID of the cooking class being requested
                { "RequestDate", DateTime.Now.ToString("yyyy-MM-dd") }, // Date of the request, formatted as a date
                { "Status", "Pending" }, // Status of the request (e.g., Pending, Approved, Rejected)
                { "DietaryPreferences", "Vegetarian" }, // User's dietary preferences
                { "CookingGoals", "Learn Italian recipes" }, // User's goals for the cooking class
                { "Comments", "Excited to learn pasta-making techniques" }
        };

        var request = new CookingClassRequest();
        foreach (var kvp in cookingRequestData)
        {
            var propertyInfo = typeof(CookingClassRequest).GetProperty(kvp.Key);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(request, kvp.Value);
            }
        }
        _context.CookingClassRequests.Add(request);
        _context.SaveChanges();

        // Delete request
        string assemblyName = "dotnetapp";
        Assembly assembly = Assembly.Load(assemblyName);
        string ServiceName = "dotnetapp.Services.CookingClassRequestService";
        string typeName = "dotnetapp.Models.CookingClassRequest";

        Type serviceType = assembly.GetType(ServiceName);
        Type modelType = assembly.GetType(typeName);

        MethodInfo deleteMethod = serviceType.GetMethod("DeleteCookingClassRequest", new[] { typeof(int) });

        if (deleteMethod != null)
        {
            var service = Activator.CreateInstance(serviceType, _context);
            var deleteResult = (Task<bool>)deleteMethod.Invoke(service, new object[] { 200 });

            var deletedRequestFromDb = await _context.CookingClassRequests.FindAsync(200);
            Assert.IsNull(deletedRequestFromDb);
        }
        else
        {
            Assert.Fail();
        }
    }
    [Test, Order(12)]
    public async Task Backend_Test_Post_Method_AddFeedback_In_Feedback_Service_Posts_Successfully()
    {
            ClearDatabase();

        // Add user
        var userData = new Dictionary<string, object>
        {
            { "UserId",42 },
            { "Username", "testuser" },
            { "Password", "testpassword" },
            { "Email", "test@example.com" },
            { "MobileNumber", "1234567890" },
            { "UserRole", "User" }
        };

        var user = new User();
        foreach (var kvp in userData)
        {
            var propertyInfo = typeof(User).GetProperty(kvp.Key);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(user, kvp.Value);
            }
        }
        _context.Users.Add(user);
        _context.SaveChanges();
        // Add loan application
        string assemblyName = "dotnetapp";
        Assembly assembly = Assembly.Load(assemblyName);
        string ServiceName = "dotnetapp.Services.FeedbackService";
        string typeName = "dotnetapp.Models.Feedback";

        Type serviceType = assembly.GetType(ServiceName);
        Type modelType = assembly.GetType(typeName);

        MethodInfo method = serviceType.GetMethod("AddFeedback", new[] { modelType });

        if (method != null)
        {
            var feedbackData = new Dictionary<string, object>
                {
                    { "FeedbackId", 11 },
                    { "UserId", 42 },
                    { "FeedbackText", "Great experience!" },
                    { "Date", DateTime.Now }
                };
            var feedback = new Feedback();
            foreach (var kvp in feedbackData)
            {
                var propertyInfo = typeof(Feedback).GetProperty(kvp.Key);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(feedback, kvp.Value);
                }
            }
            var service = Activator.CreateInstance(serviceType, _context);
            var result = (Task<bool>)method.Invoke(service, new object[] { feedback });
        
            var addedFeedback= await _context.Feedbacks.FindAsync(11);
            Assert.IsNotNull(addedFeedback);
            Assert.AreEqual("Great experience!",addedFeedback.FeedbackText);

        }
        else{
            Assert.Fail();
        }
    }

    [Test, Order(13)]
    public async Task Backend_Test_Delete_Method_Feedback_In_Feeback_Service_Deletes_Successfully()
    {
        // Add user
        ClearDatabase();

        var userData = new Dictionary<string, object>
        {
            { "UserId",42 },
            { "Username", "testuser" },
            { "Password", "testpassword" },
            { "Email", "test@example.com" },
            { "MobileNumber", "1234567890" },
            { "UserRole", "User" }
        };

        var user = new User();
        foreach (var kvp in userData)
        {
            var propertyInfo = typeof(User).GetProperty(kvp.Key);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(user, kvp.Value);
            }
        }
        _context.Users.Add(user);
        _context.SaveChanges();

            var feedbackData = new Dictionary<string, object>
                {
                    { "FeedbackId", 11 },
                    { "UserId", 42 },
                    { "FeedbackText", "Great experience!" },
                    { "Date", DateTime.Now }
                };
            var feedback = new Feedback();
            foreach (var kvp in feedbackData)
            {
                var propertyInfo = typeof(Feedback).GetProperty(kvp.Key);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(feedback, kvp.Value);
                }
            }
        _context.Feedbacks.Add(feedback);
        _context.SaveChanges();
        // Add loan application
        string assemblyName = "dotnetapp";
        Assembly assembly = Assembly.Load(assemblyName);
        string ServiceName = "dotnetapp.Services.FeedbackService";
        string typeName = "dotnetapp.Models.Feedback";

        Type serviceType = assembly.GetType(ServiceName);
        Type modelType = assembly.GetType(typeName);

    
        MethodInfo deletemethod = serviceType.GetMethod("DeleteFeedback", new[] { typeof(int) });

        if (deletemethod != null)
        {
            var service = Activator.CreateInstance(serviceType, _context);
            var deleteResult = (Task<bool>)deletemethod.Invoke(service, new object[] { 11 });

            var deletedFeedbackFromDb = await _context.Feedbacks.FindAsync(11);
            Assert.IsNull(deletedFeedbackFromDb);
        }
        else
        {
            Assert.Fail();
        }
    }
    [Test, Order(14)]
    public async Task Backend_Test_Get_Method_GetFeedbacksByUserId_In_Feedback_Service_Fetches_Successfully()
    {
        ClearDatabase();

        // Add user
        var userData = new Dictionary<string, object>
        {
            { "UserId", 330 },
            { "Username", "testuser" },
            { "Password", "testpassword" },
            { "Email", "test@example.com" },
            { "MobileNumber", "1234567890" },
            { "UserRole", "User" }
        };

        var user = new User();
        foreach (var kvp in userData)
        {
            var propertyInfo = typeof(User).GetProperty(kvp.Key);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(user, kvp.Value);
            }
        }
        _context.Users.Add(user);
        _context.SaveChanges();

        var feedbackData= new Dictionary<string, object>
        {
            { "FeedbackId", 13 },
            { "UserId", 330 },
            { "FeedbackText", "Great experience!" },
            { "Date", DateTime.Now }
        };

        var feedback = new Feedback();
        foreach (var kvp in feedbackData)
        {
            var propertyInfo = typeof(Feedback).GetProperty(kvp.Key);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(feedback, kvp.Value);
            }
        }
        _context.Feedbacks.Add(feedback);
        _context.SaveChanges();

        // Add loan application
        string assemblyName = "dotnetapp";
        Assembly assembly = Assembly.Load(assemblyName);
        string ServiceName = "dotnetapp.Services.FeedbackService";
        string typeName = "dotnetapp.Models.Feedback";

        Type serviceType = assembly.GetType(ServiceName);
        Type modelType = assembly.GetType(typeName);

        MethodInfo method = serviceType.GetMethod("GetFeedbacksByUserId");

        if (method != null)
        {
            var service = Activator.CreateInstance(serviceType, _context);
            var result = ( Task<IEnumerable<Feedback>>)method.Invoke(service, new object[] {330});
            Assert.IsNotNull(result);
            var check=true;
            foreach (var item in result.Result)
            {
                check=false;
                Assert.AreEqual("Great experience!", item.FeedbackText);
    
            }
            if(check==true)
            {
                Assert.Fail();

            }
        }
        else{
            Assert.Fail();
        }
    }
    [Test, Order(15)]


    public async Task Backend_Test_Post_Method_AddCookingClass_In_CookingClassService_Throws_Exception_For_Duplicate_ClassName()
    {
        ClearDatabase();

        string assemblyName = "dotnetapp";
        Assembly assembly = Assembly.Load(assemblyName);
        string ServiceName = "dotnetapp.Services.CookingClassService";
        string typeName = "dotnetapp.Models.CookingClass";
    
        Type serviceType = assembly.GetType(ServiceName);
        Type modelType = assembly.GetType(typeName);
    
        MethodInfo method = serviceType.GetMethod("AddCookingClass", new[] { modelType });
    
        if (method != null)
        {
            // Add initial CookingClass with a unique ClassName
            var cookingData = new Dictionary<string, object>
            {
                { "CookingClassId", 1 }, // Unique identifier for the cooking class
                { "ClassName", "Italian Cooking Basics" }, // Name of the cooking class
                { "CuisineType", "Italian" }, // Type of cuisine covered in the class
                { "ChefName", "Chef Mario" }, // Name of the chef conducting the class
                { "Location", "Kitchen Studio" }, // Venue of the class
                { "DurationInHours", 3 }, // Duration of the class in hours
                { "Fee", 100m }, // Cost for attending the class
                { "IngredientsProvided", "Pasta, Olive Oil, Tomatoes, Cheese" }, // Ingredients provided during the class
                { "SkillLevel", "Beginner" }, // Skill level required
                { "SpecialRequirements", "Ability to stand for 3 hours" }
            };

            var cooking = Activator.CreateInstance(modelType);
            foreach (var kvp in cookingData)
            {
                var propertyInfo = modelType.GetProperty(kvp.Key);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(cooking, kvp.Value);
                }
            }

            var service = Activator.CreateInstance(serviceType, _context);
            var result = (Task<bool>)method.Invoke(service, new object[] { cooking });
            var addedCooking = await _context.CookingClasses.FindAsync(1);
            Assert.IsNotNull(addedCooking);

            // Attempt to add another CookingClass with the same ClassName
            var duplicateCookingData = new Dictionary<string, object>
            {
                { "CookingClassId", 2 }, // Unique identifier for the cooking class
                { "ClassName", "Italian Cooking Basics" }, // Name of the cooking class
                { "CuisineType", "French" }, // Type of cuisine covered in the class
                { "ChefName", "Chef Pierre" }, // Name of the chef conducting the class
                { "Location", "Gourmet Baking Studio" }, // Venue of the class
                { "DurationInHours", 4 }, // Duration of the class in hours
                { "Fee", 150m }, // Cost for attending the class
                { "IngredientsProvided", "Flour, Butter, Sugar, Chocolate" }, // Ingredients provided during the class
                { "SkillLevel", "Intermediate" }, // Skill level required
                { "SpecialRequirements", "Ability to stand for 4 hours and handle kitchen tools" }
            };

            var duplicateCooking = Activator.CreateInstance(modelType);
            foreach (var kvp in duplicateCookingData)
            {
                var propertyInfo = modelType.GetProperty(kvp.Key);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(duplicateCooking, kvp.Value);
                }
            }

            try
            {
                var duplicateResult = (Task<bool>)method.Invoke(service, new object[] { duplicateCooking });
                Console.WriteLine("Result: " + duplicateResult.Result);
                Assert.Fail("Expected CookingClassException was not thrown.");
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex.InnerException);
                Assert.IsTrue(ex.InnerException is CookingClassException);
                Assert.AreEqual("Cooking class with the same name already exists", ex.InnerException.Message);
            }
        }
        else
        {
            Assert.Fail("Method AddCookingClass not found in CookingClassService.");
        }
    }


    private void ClearDatabase()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

}
}