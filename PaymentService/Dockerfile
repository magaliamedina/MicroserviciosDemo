FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY PaymentService.csproj .
RUN dotnet restore PaymentService.csproj
COPY . .
RUN dotnet publish PaymentService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
RUN mkdir -p /app/logs
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PaymentService.dll"]