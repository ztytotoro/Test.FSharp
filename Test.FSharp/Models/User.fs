namespace Test.FSharp.Models

open Microsoft.AspNetCore.Identity

type public User() =
    inherit IdentityUser()

    [<PersonalData>]
    member val Name: string = "" with get, set