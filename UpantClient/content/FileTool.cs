using System.Collections.Generic;
using System.IO;
using System.Windows.Shapes;
using CommonM.logger;
using CommonM.util;
using Path = System.IO.Path;

namespace UpantClient.content
{
    /// <summary>
    ///  按照功能区分,文件处理
    /// </summary>
    public class FileTool
    {
        public static ILogger logger = LogFactory.getLogger();
        public const string backupPath = "bk";

        public const string unzipPath = "tmp";
        // 解压
        public static void unzip(string file) {
            FileUtil.UnzipFile(file, Path.Combine(DataContext.config.setting.localPath, unzipPath));
        }
        // 备份
        public static void backup() {
            var setting = DataContext.config.setting;
            var ignores = setting.backupIgnores;
            logger.info(RCode.FILE_INFO_COPY, "文件即将备份");
            if (ignores == null || ignores.directories == null) {
                FileUtil.copyDirectory(setting.localPath, setting.localPath, backupPath, null, new List<string>() { backupPath });
            }
            else {
                ignores.directories.Add(backupPath);
                FileUtil.copyDirectory(setting.localPath, setting.localPath, backupPath, ignores.files, ignores.directories);
            }
            logger.info(RCode.FILE_OK_COPY);
        }
        // 修改
    }
}