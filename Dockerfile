FROM mcr.microsoft.com/dotnet/framework/runtime AS base

WORKDIR /home

COPY bin/Release/net9.0/linux-arm64/publish .

LABEL org.opencontainers.image.description="ED Port Test server, written in C#"
LABEL org.opencontainers.image.authors="Niceygy (Ava Whale)"

RUN chmod 777 EDPortTest

CMD [ "./EDPortTest" ]