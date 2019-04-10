module Microsoft.AspNetCore.Builder

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Identity
open Test.FSharp.Models
open System.Linq
open Test.FSharp.DataAccess

type IApplicationBuilder with
    member this.Initialize() =
        use scope = this.ApplicationServices.CreateScope()
        let context = scope.ServiceProvider.GetService<ApplicationContext>()
        let roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>()
        let userManager = scope.ServiceProvider.GetService<UserManager<User>>()
        
        if not(userManager.Users.Any(fun x -> x.UserName = "ztytotoro")) then
            let user = User(
                        UserName = "ztytotoro",
                        Email = "ztytotoro@outlook.com",
                        EmailConfirmed = true)
            userManager.CreateAsync(user, "z@2019Code4fun").GetAwaiter().GetResult() |> ignore