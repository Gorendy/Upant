using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonM.util
{
    public class StringUtil
    {
        public static string getMD5(string str) {
            var si = new StringBuilder(32);
            using (var md = MD5.Create()) {
                var bs = md.ComputeHash(Encoding.Default.GetBytes(str));
                foreach (var b in bs) {
                    si.Append(b.ToString("x2"));
                }
            }
            return si.ToString();
        }

        public static string getFileMD5(string path) {
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) {
                return null;
            }
            var si = new StringBuilder(32);
            using (var md = MD5.Create()) {
                using (var fs = new FileStream(path, FileMode.Open)) {
                    var bs = md.ComputeHash(fs);
                    foreach (var b in bs) {
                        si.Append(b.ToString("x2"));
                    }
                }
            }
            return si.ToString();
        }
        public static bool isEmptys(params string[] list) {
            bool flag = false;
            foreach (string s in list) {
                if (string.IsNullOrEmpty(s)) {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
        public static string getParentXpath(string xpath) {
            if (string.IsNullOrEmpty(xpath)) {
                return null;
            }

            int num = getXpathNodeCount(xpath);
            if (num == 0) {
                return null;
            }

            Regex r = new Regex($"(^/?(/\\w+){{{num}}})");
            return r.Match(xpath).Groups[1].Value;
        }

        private static int getXpathNodeCount(string xpath) {
            int num = 0;
            char pre = xpath[0];
            for (var i = 1; i < xpath.Length; i++) {
                if ('/' == xpath[i] && '/' != pre) {
                    num++;
                }
                pre = xpath[i];
            }
            return num;
        }
    }
}