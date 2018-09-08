using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseBackup
{
    class ZipClass
    {

        public void ZipFile(string FileToZip, string ZipedFile, int CompressionLevel, int BlockSize)
        {
            if (!System.IO.File.Exists(FileToZip))
            {
                throw new System.IO.FileNotFoundException("The specified file " + FileToZip + " could not be found. Zipping aborderd");
            }

            System.IO.FileStream StreamToZip = new System.IO.FileStream(FileToZip, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.FileStream ZipFile = System.IO.File.Create(ZipedFile);
            ZipOutputStream ZipStream = new ZipOutputStream(ZipFile);
            ZipEntry ZipEntry = new ZipEntry("ZippedFile");
            ZipStream.PutNextEntry(ZipEntry);
            ZipStream.SetLevel(CompressionLevel);
            byte[] buffer = new byte[BlockSize];
            System.Int32 size = StreamToZip.Read(buffer, 0, buffer.Length);
            ZipStream.Write(buffer, 0, size);
            try
            {
                while (size < StreamToZip.Length)
                {
                    int sizeRead = StreamToZip.Read(buffer, 0, buffer.Length);
                    ZipStream.Write(buffer, 0, sizeRead);
                    size += sizeRead;
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            ZipStream.Finish();
            ZipStream.Close();
            StreamToZip.Close();
        }
        /// <summary>
        /// 文件加密压缩
        /// </summary>
        /// <param name="FileToZip">需要压缩的文件路径</param>
        /// <param name="ZipedFile">压缩包路径（压缩包文件类型看自己需求）</param>
        /// <param name="password">加密密码</param>
        public static void ZipFileMain(string FileToZip, string ZipedFile, string password)
        {
            ZipOutputStream s = new ZipOutputStream(File.Create(ZipedFile));

            s.SetLevel(6); // 0 - store only to 9 - means best compression

            s.Password = md5.encrypt(password);

            //打开压缩文件 
            FileStream fs = File.OpenRead(FileToZip);

            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);

            Array arr = FileToZip.Split('\\');
            string le = arr.GetValue(arr.Length - 1).ToString();
            ZipEntry entry = new ZipEntry(le);
            entry.DateTime = DateTime.Now;
            entry.Size = fs.Length;
            fs.Close();
            s.PutNextEntry(entry);
            s.Write(buffer, 0, buffer.Length);
            s.Finish();
            s.Close();
        }

    }
    class UnZipClass
    {
        public void UnZip(string directoryName, string ZipedFile, string password)
        {
            using (FileStream fileStreamIn = new FileStream(ZipedFile, FileMode.Open, FileAccess.Read))
            {
                using (ZipInputStream zipInStream = new ZipInputStream(fileStreamIn))
                {
                    zipInStream.Password = md5.encrypt(password);
                    ZipEntry entry = zipInStream.GetNextEntry();
                    do
                    {
                        using (FileStream fileStreamOut = new FileStream(directoryName + @"\" + entry.Name, FileMode.Create, FileAccess.Write))
                        {

                            int size = 2048;
                            byte[] buffer = new byte[2048];
                            do
                            {
                                size = zipInStream.Read(buffer, 0, buffer.Length);
                                fileStreamOut.Write(buffer, 0, size);
                            } while (size > 0);
                        }
                    } while ((entry = zipInStream.GetNextEntry()) != null);
                }
            }
        }
    }
    class md5
    {
        #region "MD5加密"
        /// <summary>
        ///32位 MD5加密
        /// </summary>
        /// <param name="str">加密字符</param>
        /// <returns></returns>
        public static string encrypt(string str)
        {
            string cl = str;
            string pwd = "";
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            for (int i = 0; i < s.Length; i++)
            {
                pwd = pwd + s[i].ToString("X");
            }
            return pwd;
        }
        #endregion
    }
}
