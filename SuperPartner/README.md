# Web api layer includes items
- Controller class
- Global Exception Handling (Exceptoin Filter)
- Authorization Processing (Authorization, which includes check permission, refresh token, get associate data of object etc)
- Logger initial (Log4Net)

# Controller class agreement
- DO NOT have if/for/while statement in method
- Keep Api method simple as possible
- Authorize will be apply in API method
- Use WsResponse or WsResponse<> as response object
- DO NOT try catch exception in the Api method. Exceptions will be handled in global(SpExceptionFilter class). You show define the error message for unhandled exception

# Response object
The object will use as response object. It include Trans property. If there are any exception, the ErrorCode of Trans will be non-zero

# Controller code (Inject manager object to controller class)
```c#
        private UserManager userManager;
        public UserController(UserManager userManager)
        {
            this.userManager = userManager;
        }
```
# Api method (Not apply function permission)
```c#
        [HttpGet("count")]
        public ActionResult<WsResponse<int>> GetUserCount()
        {
            WsResponse<int> result = this.userManager.GetUserCount();
            return result;
        }
```

# TODO
- Intergrate swagger into project
- Log fatel information include request body, header etc.