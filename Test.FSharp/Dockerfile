FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Test.FSharp/Test.FSharp.fsproj", "Test.FSharp/"]
RUN dotnet restore "Test.FSharp/Test.FSharp.fsproj"
COPY . .
WORKDIR "/src/Test.FSharp"
RUN dotnet build "Test.FSharp.fsproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Test.FSharp.fsproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Test.FSharp.dll"]