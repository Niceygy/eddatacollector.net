FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base

WORKDIR /home

COPY bin/Release/net9.0/linux-arm64/publish .

# RUN ls /home && sleep 1000
# RUN /bin/sh -c ls /home



RUN chmod 777 /home/eddatacollector.net

LABEL org.opencontainers.image.description="EDDataCollector"
LABEL org.opencontainers.image.authors="Niceygy (Ava Whale)"

# RUN chmod 777 

CMD [ "/home/eddatacollector.net" ]