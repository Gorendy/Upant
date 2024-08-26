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
        private static readonly Logger logger = (Logger)LogFactory.getLogger(typeof(FileUtil));

        /// <summary>
        /// 移动文件，如果目标位置不存在则创建
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="toPath"></param>
        public static void moveFile(string sourceFile, string toPath, string newFileName = null) {
            logger.debug(RCode.FILE_OPERATION, () => $"{sourceFile} will be move to {toPath}");
            logger.info(RCode.FILE_OPERATION, "file will be move");
            if (string.IsNullOrEmpty(sourceFile) || string.IsNullOrEmpty(toPath)) {
                logger.warn(RCode.WARN, "params is null when move file");
                return;
            }

            if (!File.Exists(sourceFile)) {
                logger.info(RCode.FILE_NOT_EXIST, "file not exist");
                return;
            }

            if (!Directory.Exists(toPath)) {
                Directory.CreateDirectory(toPath);
                logger.debug(RCode.FILE_OPERATION, () => $"{toPath} is not exist and be created");
            }

            try {
                File.Move(sourceFile,
                    string.IsNullOrEmpty(newFileName)
                        ? Path.Combine(toPath, Path.GetFileName(sourceFile))
                        : Path.Combine(toPath, newFileName));
            }
            catch (Exception e) {
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
        public static string findFile(string path, string fileName, bool deeplyFind = false) {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(fileName)) {
                logger.warn(RCode.WARN, "params is null when find File");
                return null;
            }

            if (!Directory.Exists(path)) {
                logger.warn(RCode.FILE_NOT_EXIST, $"{path} is not found");
                return null;
            }

            string result = null;
            if (!deeplyFind) // 不查找子目录
            {
                foreach (string file in Directory.GetFiles(path)) {
                    if (Path.GetFileName(file).Equals(fileName)) {
                        result = file;
                    }
                }

                return result;
            }

            Queue<string> queue = new Queue<string>();
            queue.Enqueue(path);
            string tmp;
            while (queue.Count > 0) {
                tmp = queue.Dequeue();
                foreach (string dir in Directory.GetDirectories(tmp)) {
                    queue.Enqueue(dir);
                }

                foreach (string file in Directory.GetFiles(tmp)) {
                    if (Path.GetFileName(file).Equals(file)) {
                        result = file;
                        queue.Clear();
                    }
                }
            }

            return result;
        }

        public static string findFileBySuffix(string path, string suffixFile) {
            if (string.IsNullOrEmpty(path)) {
                return null;
            }

            if (!Directory.Exists(path)) {
                if (File.Exists(path)) {
                    if (path.EndsWith(suffixFile)) {
                        return path;
                    }
                }

                return null;
            }

            string result = null;
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(path);
            while (queue.Count > 0) {
                string tmp = queue.Dequeue();
                foreach (var dir in Directory.GetDirectories(tmp)) {
                    queue.Enqueue(dir);
                }

                foreach (var file in Directory.GetFiles(tmp)) {
                    if (file.EndsWith(suffixFile)) {
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
        public static void deleteFile(string path, bool delOwn = true) {
            logger.debug(RCode.FILE_OPERATION, () => $"{path} will be deleted");
            if (string.IsNullOrEmpty(path)) {
                logger.info(RCode.FILE_WARN, $"{path} is null");
                return;
            }

            if (File.Exists(path)) {
                try {
                    File.Delete(path);
                }
                catch (Exception e) {
                    logger.error(RCode.FILE_ERROR_DELETE, $"{path} file deleted unsuccessfully", e);
                }

                return;
            }

            // 删除文件夹中的文件
            foreach (string file in Directory.GetFiles(path)) {
                try {
                    File.Delete(file);
                }
                catch (Exception e) {
                    logger.error(RCode.FILE_ERROR_DELETE, $"{path} file deleted unsuccessfully", e);
                }
            }

            // 递归删除文件夹
            foreach (string dir in Directory.GetDirectories(path)) {
                deleteFile(dir);
            }

            if (delOwn && Directory.GetFiles(path).Length == 0 &&
                Directory.GetDirectories(path).Length == 0) {
                try {
                    Directory.Delete(path);
                }
                catch (Exception e) {
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
        public static void UnzipFile(string filePath, string destinationDir) {
            logger.debug(RCode.FILE_OPERATION, () => $"{filePath} will be unzip");
            logger.info(RCode.FILE_OPERATION, "file will be unzip");
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(destinationDir)) {
                logger.warn(RCode.WARN, "params is null when unzip");
                return;
            }

            if (!File.Exists(filePath)) {
                logger.info(RCode.FILE_NOT_EXIST, $"{filePath} is not exist");
                return;
            }

            if (!Directory.Exists(destinationDir)) {
                logger.info(RCode.FILE_WARN, $"{destinationDir} is not exist");
                Directory.CreateDirectory(destinationDir);
                logger.debug(RCode.FILE_OPERATION, () => $"{destinationDir} be created when unzip time");
            }

            try {
                ZipFile.ExtractToDirectory(filePath, destinationDir);
            }
            catch (Exception e) {
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
        public static void UnzipFileExtractFile(string filePath, string destinationDir, string fileName) {
            logger.info(RCode.FILE_OPERATION, "file will be unzip");
            logger.debug(RCode.FILE_OPERATION, () => $"{filePath} will be unzip");
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(destinationDir)) {
                logger.warn(RCode.WARN, "params is null when unzip");
                return;
            }

            if (!File.Exists(filePath)) {
                logger.info(RCode.FILE_NOT_EXIST, $"{filePath} is not exist");
                return;
            }

            if (!Directory.Exists(destinationDir)) {
                logger.info(RCode.FILE_WARN, $"{destinationDir} is not exist");
                Directory.CreateDirectory(destinationDir);
                logger.debug(RCode.FILE_OPERATION, () => $"{destinationDir} be created when unzip time");
            }

            try {
                using (ZipArchive za = ZipFile.OpenRead(filePath)) {
                    var result = za.GetEntry(fileName);
                    if (result != null) {
                        result.ExtractToFile(Path.Combine(destinationDir, result.FullName), overwrite: true);
                    }
                    else {
                        logger.warn(RCode.FILE_NOTFOUND, $"{fileName} is not found");
                    }
                }
            }
            catch (Exception e) {
                logger.error(RCode.FILE_ERROR_UNZIP, $"{filePath} unzip unsuccessfully", e);
                return;
            }

            logger.info(RCode.FILE_OK_UNZIP);
        }

        public static void File2ByteArray(string fromFile) {
            if (!File.Exists(fromFile)) {
                return;
            }

            try {
                using (var fsr = new FileStream(fromFile, FileMode.Open, FileAccess.Read)) {
                    using (var br = new BinaryReader(fsr)) {
                        br.BaseStream.Seek(0, SeekOrigin.Begin);
                        //ByteArray2File(br.ReadBytes((int)br.BaseStream.Length), @"C:\localfile\work\log", "test.zip");
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void ByteArray2File(byte[] bytes, string filePath, string fileName, string extension) {
            if (bytes == null || bytes.Length == 0 || string.IsNullOrEmpty(filePath)) {
                return;
            }

            if (string.IsNullOrEmpty(fileName)) {
                fileName = DateTime.Today.ToString("yyyy_MM_dd") + extension;
            }
            else {
                if (!fileName.EndsWith(extension)) {
                    fileName = fileName + extension;
                }
            }

            try {
                using (var fs = new FileStream(Path.Combine(filePath, fileName), FileMode.CreateNew)) {
                    using (var bs = new BinaryWriter(fs)) {
                        bs.Write(bytes, 0, bytes.Length);
                        bs.Flush();
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }

        #region 复制文件，如果目标目录文件存在则报错

        /// <summary>
        /// 将源文件夹的内容复制到目标文件夹的源文件名称的子目录下
        /// </summary>
        /// <param name="sourceDir">源文件夹</param>
        /// <param name="distDir">目标文件夹</param>
        /// <param name="newDirName">子文件夹别名</param>
        public static void copyDirectory(string sourceDir, string distDir, string newDirName = null) {
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

            string targetDir = null, tmpName;
            tmpName = string.IsNullOrEmpty(newDirName) ? Path.GetFileName(sourceDir) : newDirName;
            targetDir = Path.Combine(distDir, tmpName);
            if (hasDirectory(targetDir)) {
                logger.debug(RCode.FILE_EXIST, () => $"{targetDir} is exist and deleted when copy time");
                try {
                    deleteFile(targetDir); // 删除已有文件夹
                    logger.debug(RCode.FILE_OPERATION, () => $"{targetDir} will be deleted when copy");
                }
                catch (Exception e) {
                    logger.error(RCode.FILE_ERROR_DELETE, $"{targetDir} deleted unsuccessfully when copy", e);
                    return;
                }
            }

            Queue<string> source = new Queue<string>();// 遍历源文件夹的子文件
            Queue<string> subDir = new Queue<string>(); // 子文件夹
            source.Enqueue(sourceDir);
            subDir.Enqueue(tmpName);
            string curDir;
            int total = 0, ernum = 0;
            while (source.Count > 0) {
                curDir = source.Dequeue();
                tmpName = subDir.Dequeue();
                foreach (string dir in Directory.GetDirectories(curDir)) {
                    subDir.Enqueue(Path.Combine(tmpName, Path.GetFileName(dir)));
                    source.Enqueue(dir);
                }

                targetDir = Path.Combine(distDir, tmpName);
                if (!hasDirectory(targetDir)) {
                    Directory.CreateDirectory(targetDir);
                }

                string tmp = null;
                try {
                    foreach (string file in Directory.GetFiles(curDir)) {
                        total++;
                        tmp = file;
                        File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));
                        logger.debug(RCode.FILE_OK_COPY, () => $"{file} copied successfully");
                    }
                }
                catch (Exception e) {
                    logger.warn(RCode.FILE_ERROR_COPY, $"{tmp} copy error", e);
                }
            }
            logger.info(RCode.FILE_OK_COPY, $"{sourceDir} successfully copy, count:{total}");
        }

        /// <summary>
        /// 将目标文件夹内容（包括文件夹）复制到目标文件夹下
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="distDir"></param>
        /// <param name="newDirName"></param>
        /// <param name="files"></param>
        /// <param name="dirs"></param>
        public static void copyDirectory(string sourceDir, string distDir, string newDirName, List<string> files, List<string> dirs) {
            if (files == null && dirs == null) {
                return;
            }
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

            string targetDir = null, tmpName;
            tmpName = string.IsNullOrEmpty(newDirName) ? Path.GetFileName(sourceDir) : newDirName;
            targetDir = Path.Combine(distDir, tmpName);
            if (hasDirectory(targetDir)) {
                logger.debug(RCode.FILE_EXIST, () => $"{targetDir} is exist and deleted when copy time");
                try {
                    deleteFile(targetDir); // 删除已有文件夹
                    logger.debug(RCode.FILE_OPERATION, () => $"{targetDir} will be deleted when copy");
                }
                catch (Exception e) {
                    logger.error(RCode.FILE_ERROR_DELETE, $"{targetDir} deleted unsuccessfully when copy", e);
                    return;
                }
            }

            Queue<string> source = new Queue<string>();// 遍历源文件夹的子文件
            Queue<string> subDir = new Queue<string>(); // 子文件夹
            source.Enqueue(sourceDir);
            subDir.Enqueue(tmpName);
            string curDir;
            int total = 0, ernum = 0;
            while (source.Count > 0) {
                curDir = source.Dequeue();
                tmpName = subDir.Dequeue();
                foreach (string dir in Directory.GetDirectories(curDir)) {
                    string fn = Path.GetFileName(dir);
                    if (dirs != null && dirs.Contains(fn)) {
                        continue;
                    }
                    subDir.Enqueue(Path.Combine(tmpName, fn));
                    source.Enqueue(dir);
                }

                targetDir = Path.Combine(distDir, tmpName);
                if (!hasDirectory(targetDir)) {
                    Directory.CreateDirectory(targetDir);
                }
                // 复制文件
                string tmp = null;
                try {
                    if (files == null || files.Count == 0) {
                        foreach (string file in Directory.GetFiles(curDir)) {// 如果不存在忽略文件
                            total++;
                            tmp = file;
                            File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));
                            logger.debug(RCode.FILE_OK_COPY, () => $"{file} copied successfully");
                        }
                    }
                    else {
                        foreach (string file in Directory.GetFiles(curDir)) {
                            string fn = Path.GetFileName(file);
                            if (files.Contains(fn)) {
                                continue;
                            }
                            total++;
                            tmp = file;
                            File.Copy(file, Path.Combine(targetDir, fn));
                            logger.debug(RCode.FILE_OK_COPY, () => $"{file} copied successfully");
                        }
                    }
                    
                }
                catch (Exception e) {
                    logger.warn(RCode.FILE_ERROR_COPY, $"{tmp} copy error", e);
                }
            }
            logger.info(RCode.FILE_OK_COPY, $"{sourceDir} successfully copy, count:{total}");
        }
        /// <summary>
        /// 将源文件夹下的内容复制到目标文件夹下，不包含在子目录中
        /// 如果文件存在则覆盖
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="distDir"></param>
        /// <param name="newDirName"></param>
        /// <param name="files"></param>
        /// <param name="dirs"></param>
        public static void copyContent2Dir(string sourceDir, string distDir, List<string> files, List<string> dirs) {
            if (files == null && dirs == null) {
                return;
            }
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

            string targetDir = distDir, tmpName = "";

            Queue<string> source = new Queue<string>();// 遍历源文件夹的子文件
            Queue<string> subDir = new Queue<string>(); // 子文件夹
            source.Enqueue(sourceDir);
            subDir.Enqueue(tmpName);
            string curDir;// 当前源目录
            int total = 0, ernum = 0;
            while (source.Count > 0) {
                curDir = source.Dequeue();
                tmpName = subDir.Dequeue();
                foreach (string dir in Directory.GetDirectories(curDir)) {
                    string fn = Path.GetFileName(dir);
                    if (dirs != null && dirs.Contains(fn)) {
                        continue;
                    }
                    subDir.Enqueue(Path.Combine(tmpName, fn));
                    source.Enqueue(dir);
                }

                targetDir = Path.Combine(distDir, tmpName);
                if (!hasDirectory(targetDir)) {
                    Directory.CreateDirectory(targetDir);
                }

                string tmp = null;
                try {
                    if (files == null || files.Count == 0) {
                        foreach (string file in Directory.GetFiles(curDir)) {
                            total++;
                            tmp = file;
                            File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)), true);
                            logger.debug(RCode.FILE_OK_COPY, () => $"{file} copied successfully");
                        }
                    }
                    else {
                        foreach (string file in Directory.GetFiles(curDir)) {
                            string fn = Path.GetFileName(file);
                            if (files.Contains(fn)) {
                                continue;
                            }
                            total++;
                            tmp = file;
                            File.Copy(file, Path.Combine(targetDir, fn), true);
                            logger.debug(RCode.FILE_OK_COPY, () => $"{file} copied successfully");
                        }
                    }
                    
                }
                catch (Exception e) {
                    logger.warn(RCode.FILE_ERROR_COPY, $"{tmp} copy error", e);
                }
            }
            logger.info(RCode.FILE_OK_COPY, $"{sourceDir} successfully copy, count:{total}");
        }
        

        /// <summary>
        /// 复制文件到文件夹中，如果目标文件夹不存在，则添加
        /// 如果存在则覆盖
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="distPath"></param>
        /// <param name="newFileName">文件夹名称</param>
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
                File.Copy(sourceFile, targetFile, true);// 如果存在，则覆盖
                logger.debug(RCode.FILE_OPERATION, () => $"{Path.GetFileName(targetFile)} successfully copy");
            }
            catch (Exception e) {
                flag = true;
                logger.error(RCode.FILE_ERROR_COPY, $"{sourceFile} copy unsuccessfully", e);
            }

            if (!flag)
                logger.info(RCode.FILE_OK_COPY, $"{targetFile} be copy completely");
        }


        #endregion

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