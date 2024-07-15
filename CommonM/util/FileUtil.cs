using System;
using System.Collections.Generic;
using System.IO;
using CommonM.logger;

namespace CommonM.util
{
    /// <summary>
    /// 文件工具类，对文件进行操作
    /// </summary>
    public class FileUtil
    {
        private static readonly ILogger logger = LogFactory.getLogger(typeof(FileUtil));

        /// <summary>
        /// 复制文件夹到文件夹，如果存在新名称，则在目标文件夹中创建对应名称的文件夹并作为目标文件夹；如果不存在则复制到目标文件中
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="distDir"></param>
        /// <param name="newDirName"></param>
        public static void copyDirectory(string sourceDir, string distDir, string newDirName) {
            if (!hasDirectory(sourceDir)) {
                logger.error(ResultUtil.error(RCode.FILE_DIR_NOTFOUND, $"{sourceDir} is not found or is a file"));
                return;
            }

            if (!hasDirectory(distDir)) {
                Directory.CreateDirectory(distDir);
                logger.debug(ResultUtil.msg(RCode.FILE_DIR_NOTFOUND,
                    $"{distDir} not exists and be created completely"));
            }

            if (string.IsNullOrEmpty(newDirName)) {
                // 直接复制
                try {
                    Directory.Move(sourceDir, distDir);
                }
                catch (Exception e) {
                    logger.error(ResultUtil.error(RCode.FILE_ERROR_COPY, $"{sourceDir} copy unsuccessfully"));
                    throw;
                }

                return;
            }

            // 将文件夹中文件，复制到新名称文件夹
            string targetDir = Path.Combine(distDir, newDirName);
            if (hasDirectory(targetDir)) {
                try {
                    deleteDirectory(targetDir); // 删除已有文件夹
                }
                catch (Exception e) {
                   logger.error(ResultUtil.error(RCode.FILE_ERROR_DELETE, 
                       $"{targetDir} deleted unsuccessfully when copy"), e);
                   goto copy;
                }
                logger.debug(ResultUtil.msg(RCode.FILE_EXIST, $"{targetDir} is exist and deleted when copy time"));
            }
            
        copy:
            Directory.CreateDirectory(targetDir);
            Queue<string> dirs = new Queue<string>();
            Queue<string> target = new Queue<string>(); // 目标文件夹中子文件夹
            dirs.Enqueue(sourceDir);
            target.Enqueue(newDirName);
            string curDir;
            string suffixDir; // 目标文件夹后缀,例如：目标文件a,suffix=b ,target = a\b
            // 对各个文件夹 使用广搜复制
            while (dirs.Count > 0) {
                curDir = dirs.Dequeue();
                targetDir = Path.Combine(sourceDir, suffixDir = target.Dequeue());// 将要把文件复制的目标文件夹
                foreach (string dir in Directory.GetFiles(curDir)) {
                    copyFile(dir, targetDir);
                }

                // 在目标文件夹中创建文件夹
                foreach (string sourDir in Directory.GetDirectories(curDir)) {
                    string fileName = Path.GetFileName(sourDir);
                    Directory.CreateDirectory(Path.Combine(targetDir, fileName));
                    dirs.Enqueue(sourDir);
                    target.Enqueue(Path.Combine(suffixDir, fileName));
                }
            }
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
            if (!hasFile(sourceFile)) {
                logger.error(ResultUtil.error(RCode.FILE_NOTFOUND,
                    $"{sourceFile} not found or is a directory"));
                return;
            }

            if (!hasDirectory(distPath)) {
                Directory.CreateDirectory(distPath);
                logger.debug(ResultUtil.msg(RCode.FILE_DIR_NOTFOUND,
                    $"{distPath} not exists and be created completely"));
            }

            string targetFile = null;
            if (string.IsNullOrEmpty(newFileName)) {
                targetFile = Path.Combine(distPath, Path.GetFileName(sourceFile));
            }
            else {
                targetFile = Path.Combine(distPath, newFileName);
            }

            try {
                if (hasFile(targetFile)) {
                    logger.debug(ResultUtil.msg(RCode.FILE_WARN,
                        $"{targetFile} is exist and will be delete"));
                    File.Delete(targetFile);
                }
                File.Copy(sourceFile, targetFile);
            }
            catch (Exception e) {
                logger.error(ResultUtil.error(RCode.FILE_ERROR_COPY, $"{sourceFile} copy unsuccessfully"), e);
            }

            logger.debug(ResultUtil.msg(RCode.FILE_OK_COPY, $"{targetFile} be copy completely"));
        }

        /// <summary>
        /// 删除文件夹内所有文件包括文件夹
        /// 采用积极删除策略，尽可能将所有文件删除，无法删除的文件将跳过
        /// 可能在删除文件夹时抛出异常
        /// </summary>
        /// <param name="absolutePath"></param>
        public static void deleteDirectory(string absolutePath) {
            if (!hasDirectory(absolutePath)) {
                logger.warn(ResultUtil.msg(RCode.FILE_WARN_NOTEXIST, $"{absolutePath} is not exist"));
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
                    logger.error(ResultUtil.error(RCode.FILE_ERROR_DELETE, curFile), e);
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

            logger.info(ResultUtil.ok(RCode.FILE_OK_DELETE,
                $"{absolutePath} deleted successfully,have [{errorNum}] files can not deleted"));
        }

        public static bool hasDirectory(string absolutePath) {
            return Directory.Exists(absolutePath);
        }

        public static bool hasFile(string absoluteFile) {
            return File.Exists(absoluteFile);
        }
    }
}