#!/bin/bash

DB_HOST=${DB_HOST:-postgres}
DB_PORT=${DB_PORT:-5432}
DB_USER=${DB_USER:-postgres}

echo "🔄 Aguardando banco de dados..."
while ! pg_isready -h $DB_HOST -p $DB_PORT -U $DB_USER -q; do
    echo "⏳ Banco ainda não disponível, aguardando..."
    sleep 2
done

echo "✅ Banco disponível! Aplicando migrations..."
cd /src/src/SnackFlow.Infrastructure
dotnet ef database update
echo "✅ Migrations concluídas!"