# Asp Core web api startup project
The project aids with developer who quickly create new project for web api purpose. it includes three layers which is UI/Biz/Dao.

# The project includes
- Global exception handling
- Token handling
- Web api format agreement
- Web api sample wihch run in project

# Token handler
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