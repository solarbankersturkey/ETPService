#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 443
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["CustomerService/customerservice.csproj", "CustomerService/"]
RUN dotnet restore "CustomerService/customerservice.csproj"
COPY . .
WORKDIR "/src/CustomerService"
RUN dotnet build "customerservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "customerservice.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "customerservice.dll"]