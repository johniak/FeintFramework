FROM microsoft/dotnet:sdk
WORKDIR /app

# copy csproj and restore as distinct layers
COPY . .
WORKDIR /app/FeintSite
RUN dotnet restore

# copy and build everything else
#RUN dotnet publish -c Release -o out
#ENTRYPOINT ["dotnet", "out/dotnetapp.dll"]
