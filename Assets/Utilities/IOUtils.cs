namespace Game.Core.Utilities
{
	using Path = System.IO.Path;
	using Directory = System.IO.Directory;
	using FileInfo = System.IO.FileInfo;
	using DirectoryInfo = System.IO.DirectoryInfo;
	using System.Collections.Generic;

	public static class IOUtils
	{
		public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
		{
			// Get information about the source directory
			var dir = new DirectoryInfo(sourceDir);

			// Check if the source directory exists
			if (!dir.Exists)
			{
				throw new System.IO.DirectoryNotFoundException("[CopyDirectory] Source directory not found:" + dir.FullName);
			}

			// Cache directories before we start copying
			DirectoryInfo[] dirs = dir.GetDirectories();

			// Create the destination directory
			Directory.CreateDirectory(destinationDir);

			// Get the files in the source directory and copy to the destination directory
			foreach (FileInfo file in dir.GetFiles())
			{
				string targetFilePath = Path.Combine(destinationDir, file.Name);
				string extension = Path.GetExtension(targetFilePath);
				if (extension != ".DS_Store")
				{
					file.CopyTo(targetFilePath, true);
				}
			}

			// If recursive and copying subdirectories, recursively call this method
			if (recursive)
			{
				foreach (DirectoryInfo subDir in dirs)
				{
					string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
					CopyDirectory(subDir.FullName, newDestinationDir, true);
				}
			}
		}

		public static Dictionary<string, IEnumerable<string>> GetFiles(string rootFolder, bool recursive)
		{
			Dictionary<string, IEnumerable<string>> filesByFolder = new Dictionary<string, IEnumerable<string>>();
			GetFiles(rootFolder, recursive, in filesByFolder);
			return filesByFolder;
		}

		public static void GetFiles(string rootFolder, bool recursive, in Dictionary<string, IEnumerable<string>> filesByFolder)
		{
			if (Directory.Exists(rootFolder))
			{
				IEnumerable<string> files = Directory.EnumerateFiles(rootFolder);
				filesByFolder.Add(rootFolder, files);

				if (recursive)
				{
					IEnumerable<string> childrenFolder = Directory.EnumerateDirectories(rootFolder);
					IEnumerator<string> folderIt = childrenFolder.GetEnumerator();
					while (folderIt.MoveNext())
					{
						GetFiles(folderIt.Current, recursive, filesByFolder);
					}
				}
			}
		}
	}
}
