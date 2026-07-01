#!/bin/bash

set -e

echo "🚀 Starting deployment..."

# Ensure we're on main branch and pull latest code
echo "📥 Pulling latest code..."
git fetch origin
git checkout main
git reset --hard origin/main

# Build and run Docker containers
echo "🐳 Building and starting Docker containers..."
docker compose down || true
docker compose up -d --build

echo "✅ Deployment completed successfully!"
echo "🌐 App is running on port 3004"