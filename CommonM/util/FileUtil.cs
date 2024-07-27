using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using CommonM.logger;

namespace CommonM.util
{
    /// <summary>
    /// 文件工具类，对文件进行操作
    /// </summary>
    public class FileUtil
    {
        private static readonly Logger logger = (Logger) LogFactory.getLogger(typeof(FileUtil));
        /// <summary>
        /// 移动文件，如果目标位置不存在则创建
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="toPath"></param>
        public static void moveFile(string sourceFile, string toPath, string newFileName = null)
        {
            logger.debug(RCode.FILE_OPERATION, () => $"{sourceFile} will be move to {toPath}");
            logger.info(RCode.FILE_OPERATION, "file will be move");
            if (string.IsNullOrEmpty(sourceFile) || string.IsNullOrEmpty(toPath))
            {
                logger.warn(RCode.WARN, "params is null when move file");
                return;
            }
            if (!File.Exists(sourceFile))
            {
                logger.info(RCode.FILE_NOT_EXIST, "file not exist");
                return;
            }
            if (!Directory.Exists(toPath))
            {
                Directory.CreateDirectory(toPath);
                logger.debug(RCode.FILE_OPERATION, () => $"{toPath} is not exist and be created");
            }

            try
            {
                File.Move(sourceFile,
                    string.IsNullOrEmpty(newFileName)
                        ? Path.Combine(toPath, Path.GetFileName(sourceFile))
                        : Path.Combine(toPath, newFileName));
            }
            catch (Exception e)
            {
                logger.error(RCode.FILE_ERROR_MOVE, $"{sourceFile} move unsuccessfully", e);
                return;
            }
            logger.info(RCode.FILE_OK_MOVE, "file move successfully");
        }

        /// <summary>
        /// 寻找文件，在当前文件夹下
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns>文件绝对路径</returns>
        public static string findFile(string path, string fileName, bool deeplyFind = false)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(fileName))
            {
                logger.warn(RCode.WARN, "params is null when find File");
                return null;
            }

