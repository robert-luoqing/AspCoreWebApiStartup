# Asp Core web api startup project
The project aids with developer who quickly create new project for web api purpose. it includes three layers which is UI/Biz/Dao.

# The project includes
- Global exception handling
- Token handling
- Web api format agreement
- Web api sample wihch run in project
- Unit test support
# Global exception handling
The platform use log4net as logger.   
SpExceptionFilter.cs class will log more detail information if the unhandling exception coming from business loginc.
```c#
// Log more information as detail as possible
var request = context.HttpContext.Request;
Logger.Fatal("Request url: " + request.Path);
Logger.Fatal("Request Query String: " + request.QueryString.ToString());
request.Body.Seek(0, SeekOrigin.Begin);
using (var reader = new StreamReader(request.Body))
{
    var bodyContent = reader.ReadToEnd();
    Logger.Fatal("Request body: " + bodyContent);
}
```
We need follow the try catch rule
- Dont't swallow any exception
If any exception need be swallowed, please comment the detail information
- Don't simple try catch to switch to SpException
- Try-Catch-Log in all threading method
We don't want the whole processing will die if there are some unhandling exception in thread. We just want get notice in fatal logs.

# Token handler
I don't like use session in web api, so it should use token or jwt to handle. Current we just provide token handler    
<br>
There are two token handler in the framework
- Token save in memory
- Token save in redis
You can switch the token process in Startup.cs file
```c#
// Token handler, The default is save token and associate data in memoery.
services.AddSingleton<ITokenHandler>(new MemoryTokenHandler());
// You can use redis as token storage
// services.AddSingleton<ITokenHandler>(new RedisTokenHandler(new PermissionRedisHelper(settingModel.PermissionRedis, settingModel.RedisPrefix), "token"));
```
After that, you can inject "ITokenHandler" or inject "BizContext" to use TokenHandler , The basic scenaros has two place
- Get token and its associate data in SpAuthFilter.cs
```c#
	var token = context.HttpContext.Request.Headers["token"];
	spContext.Token = token;
	if(!string.IsNullOrEmpty(token))
	{
		spContext.LoginUser = tokenHandler.GetAssociateObjectByToken<LoginUser>(token);
	}
```
- Generate token in login in UserManager.cs. The TokenHandler was put in BizContext
```c#
    public string Login(string userName, string password)
    {
        // Verify user name and password
        // .....
        var token = this.BizContext.TokenHandler.GeneToken(user);
        return token;
    }
```

