namespace Test.FSharp

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Identity
open Test.FSharp.Models
open Test.FSharp.DataAccess
open System
open Microsoft.AspNetCore.Authentication.JwtBearer
open Microsoft.IdentityModel.Tokens
open Microsoft.EntityFrameworkCore
open Swashbuckle.AspNetCore.Swagger;
open System.Text

type Startup private () =

    new(configuration : IConfiguration) as this =
        Startup()
        then this.Configuration <- configuration

    member this.ConfigureServices(services : IServiceCollection) =
        services.Configure<CookiePolicyOptions>(fun (options : CookiePolicyOptions) ->
            options.CheckConsentNeeded <- fun context -> true
            options.MinimumSameSitePolicy <- SameSiteMode.None)
        |> ignore

        services.AddIdentityCore<User>(fun config -> config.SignIn.RequireConfirmedEmail <- true)
            .AddSignInManager()
            .AddRoles<IdentityRole>()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<ApplicationContext>
            () |> ignore

        services.Configure<DataProtectionTokenProviderOptions>
            (fun (o : DataProtectionTokenProviderOptions) ->
            o.TokenLifespan <- TimeSpan.FromHours(3.0)) |> ignore

        services.Configure<IdentityOptions>(fun (options : IdentityOptions) ->
            options.Password.RequireDigit <- true
            options.Password.RequireLowercase <- false
            options.Password.RequireNonAlphanumeric <- false
            options.Password.RequireUppercase <- false
            options.Password.RequiredLength <- 6
            options.Password.RequiredUniqueChars <- 0
            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan <- TimeSpan.FromMinutes(60.0)
            options.Lockout.MaxFailedAccessAttempts <- 5
            options.Lockout.AllowedForNewUsers <- true
            // User settings.
            options.User.AllowedUserNameCharacters <- "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"
            options.User.RequireUniqueEmail <- false)
        |> ignore

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(fun options ->
            options.TokenValidationParameters <- TokenValidationParameters(
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = this.Configuration.["Token:Audience"],
                ValidIssuer = this.Configuration.["Token:Issuer"],
                IssuerSigningKey = SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration.["Token:Key"]))
                )
            )
        |> ignore

        services.AddCors() |> ignore

        services.AddDbContext<ApplicationContext>(fun options ->
        options.UseNpgsql(this.Configuration.GetConnectionString("DefaultConnection")) |> ignore
        ).BuildServiceProvider()
        |> ignore

        services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
        |> ignore

        services.AddSwaggerGen(fun c -> c.SwaggerDoc("v1", Info(Title = "My API", Version = "v1")))
        |> ignore

    member __.Configure(app : IApplicationBuilder, env : IHostingEnvironment) =
        if (env.IsDevelopment()) then app.UseDeveloperExceptionPage() |> ignore
        else app.UseHsts() |> ignore

        app.UseHttpsRedirection() |> ignore
        app.UseAuthentication() |> ignore

        if env.IsDevelopment() then 
            app.UseSwagger() |> ignore
            app.UseSwaggerUI(fun c ->
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1")
            ) |> ignore

        app.UseCors(fun builder ->
        builder.WithOrigins("http://localhost:8080")
            .AllowCredentials()
            .AllowAnyMethod()
            .AllowAnyHeader()
            |> ignore
        ) |> ignore

        app.Initialize()
        
        app.UseMvc() |> ignore

    member val Configuration : IConfiguration = null with get, set
