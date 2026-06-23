FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet build
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Copiar herramientas de EF
COPY --from=build /root/.dotnet /root/.dotnet

# Script para ejecutar migraciones
RUN echo '#!/bin/bash\n\
until pg_isready -h postgres -p 5432 > /dev/null 2>&1; do\n\
  echo "Esperando PostgreSQL..."\n\
  sleep 1\n\
done\n\
\n\
exec dotnet LogisticaApp.dll' > /app/entrypoint.sh && chmod +x /app/entrypoint.sh

EXPOSE 8080
ENTRYPOINT ["/app/entrypoint.sh"]
