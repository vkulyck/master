#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["GmWeb.Web.Api/GmWeb.Web.Api.csproj", "GmWeb.Web.Api/"]
COPY ["GmWeb.Web.Common/GmWeb.Web.Common.csproj", "GmWeb.Web.Common/"]
COPY ["GmWeb.Logic/GmWeb.Logic.Standard.csproj", "GmWeb.Logic/"]
COPY ["GmWeb.Common/GmWeb.Common.Standard.csproj", "GmWeb.Common/"]
RUN dotnet restore "GmWeb.Web.Api/GmWeb.Web.Api.csproj"
COPY . .
WORKDIR "/src/GmWeb.Web.Api"
RUN dotnet build "GmWeb.Web.Api.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "GmWeb.Web.Api.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GmWeb.Web.Api.dll"]