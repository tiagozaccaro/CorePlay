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
  "CorePlay.Plugins.Metadata.ScreenScraper/CorePlay.Plugins.Metadata.ScreenScraper.csproj"
  "CorePlay.Plugins.Metadata.SteamGridDB/CorePlay.Plugins.Metadata.SteamGridDB.csproj"
  "CorePlay.Plugins.Metadata.IGDB/CorePlay.Plugins.Metadata.IGDB.csproj"
  "CorePlay.Plugins.Library.SteamLibrary/CorePlay.Plugins.Library.SteamLibrary.csproj"
  # Add more projects here
)

# Ensure the output folder exists
mkdir -p "$OUTPUT_FOLDER"

# Loop through each project to restore and publish
for PROJECT in "${PROJECTS[@]}"; do
  # Restore the project
  echo "Restoring $PROJECT..."
  dotnet restore "$PROJECT"
  
  if [ $? -ne 0 ]; then
    echo "Failed to restore $PROJECT"
    exit 1
  fi

  # Publish the project to a temporary folder inside the output folder
  TEMP_FOLDER="$OUTPUT_FOLDER/tmp"
  mkdir -p "$TEMP_FOLDER"

  echo "Publishing $PROJECT to $TEMP_FOLDER with configuration $CONFIGURATION..."
  dotnet publish "$PROJECT" -c "$CONFIGURATION" -o "$TEMP_FOLDER" --no-restore --force
  
  if [ $? -ne 0 ]; then
    echo "Failed to publish $PROJECT"
    exit 1
  fi

  # Move all files from the temporary folder to the root output folder
  echo "Moving files from $TEMP_FOLDER to $OUTPUT_FOLDER..."
  mv "$TEMP_FOLDER"/* "$OUTPUT_FOLDER"

  # Remove the temporary folder
  rm -rf "$TEMP_FOLDER"

  echo "Successfully published $PROJECT and moved files to $OUTPUT_FOLDER"
done

echo "All projects have been restored, published, and cleaned up to $OUTPUT_FOLDER with configuration $CONFIGURATION."
