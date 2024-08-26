using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.FtpClient;
using CommonM.logger;
using Logger = CommonM.logger.Logger;

namespace CommonM.util
{
    /// <summary>
    /// ftp文件工具类
    /// </summary>
    public class FTPUtil
    {
        private static readonly Logger logger = (Logger)LogFactory.getLogger(typeof(FTPUtil));
        private const int defaultBuffer = 1024 * 128;
        /// <summary>
        /// 判断是否连接
        /// </summary>
        /// <param name="serverIp"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool isConnected(string serverIp, string username, string password) {
            if (StringUtil.isEmptys(serverIp, username, password)) {
                logger.warn(RCode.FTP_WARN, $"param is null, server ip='{serverIp}', username='{username}',password='{password}'");
                return false;
            }

            using (FtpClient ftp = new FtpClient())
            {
                ftp.Host = serverIp;
                ftp.Credentials = new NetworkCredential(username, password);
                ftp.Connect();
                return ftp.IsConnected;
            }
        }

        /// <summary>
        /// 从远程服务器中获取文件信息
        /// </summary>
        /// <param name="serverIp">远程服务ip</param>
        /// <param name="username">登录用户名</param>
        /// <param name="password">密码</param>
        /// <param name="serverPath">服务器路径，例子："/Serverpath/"</param>
        /// <param name="localPath">本地保存路径</param>
        /// <param name="fileType">下载文件类型，例如，.zip</param>
        public bool downloadFileWithFTP(string serverIp, string username, string password, string serverPath, 
            string localPath, string fileType) {
            if (StringUtil.isEmptys(serverIp, username, password, serverPath, localPath)) {
                logger.warn(RCode.FTP_WARN, $"param is null, server ip='{serverIp}', username='{username}'," +
                                            $"password='{password}', server path='{serverPath}', local path='{localPath}'.");
                return false;
            }
            logger.info(RCode.FTP_INFO, $"downloading file from ftp server:'{serverIp}', server path:'{serverPath}'");
            // 连接远程ftp服务端
            FtpClient ftp;
            try {
                ftp = new FtpClient();
                ftp.Host = serverIp;
                ftp.Credentials = new NetworkCredential(username, password);
                ftp.Connect();
            }
            catch (Exception e) {
                logger.error(RCode.FTP_ERROR_CONNECT, $"{serverIp} connected error, account:'{username}'='{password}'", e);
                return false;
            }

            if (!Directory.Exists(localPath)) {
                logger.warn(RCode.FTP_WARN, $"{localPath} is not exist!");
                return false;
            }

            List<string> downloadFiles = new List<string>();
            // 遍历服务器文件夹列表
            foreach (FtpListItem item in ftp.GetListing(serverPath, FtpListOption.Modify | FtpListOption.Size)) {
                if (!Path.GetExtension(item.Name).Equals(fileType)) {
                    continue;
                }
                string localFile = Path.Combine(localPath, item.Name);
                try {
                    // 尝试下载文件
                    using (Stream stream = ftp.OpenRead(item.FullName))
                    using (FileStream fs = new FileStream(localFile, FileMode.Create)) {
                        int count = 0;
                        byte[] buffer = new byte[defaultBuffer];
                        while ((count = stream.Read(buffer, 0, buffer.Length)) > 0) {
                            fs.Write(buffer, 0, count);
                        }
                        fs.Flush();
                    }
                    downloadFiles.Add(item.Name);
                }
                catch (Exception e) {
                    logger.error(RCode.FTP_ERROR_DOWNLOAD, $"FTP download file[{item.Name}] error", e);
                }
            }
            // 校验文件是否下载完成
            if (downloadFiles.Count == 0) {
                logger.warn(RCode.FTP_WARN, $"there is nothing in directory[{serverPath}]");
                return false;
            }

            string[] files = Directory.GetFiles(localPath, '*' + fileType);
            int fileNumber = 0;
            foreach (string file in files) {
                string name = Path.GetFileName(file);
                if (downloadFiles.Contains(name)) {
                    fileNumber++;
                }
            }
            logger.warn(RCode.FTP_WARN,"download file number is wrong!");
            return fileNumber == downloadFiles.Count;
        }
    }
}