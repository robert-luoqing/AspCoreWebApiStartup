# How to generate data context from exist database
Please advise by https://docs.microsoft.com/en-us/ef/core/get-started/aspnetcore/existing-db?toc=%2Faspnet%2Fcore%2Ftoc.json&bc=%2Faspnet%2Fcore%2Fbreadcrumb%2Ftoc.json&view=aspnetcore-2.2

# The step of generate context from database
- Tools –> NuGet Package Manager –> Package Manager Console
- Choose default project as "SuperPartner.DataLayer"
- Input Scaffold-DbContext "Server=localhost;Database=SpFramework;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir DataContext in console

# Business class agreement
- All class name must end with "Dao". It will automatically inject to IoC.
- Don't write business logic in the layer
- DO NOT have if/for/while statement in method except query, batch operation
