# Asp Core web api startup project
The project aids with developer who quickly create new project for web api purpose. it includes three layers which is UI/Biz/Dao.

# The project includes
- Global exception handling
- Token handling
- Web api format agreement
- Web api sample wihch run in project
- Unit test support

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
