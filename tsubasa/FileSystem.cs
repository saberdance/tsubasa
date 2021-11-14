using System;
using System.Linq;
using System.Text;
using System.IO.Compression;

namespace tsubasa
{
    public static class FileSystem
    {
        public static void DeleteFile(string path)
        {
            if (File.Exists(path) is false)
            {
                return;
            }
            if (File.GetAttributes(path) is FileAttributes.Directory)
            {
                Directory.Delete(path, true);
            }
            else
            {
                File.Delete(path);
            }
        }
        public static List<string> GetFilesWithExtension(string path,string ext,bool searchSub=true)
        {
            string[] fileList = null;
            if (searchSub==true)
            {
                fileList = Directory.GetFiles(path, "*." + ext, SearchOption.AllDirectories);
            }
            else
            {
                fileList = Directory.GetFiles(path, "*." + ext, SearchOption.TopDirectoryOnly);
            }
            
            List<string> files = new List<string>(fileList);
            return files;
        }

        public static List<string> GetFilesContainsName(string path, string pattern)
        {
            return (from file in Directory.GetFiles(path)
                    where file.IndexOf(pattern) is not -1
                    select file).ToList();
        }
        public static void Zip(String srcFolder, String targetPath)
        {

            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }
            ZipFile.CreateFromDirectory(srcFolder, targetPath, 0, false, System.Text.Encoding.Default);
        }
        public static void ZipOneFile(String srcFile, string targetPath)
        {
            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }
            FileStream zipToOpen = new FileStream(targetPath, FileMode.Create);
            ZipArchive zipArchive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);
            //Logger.Log(Path.GetFileNameWithoutExtension(srcFile));
            ZipArchiveEntry targetEntry = zipArchive.CreateEntryFromFile(srcFile, Path.GetFileName(srcFile));
            zipArchive.Dispose();
            zipToOpen.Dispose();
        }

        public static void Unzip(String SrcFile, String targetPath)
        {
            if (Directory.Exists(targetPath))
            {
                Directory.Delete(targetPath, true);
            }
            ZipFile.ExtractToDirectory(SrcFile, targetPath);
        }
        public static void ZipFiles(List<String> files, String targetPath,bool entryWithPath=true)
        {
            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }
            using (FileStream zipToOpen = new FileStream(targetPath, FileMode.Create))
            {
                zipToOpen.Close();
                using (ZipArchive archive = ZipFile.Open(targetPath, ZipArchiveMode.Update,Encoding.UTF8))
                {
                    foreach (string oneFile in files)
                    {

                        // archive.CreateEntryFromFile(oneFile, Path.GetFileName(oneFile));
                        string entryName = null;
                        if (entryWithPath==true)
                        {
                            entryName = oneFile;
                        }
                        else
                        {
                            entryName = Path.GetFileName(oneFile);
                        }
                        archive.CreateEntryFromFile(oneFile, entryName);
                    }        
                }
            }
        }

        /// <summary>
        /// Copy all files and folders from sourceDirectory to targetDirectory
        /// Exist files will be overwrite
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <param name="targetDirectory"></param>
        /// <returns>CopySuccess</returns>
        public static bool CopyDirectory(string sourceDirectory, string targetDirectory)
        {
            try
            {
                DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
                DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
                CopyDirectoryFiles(diSource, diTarget);
                return true;
            }
            catch (Exception e)
            {
                Logger.Log("Copy Error:" + e.ToString());
                return false;
            }

        }

        public static void CopyDirectoryFiles(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);
            foreach (FileInfo fi in source.GetFiles())
            {
                Logger.Log($"Copy file:{ fi.FullName } to { Path.Combine(target.FullName, fi.Name) }");
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyDirectoryFiles(diSourceSubDir, nextTargetSubDir);
            }
        }
        public static void DeleteAllFileWithExtension(string filePath, string extention)
        {
            try
            {
                if (Directory.Exists(filePath))
                {
                    DirectoryInfo dir = new DirectoryInfo(filePath);
                    FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                    foreach (FileSystemInfo i in fileinfo)
                    {
                        if (i is DirectoryInfo)            //判断是否文件夹
                        {
                            continue;
                        }
                        else
                        {
                            if (i.Extension == extention)
                                File.Delete(i.FullName);      //删除指定文件
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }
        }
        public static List<string> ResolveLogFiles(List<string> logFiles)
        {
            List<string> results = new List<string>();
            try
            {
                foreach (string logFile in logFiles)
                {
                    File.Copy(logFile, logFile + "_copy", true);
                    results.Add(logFile + "_copy");
                }
            }
            catch
            {
            }
            return results;
        }
    }


}
