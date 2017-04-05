FROM microsoft/dotnet:sdk
WORKDIR /app

# copy csproj and restore as distinct layers
COPY ./FeintServer/app.csproj ./FeintServer/app.csproj
COPY ./FeintSDK/app.csproj ./FeintSDK/app.csproj
COPY ./FeintApi/app.csproj ./FeintApi/app.csproj
COPY ./FeintSite/app.csproj ./FeintSite/app.csproj
WORKDIR /app/FeintSite
RUN dotnet restore
WORKDIR /app
COPY . .
WORKDIR /app/FeintSite
RUN dotnet ef database update
