FROM microsoft/dotnet:sdk
WORKDIR /app

# copy csproj and restore as distinct layers
COPY ./ExampleApi.csproj ./ExampleApi.csproj
RUN dotnet restore
COPY . .
RUN dotnet ef database update || true
EXPOSE 5000
CMD ["dotnet", "watch", "run"]