# Permission Checker
The framework implement simple permission checking. There are two check method. One is Url checking. Another is function point check.  
## Initial Check module
Put below code in ConfigureServices method of Startup.cs
```c#
// Permission implement
services.AddDbContext<PermissionDataContext>
        (options =>
        options.UseSqlServer(Configuration.GetConnectionString("SpFrameworkDatabase"), b => b.MigrationsAssembly("SuperPartner"))
            .UseLoggerFactory(MyLoggerFactory)
    );
services.AddScoped(typeof(IAuthorizationStorageProvider), typeof(AuthorizationStorageProvider));
services.AddScoped(typeof(IAuthorizationHandler), typeof(AuthorizationHandler));
```
## Url checking
If enable Url checking, you need set NeedCheckPermissionFromUrl to true. Of course, you can direct code in SpAuthFilter.cs
```json
    "NeedCheckPermissionFromUrl": true,
    "IgnoreCheckPermissionUrls": [
      "^/api/user/login"
    ],
    "IgnoreCheckPermissionUrlsWhenLogined": []
```
Put code to check url in SpAuthFilter.cs
```c#
if (bizContext.Configuration.NeedCheckPermissionFromUrl)
{
    var url = context.HttpContext.Request.Path.Value;
    var authorizationHandler = context.HttpContext.RequestServices.GetService(typeof(IAuthorizationHandler)) as IAuthorizationHandler;
    var isPassed = authorizationHandler.CheckUrl(bizContext.LoginUser.UserId.ToString(),
        url,
        bizContext.Configuration.IgnoreCheckPermissionUrls,
        bizContext.Configuration.IgnoreCheckPermissionUrlsWhenLogined);
    if (isPassed) return; // passed check permission

    // failed to check url according permission
    if (bizContext.LoginUser == null)
    {
        var response = new WsResponse();
        response.Trans.ErrorCode = "401";
        response.Trans.ErrorMsg = "Not login";
        var result = new JsonResult(response);
        result.StatusCode = 401;
        context.Result = result;
    }
    else
    {
        var response = new WsResponse();
        response.Trans.ErrorCode = "403";
        response.Trans.ErrorMsg = "Authorization Failed";
        var result = new JsonResult(response);
        result.StatusCode = 403;
        context.Result = result;
    }
}
```
## Function point checking
If enable function checking, you just put [SpFunction("UserOperation")] attribute on your API like below
```c#
[HttpPost("list")]
[SpFunction("UserOperation")]
public ActionResult<List<WsUserInfo>> GetUserList([FromBody] WsListRequest<string> req)
{
    return this.userManager.GetUserList(req.Condition, req.Pager);
}
```
Notice, "UserOperation" is function code. These code will configued in database. You can see "How to map functions to user"
## How to map functions to user
- Database install
You can create table from SuperPartner/Database/permission.sql. After create the table, you can confige the connect string in services.AddDbContext<PermissionDataContext> 
- Defined the function point. 
The function point includes the associate urls and the description.   
You can use "IAuthorizationStorageProvider.CreateOrUpdateFunc" to define a function point first
```c#
var provider = new AuthorizationStorageProvider(dataContext);
var extendProperties = new Dictionary<string, string>();
extendProperties.Add("Index", "0001");
provider.CreateOrUpdateFunc(new PermFunc()
{
    FuncCode = "UserOperation",
    FuncName = "User Operation",
    FuncDesc = "Operating user module, like query user list, add/update/delete user",
    AssociateUrls = "^/api/user/",
    ExtendProperties = extendProperties
});
```
- Assign the function to specify user.
```c#
provider.AssignFuncToUser("1", "UserOperation", AccessLevel.AccessSelfData);
```

# Unit test support
The platform have add unit test case which can help developer to do unit test, prepare unit test data (stub) and the environment.  
All test case will be put in SuperPartner.Test project. The root folder is corresponse with project name which you need test.
In the sample, You need test login method in UserManager.cs, The test case is put under SuperPartner.Test/BsinessLayer/Organization  
There are two files which are UserManagerTest.cs and UserManagerStub.cs
- Prepare environment
```c#
[TestMethod]
public void TestLoginWithNormalCorrectLogin()
{
    // Prepare env.
    var users = this.GetPrepareUsers();
    var stub = new UserManagerStub(users);

    var bizContext = GetBizContext();
    var userManager = new UserManager(bizContext, stub);
    .......
}
```
- The stub for the test case
```c#
public class UserManagerStub : UserDao
{
    private List<User> list;

    public UserManagerStub(List<User> list) : base(null)
    {
        this.list = list;
    }

    public override List<LoginUser> GetLoginUserByLoginName(string loginName)
    {
        return this.list.Where(o => o.LoginName == loginName).Select(o =>
        {
            var result = new LoginUser();
            ObjectHelper.CopyPropertyValue(o, result);
            return result;
        }).ToList();
    }

    public override void UpdateFailTimes(int userId, int failTime)
    {
        this.list.Where(o => o.UserId == userId).Single().FailTimes = failTime;
    }
}
```
- Test and verify
```c#
[TestMethod]
public void TestLoginWithNormalCorrectLogin()
{
    ......
    var token = userManager.Login("admin", "123456");

    // Check
    var loginUser = bizContext.TokenHandler.GetAssociateObjectByToken<LoginUser>(token);
    Assert.AreEqual(loginUser.UserId, 1);
    Assert.AreEqual(users[0].FailTimes, 0);
}
```

****
TODO:
- jwt support
- function permission support
- add rold into permssion
- add auth in swagger
- add OAuth support
