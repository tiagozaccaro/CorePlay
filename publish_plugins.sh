#!/bin/bash

# Default configuration and output folder
CONFIGURATION="Debug"
OUTPUT_FOLDER="deploy/plugins"

# Check if a configuration argument was provided
if [ "$1" == "Release" ] || [ "$1" == "Debug" ]; then
  CONFIGURATION=$1
fi

# List of projects
PROJECTS=(
  "CorePlay.Plugins.Metadata.SteamGridDB/CorePlay.Plugins.Metadata.SteamGridDB.csproj"
  "CorePlay.Plugins.Metadata.IGDB/CorePlay.Plugins.Metadata.IGDB.csproj"
  "CorePlay.Plugins.Library.SteamLibrary/CorePlay.Plugins.Library.SteamLibrary.csproj"
  # Add more projects here
)

# Loop through each project and publish
for PROJECT in "${PROJECTS[@]}"; do
  PROJECT_NAME=$(basename "$PROJECT" .csproj)
  PUBLISH_PATH="$OUTPUT_FOLDER/$PROJECT_NAME/$CONFIGURATION"

  echo "Publishing $PROJECT to $PUBLISH_PATH with configuration $CONFIGURATION..."
  dotnet publish "$PROJECT" -c "$CONFIGURATION" -o "$PUBLISH_PATH"
  
  if [ $? -ne 0 ]; then
    echo "Failed to publish $PROJECT"
    exit 1
  fi

  echo "Successfully published $PROJECT to $PUBLISH_PATH"
done

echo "All projects have been published to $OUTPUT_FOLDER with configuration $CONFIGURATION."
