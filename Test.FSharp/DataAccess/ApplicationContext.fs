namespace Test.FSharp.DataAccess

open Microsoft.AspNetCore.Identity.EntityFrameworkCore
open Microsoft.EntityFrameworkCore
open Test.FSharp.Models

type public ApplicationContext(options: DbContextOptions<ApplicationContext>) =
    inherit IdentityDbContext<User>(options)

    override __.OnModelCreating(builder: ModelBuilder) =
        base.OnModelCreating(builder)