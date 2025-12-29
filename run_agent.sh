#!/bin/bash
# Convenience script to run the VAPT Agent

echo "🚀 Starting VAPT Agent..."

# Check if .env file exists and load it
if [ -f ".env" ]; then
    export $(grep -v '^#' .env | xargs)
fi

# Run the agent
python scripts/vapt_agent.py