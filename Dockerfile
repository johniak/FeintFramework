FROM mcr.microsoft.com/dotnet/core/sdk:3.1
WORKDIR /app
RUN apt-get update && apt-get -y install postgresql
RUN dotnet tool install --global dotnet-ef
# copy csproj and restore as distinct layers
COPY ./FeintSDK/app.csproj ./FeintSDK/app.csproj
COPY ./FeintApi/app.csproj ./FeintApi/app.csproj
COPY ./FeintSite/app.csproj ./FeintSite/app.csproj
WORKDIR /app/FeintSite
RUN printf '\nexport PATH="$PATH:/root/.dotnet/tools"\n'  >> ~/.bashrc
ENV PATH="/root/.dotnet/tools:${PATH}"
RUN dotnet restore
WORKDIR /app
COPY . .
WORKDIR /app/FeintSite
