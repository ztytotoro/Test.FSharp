﻿namespace Test.FSharp.Controllers

open Microsoft.AspNetCore.Mvc

[<Route("api/[controller]")>]
[<ApiController>]
type ValuesController () =
    inherit ControllerBase()

    [<HttpGet>]
    member this.Get() =
        let values = [|"value1"; "value2"|]
        this.Ok(values)
        // ActionResult<string[]>(values)

    [<HttpGet("{id}")>]
    member this.Get(id:int) =
        let value = "value"
        ActionResult<string>(value)

    [<HttpPost>]
    member this.Post([<FromBody>] value:string) =
        ()

    [<HttpPut("{id}")>]
    member this.Put(id:int, [<FromBody>] value:string ) =
        ()

    [<HttpDelete("{id}")>]
    member this.Delete(id:int) =
        ()
