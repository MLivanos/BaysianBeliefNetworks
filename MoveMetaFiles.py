import os
import shutil

# Paths for your Unity project
UNITY_PATH = "C:/Users/prodi/UnityGames/CelestialAnomalies/BaysianBeliefNetworks/BaysianBeliefNetworkDemo/Assets/"
TEMP_ASSETS_DIR = os.path.join(UNITY_PATH, "TemporaryAssets")  # Imported assets
HIDDEN_FILES_DIR = os.path.join(UNITY_PATH, "HiddenFiles")  # Directory for fixed assets

def delete_temp_meta_files(directory):
    """Delete all .meta files in TemporaryAssets."""
    for root, _, files in os.walk(directory):
        for file in files:
            if file.endswith(".meta"):
                file_path = os.path.join(root, file)
                print(f"Deleting: {file_path}")
                os.remove(file_path)

def move_assets(temp_dir, hidden_dir):
    """Move assets from TemporaryAssets to HiddenFiles."""
    for root, dirs, files in os.walk(temp_dir):
        # Process files
        for name in files:
            src_path = os.path.join(root, name)
            rel_path = os.path.relpath(src_path, temp_dir)
            dest_path = os.path.join(hidden_dir, rel_path)

            # Ensure the destination directory exists
            os.makedirs(os.path.dirname(dest_path), exist_ok=True)

            # Move the file
            print(f"Copying file {src_path} to {dest_path}")
            shutil.copy2(src_path, dest_path)  # Copy with metadata

        # Process directories (to ensure empty directories are created)
        for name in dirs:
            src_dir = os.path.join(root, name)
            rel_path = os.path.relpath(src_dir, temp_dir)
            dest_dir = os.path.join(hidden_dir, rel_path)

            # Create the directory if it doesn't exist
            if not os.path.exists(dest_dir):
                print(f"Creating directory {dest_dir}")
                os.makedirs(dest_dir, exist_ok=True)
                
def main():
    # Step 1: Delete .meta files in TemporaryAssets
    print("Deleting existing .meta files in TemporaryAssets...")
    delete_temp_meta_files(TEMP_ASSETS_DIR)

    # Step 2: Move assets to HiddenFiles
    print("Moving assets to HiddenFiles...")
    move_assets(TEMP_ASSETS_DIR, HIDDEN_FILES_DIR)

    print("Migration complete! Verify everything before launching Unity.")

if __name__ == "__main__":
    main()
