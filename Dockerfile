FROM mcr.microsoft.com/dotnet/runtime:9.0-azurelinux3.0-distroless AS base

WORKDIR /home

COPY bin/Release/net9.0/linux-arm64/publish .

LABEL org.opencontainers.image.description="EDDataCollector"
LABEL org.opencontainers.image.authors="Niceygy (Ava Whale)"

# RUN chmod 777 

CMD [ "./eddatacollector.net" ]