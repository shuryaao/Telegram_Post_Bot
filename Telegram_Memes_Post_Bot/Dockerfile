#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 5013
EXPOSE 5014

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Telegram_Memes_Post_Bot/Telegram_Memes_Post_Bot.csproj", "Telegram_Memes_Post_Bot/"]
RUN dotnet restore "Telegram_Memes_Post_Bot/Telegram_Memes_Post_Bot.csproj"
COPY . .
WORKDIR "/src/Telegram_Memes_Post_Bot"
RUN dotnet build "Telegram_Memes_Post_Bot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Telegram_Memes_Post_Bot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Telegram_Memes_Post_Bot.dll"]