#!/bin/bash

set -e

echo "🚀 Starting deployment..."

# Build and run Docker containers
echo "🐳 Building and starting Docker containers..."
docker compose down || true
docker compose up -d --build

echo "✅ Deployment completed successfully!"
echo "🌐 App is running on port 3004"