            if (!Directory.Exists(path))
            {
                logger.warn(RCode.FILE_NOT_EXIST, $"{path} is not found");
                return null;
            }
            string result = null;
            if (!deeplyFind) // 不查找子目录
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    if (Path.GetFileName(file).Equals(file))
                    {
                        result = file;
                    }
                }
                return result;
            }

            Queue<string> queue = new Queue<string>();
            queue.Enqueue(path);
            string tmp;
            while (queue.Count > 0)
            {
                tmp = queue.Dequeue();
                foreach (string dir in Directory.GetDirectories(tmp))
                {
                    queue.Enqueue(dir);
                }

                foreach (string file in Directory.GetFiles(tmp))
                {
                    if (Path.GetFileName(file).Equals(file))
                    {
                        result = file;
                        queue.Clear();
                    }
                }
            }
            return result;
        }
        public static string findFileBySuffix(string path, string suffixFile)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            if (!Directory.Exists(path))
            {
                if (File.Exists(path))
                {
                    if (path.EndsWith(suffixFile))
                    {
                        return path;
                    }
                }
                return null;
            }
            string result = null;
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(path);
            while (queue.Count > 0)
            {
                string tmp = queue.Dequeue();
                foreach (var dir in Directory.GetDirectories(tmp))
                {
                    queue.Enqueue(dir);
                }
                foreach (var file in Directory.GetFiles(tmp))
                {
                    if (file.EndsWith(suffixFile))
                    {
                        result = file;
                        queue.Clear();
                        break;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// 递归删除文件夹内容并包括文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="delOwn">是否删除当前文件夹</param>
        public static void deleteFile(string path, bool delOwn = true)
        {
            logger.debug(RCode.FILE_OPERATION, () => $"{path} will be deleted");
            if (string.IsNullOrEmpty(path))
            {
                logger.info(RCode.FILE_WARN, $"{path} is null");
                return;
            }
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                } catch(Exception e)
                {
                    logger.error(RCode.FILE_ERROR_DELETE, $"{path} file deleted unsuccessfully", e);
                }

                return;
            }
            // 删除文件夹中的文件
            foreach (string file in Directory.GetFiles(path))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception e)
                {
                    logger.error(RCode.FILE_ERROR_DELETE, $"{path} file deleted unsuccessfully", e);
                }
            }
            // 递归删除文件夹
            foreach (string dir in Directory.GetDirectories(path))
            {
                deleteFile(dir);
            }

            if (delOwn && Directory.GetFiles(path).Length == 0 && 
                Directory.GetDirectories(path).Length == 0)
            {
                try
                {
                    Directory.Delete(path);
                }
                catch (Exception e)
                {
                    logger.error(RCode.FILE_ERROR_DELETE, $"{path} directory deleted unsuccessfully", e);
                    return;
                }
                logger.debug(RCode.FILE_OPERATION, () => $"{path} delete successfully");
            }
        }
        /// <summary>
        /// 解压缩文件zip格式
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="destinationDir">目标文件夹</param>
        public static void UnzipFile(string filePath, string destinationDir)
        {
            logger.debug(RCode.FILE_OPERATION, () => $"{filePath} will be unzip");
            logger.info(RCode.FILE_OPERATION, "file will be unzip");
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(destinationDir))
            {
                logger.warn(RCode.WARN, "params is null when unzip");
                return;
            }
            if (!File.Exists(filePath))
            {
                logger.info(RCode.FILE_NOT_EXIST,$"{filePath} is not exist");
                return;
            }
            if (!Directory.Exists(destinationDir))
            {
                logger.info(RCode.FILE_WARN,$"{destinationDir} is not exist");
                Directory.CreateDirectory(destinationDir);
                logger.debug(RCode.FILE_OPERATION, () => $"{destinationDir} be created when unzip time");
            }

            try
            {
                ZipFile.ExtractToDirectory(filePath, destinationDir);
            }
            catch (Exception e)
            {
                logger.error(RCode.FILE_ERROR_UNZIP, $"{filePath} unzip unsuccessfully", e);
                return;
            }
            logger.info(RCode.FILE_OK_UNZIP);
        }
        /// <summary>
        /// 解压缩特定的文件名在压缩包中（只能是压缩包子目录下否则找不到）
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="destinationDir"></param>
        /// <param name="fileName"></param>
        public static void UnzipFileExtractFile(string filePath, string destinationDir, string fileName)
        {
            logger.info(RCode.FILE_OPERATION, "file will be unzip");
            logger.debug(RCode.FILE_OPERATION, () => $"{filePath} will be unzip");
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(destinationDir))
            {
                logger.warn(RCode.WARN, "params is null when unzip");
                return;
            }
            if (!File.Exists(filePath))
            {
                logger.info(RCode.FILE_NOT_EXIST,$"{filePath} is not exist");
                return;
            }
            if (!Directory.Exists(destinationDir))
            {
                logger.info(RCode.FILE_WARN,$"{destinationDir} is not exist");
                Directory.CreateDirectory(destinationDir);
                logger.debug(RCode.FILE_OPERATION, () => $"{destinationDir} be created when unzip time");
            }
            try
            {
                using (ZipArchive za = ZipFile.OpenRead(filePath))
                {
                    var result = za.GetEntry(fileName);
                    if (result != null)
                    {
                        result.ExtractToFile(Path.Combine(destinationDir, result.FullName), overwrite: true);
                    }
                    else
                    {
                        logger.warn(RCode.FILE_NOTFOUND,$"{fileName} is not found");
                    }
                }
            }
            catch (Exception e)
            {
                logger.error(RCode.FILE_ERROR_UNZIP, $"{filePath} unzip unsuccessfully", e);
                return;
            }
            logger.info(RCode.FILE_OK_UNZIP);
        }
        
        public static void File2ByteArray(string fromFile)
        {
            if (!File.Exists(fromFile))
            {
                return;
            }
            try
            {

                using (var fsr = new FileStream(fromFile, FileMode.Open, FileAccess.Read))
                {
                    using (var br = new BinaryReader(fsr))
                    {
                        br.BaseStream.Seek(0, SeekOrigin.Begin);
                        //ByteArray2File(br.ReadBytes((int)br.BaseStream.Length), @"C:\localfile\work\log", "test.zip");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public static void ByteArray2File(byte[] bytes, string filePath, string fileName, string extension)
        {
            if (bytes == null || bytes.Length == 0 || string.IsNullOrEmpty(filePath))
            {
                return;
            }
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = DateTime.Today.ToString("yyyy_MM_dd") + extension;
            }
            else
            {
                if (!fileName.EndsWith(extension))
                {
                    fileName = fileName + extension;
                }
            }
            try
            {
                using (var fs = new FileStream(Path.Combine(filePath, fileName), FileMode.CreateNew))
                {
                    using (var bs = new BinaryWriter(fs))
                    {
                        bs.Write(bytes, 0, bytes.Length);
                        bs.Flush();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        /// <summary>
        /// 复制文件夹到文件夹，如果存在新名称，则在目标文件夹中创建对应名称的文件夹并作为目标文件夹；如果不存在则复制到目标文件中
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="distDir"></param>
        /// <param name="newDirName"></param>
        public static void copyDirectory(string sourceDir, string distDir, string newDirName) {
            logger.info(RCode.FILE_OPERATION, "directory will be copy");
            logger.debug(RCode.FILE_OPERATION, () => $"{sourceDir} will be copy");
            if (!hasDirectory(sourceDir)) {
                logger.warn(RCode.FILE_DIR_NOTFOUND, $"{sourceDir} is not found or is a file");
                return;
            }

            if (!hasDirectory(distDir)) {
                Directory.CreateDirectory(distDir);
                logger.debug(RCode.FILE_DIR_NOTFOUND, () => $"{distDir} not exists and be created completely");
            }
            string targetDir;
            if (string.IsNullOrEmpty(newDirName)) {
                targetDir = Path.Combine(distDir, Path.GetFileName(sourceDir));
            } else
            {
                // 将文件夹中文件，复制到新名称文件夹
                targetDir = Path.Combine(distDir, newDirName);
            }

            if (hasDirectory(targetDir)) {
                logger.debug(RCode.FILE_EXIST, () => $"{targetDir} is exist and deleted when copy time");
                try {
                    deleteFile(targetDir, false); // 删除已有文件夹
                    logger.debug(RCode.FILE_OPERATION, () => $"{targetDir} will be delted when copy");
                }
                catch (Exception e) {
                   logger.error(RCode.FILE_ERROR_DELETE, $"{targetDir} deleted unsuccessfully when copy", e);
                    return;
                }
            }
            
            Queue<string> dirs = new Queue<string>();
            Queue<string> target = new Queue<string>(); // 目标文件夹中子文件夹
            dirs.Enqueue(sourceDir);
            target.Enqueue(newDirName);
            string curDir;
            string suffixDir; // 目标文件夹后缀路径,例如：目标文件a,suffix=b ,target = a\b
            // 对各个文件夹 使用广搜复制
            int total = 0, ernum = 0;
            while (dirs.Count > 0) {
                curDir = dirs.Dequeue();
                targetDir = Path.Combine(sourceDir, suffixDir = target.Dequeue());// 将要把文件复制的目标文件夹
                foreach (string dir in Directory.GetFiles(curDir)) {
                    total++;
                    try
                    {
                        File.Copy(dir, targetDir);
                    } catch (Exception e)
                    {
                        logger.warn(RCode.FILE_ERROR_COPY, $"{dir} error");
                        ernum++;
                    }
                }

                // 在目标文件夹中创建文件夹
                foreach (string sourDir in Directory.GetDirectories(curDir)) {
                    string fileName = Path.GetFileName(sourDir);
                    Directory.CreateDirectory(Path.Combine(targetDir, fileName));
                    dirs.Enqueue(sourDir);
                    target.Enqueue(Path.Combine(suffixDir, fileName));
                }
            }
            logger.info(RCode.FILE_OK_COPY, $"{sourceDir} successfully copy, count:{total}, error{ernum}");
        }

        public static void copyFile(string sourceFile, string distPath) {
            copyFile(sourceFile, distPath, null);
        }

        /// <summary>
        /// 复制文件到文件夹中，如果目标文件夹不存在，则添加
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="distPath"></param>
        /// <param name="newFileName">复制后文件名称，没有按照源文件创建</param>
        public static void copyFile(string sourceFile, string distPath, string newFileName) {
            logger.debug(RCode.FILE_OPERATION, () => $"{sourceFile} will be copied");
            if (!hasFile(sourceFile)) {
                logger.warn(RCode.FILE_NOTFOUND,
                    $"{sourceFile} not found or is a directory");
                return;
            }

            if (!hasDirectory(distPath)) {
                Directory.CreateDirectory(distPath);
                logger.debug(RCode.FILE_DIR_NOTFOUND, () => $"{distPath} not exists and be created completely");
            }

            string targetFile = null;
            if (string.IsNullOrEmpty(newFileName)) {
                targetFile = Path.Combine(distPath, Path.GetFileName(sourceFile));
            }
            else {
                targetFile = Path.Combine(distPath, newFileName);
            }
            bool flag = false;
            try {
                if (hasFile(targetFile)) {
                    logger.debug(RCode.FILE_WARN, () => $"{targetFile} is exist and will be delete");
                    File.Delete(targetFile);
                }
                File.Copy(sourceFile, targetFile);
                logger.debug(RCode.FILE_OPERATION, () => $"{Path.GetFileName(targetFile)} successfully copy");
            }
            catch (Exception e) {
                flag = true;
                logger.error(RCode.FILE_ERROR_COPY, $"{sourceFile} copy unsuccessfully", e);
            }

            if (!flag)
                logger.info(RCode.FILE_OK_COPY, $"{targetFile} be copy completely");
        }

        /// <summary>
        /// 删除文件夹内所有文件包括文件夹
        /// 采用积极删除策略，尽可能将所有文件删除，无法删除的文件将跳过
        /// 可能在删除文件夹时抛出异常
        /// </summary>
        /// <param name="absolutePath"></param>
        public static void deleteDirectory(string absolutePath) {
            if (!hasDirectory(absolutePath)) {
                logger.warn(RCode.FILE_WARN_NOTEXIST, $"{absolutePath} is not exist");
                return;
            }

            Queue<string> queue = new Queue<string>();
            queue.Enqueue(absolutePath);
            string curDir = null;
            int count = 1; // statistics dirs num
            int errorNum = 0; // statistics deleted file unsuccessfully
            while (queue.Count > 0) {
                curDir = queue.Dequeue();

                string curFile = null;
                try {
                    foreach (string file in Directory.GetFiles(absolutePath)) {
                        curFile = file;
                        File.Delete(file);
                    }
                }
                catch (Exception e) {
                    errorNum++;
                    logger.error(RCode.FILE_ERROR_DELETE, curFile, e);
                }

                foreach (string dir in Directory.GetDirectories(curDir)) {
                    queue.Enqueue(dir);
                    count++;
                }
            }

            // 将空文件夹删除
            Stack<string> delDirs = new Stack<string>(count);
            delDirs.Push(absolutePath);
            bool delDir;
            while (delDirs.Count > 0) {
                curDir = delDirs.Peek();
                if (Directory.GetFiles(curDir).Length > 0) {
                    delDirs.Pop();
                }

                delDir = true;
                foreach (string dir in Directory.GetDirectories(curDir)) {
                    if (delDir) {
                        delDir = false;
                    }

                    delDirs.Push(dir);
                }

                if (delDir) {
                    delDirs.Pop();
                    Directory.Delete(curDir);
                }
            }

            logger.info(RCode.FILE_OK_DELETE,
                $"{absolutePath} deleted successfully,have [{errorNum}] files can not deleted");
        }

        public static bool hasDirectory(string absolutePath) {
            return Directory.Exists(absolutePath);
        }

        public static bool hasFile(string absoluteFile) {
            return File.Exists(absoluteFile);
        }
    }
}