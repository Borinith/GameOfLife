using Microsoft.Win32;
using System;
using System.IO;

namespace Gol.Application.Utils
{
    /// <summary>
    ///     File utility methods.
    /// </summary>
    public static class FileUtils
    {
        private const string EXAMPLES_DIRECTORY_NAME = "Examples";
        private const string FILES_FILTER = "Life saves (*.life)|*.life|All files (*.*)|*.*";

        /// <summary>
        ///     Get example folder location.
        /// </summary>
        /// <returns>Return default initial directory path.</returns>
        private static string GetInitialDirectory()
        {
            var initialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var examplePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, EXAMPLES_DIRECTORY_NAME);

            if (Directory.Exists(examplePath))
            {
                initialDirectory = examplePath;
            }

            return initialDirectory;
        }

        public static bool TryGetSaveFile(out Stream? saveStream)
        {
            saveStream = null;

            var saveDialog = new SaveFileDialog
            {
                InitialDirectory = GetInitialDirectory(),
                Filter = FILES_FILTER,
                RestoreDirectory = true
            };

            var dialogResult = saveDialog.ShowDialog();

            if (dialogResult.HasValue && dialogResult.Value)
            {
                saveStream = saveDialog.OpenFile();

                return true;
            }

            return false;
        }

        public static bool TryGetOpenFile(out Stream? openStream)
        {
            openStream = null;

            var openDialog = new OpenFileDialog
            {
                InitialDirectory = GetInitialDirectory(),
                Filter = FILES_FILTER,
                RestoreDirectory = true
            };

            var dialogResult = openDialog.ShowDialog();

            if (dialogResult.HasValue && dialogResult.Value)
            {
                openStream = openDialog.OpenFile();

                return true;
            }

            return false;
        }
    }
}