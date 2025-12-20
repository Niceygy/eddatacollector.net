FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base

WORKDIR /home

COPY bin/Release/net10.0/linux-arm64/publish .

RUN chmod 777 /home/eddatacollector.net

LABEL org.opencontainers.image.description="EDDataCollector"
LABEL org.opencontainers.image.authors="Niceygy (Ava Whale)"

CMD [ "/home/eddatacollector.net" ]