FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 as build-env
ARG TARGETARCH

WORKDIR /app

# Copy  src directory and publish to output
COPY ./ .

RUN dotnet publish -a $TARGETARCH  /app/JWTAuthServer/JWTAuthServer.csproj -c Release -o /app/out

# change to published directory and set entry point
FROM mcr.microsoft.com/dotnet/aspnet:8.0.3-bookworm-slim
WORKDIR /app

COPY --from=build-env /app/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "JWTAuthServer.dll"]