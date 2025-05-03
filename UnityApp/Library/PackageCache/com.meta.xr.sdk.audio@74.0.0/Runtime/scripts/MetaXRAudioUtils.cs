/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

/************************************************************************************
 * Filename    :   MetaXRAudioUtils.cs
 * Content     :   Miscellaneous utility functions
 ***********************************************************************************/

using System.IO;
using System.Linq;

/// \brief Class to provide miscellaneous utility functions
internal class MetaXRAudioUtils
{
    /// \brief Returns the case-sensitive path for a file
    internal static string GetCaseSensitivePathForFile(string path)
    {
        if (!File.Exists(path))
        {
            return path;
        }

        string csPath = Path.GetPathRoot(path);
        foreach (var name in path.Substring(csPath.Length).Split(Path.DirectorySeparatorChar))
        {
            csPath = Directory.EnumerateFileSystemEntries(csPath, name).First();
        }

        return csPath;
    }

    internal static void CreateDirectoryForFilePath(string absPath)
    {
        int directoriesEnd = System.Math.Max(absPath.LastIndexOf('/'), absPath.LastIndexOf('\\'));
        if (directoriesEnd >= 0)
        {
            string directoryPath = absPath.Substring(0, directoriesEnd);
            if (!System.IO.Directory.Exists(directoryPath))
            {
                System.IO.Directory.CreateDirectory(directoryPath);
            }
        }
    }
}
