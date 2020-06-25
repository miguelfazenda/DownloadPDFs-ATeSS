using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Download_PDFs_AT_e_SS
{
    class Util
    {
        public static void CopyAll<T>(T source, T target)
        {
            var type = typeof(T);
            foreach (var sourceProperty in type.GetProperties())
            {
                var targetProperty = type.GetProperty(sourceProperty.Name);
                targetProperty.SetValue(target, sourceProperty.GetValue(source, null), null);
            }
            foreach (var sourceField in type.GetFields())
            {
                var targetField = type.GetField(sourceField.Name);
                targetField.SetValue(target, sourceField.GetValue(source));
            }
        }

        internal static string GetVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }

        internal static void WaitForAllFilesToDownload(string downloadFolderEmpresa)
        {
            while(true)
            {
                bool foundFileDownloading = false;
                foreach (string file in Directory.GetFiles(downloadFolderEmpresa))
                {
                    if(file.ToLower().EndsWith("crdownload"))
                    {
                        foundFileDownloading = true;
                        break;
                    }
                }

                if(!foundFileDownloading)
                {
                    return;
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        internal static Declaracao[] MergeArrays(params Declaracao[][] declaracoesArray)
        {
            int totalLen = 0;

            //Aloca espaço para a array total
            foreach (Declaracao[] declacacoes in declaracoesArray)
                totalLen += declacacoes.Length;
            Declaracao[] mergedArray = new Declaracao[totalLen];

            int pos = 0;

            foreach (Declaracao[] declacacoes in declaracoesArray)
            {
                Array.Copy(declacacoes, 0, mergedArray, pos, declacacoes.Length);
                pos += declacacoes.Length;
            }

            return mergedArray;
        }

        /*public static void RenameDownloadedFiles(string downloadFolderEmpresa, ICollection<string> filesToRename)
        {
            var directory = new DirectoryInfo(downloadFolderEmpresa);
            var ficheiros = directory.GetFiles()
                .OrderByDescending(f => f.LastWriteTime);
            var enumeratorFicheiros = ficheiros.GetEnumerator();

            foreach (string novoNomeFicheiro in filesToRename)
            {
                if (!enumeratorFicheiros.MoveNext())
                    break;

                var ficheiro = enumeratorFicheiros.Current;

                if (novoNomeFicheiro != null) //Se for null é porque é para manter o nome
                {
                    try
                    {
                        //Gera o nome (acrescenta um numero à frente se já existir)
                        int newNameTries = 0;
                        var fileName = novoNomeFicheiro;
                        while (File.Exists(Path.Combine(ficheiro.DirectoryName, fileName)))
                        {
                            newNameTries++;
                            fileName = Path.GetFileNameWithoutExtension(novoNomeFicheiro)
                                + " (" + newNameTries + ")" + Path.GetExtension(novoNomeFicheiro);
                        }

                        //Renomeia o ficheiro
                        File.Move(ficheiro.FullName, Path.Combine(ficheiro.DirectoryName, fileName));
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }*/

        internal static void WaitForFileCountToBeGreaterThan(string downloadFolderEmpresa, int fileCount)
        {
            while (true)
            {
                if (Directory.GetFiles(downloadFolderEmpresa).Length > fileCount)
                {
                    return;
                }
                else
                {
                    Thread.Sleep(300);
                }
            }
        }

        internal static void RenameLastModifiedFileInFolder(string folder, string newName, string diretorio)
        {
            var directory = new DirectoryInfo(folder);
            var ficheiro = directory.GetFiles()
                .OrderByDescending(f => f.LastWriteTime).First();

            //Muda o ficheiro de nome. Para isso verifica se um ficheiro com o mesmo nome já existe. Caso afirmativo, tenta acrescentar um (1) no nome do ficheiro

            if (newName == null)
                newName = ficheiro.FullName;

            //Gera o nome (acrescenta um numero à frente se já existir)
            int newNameTries = 0;
            var fileName = newName;
            while (File.Exists(Path.Combine(diretorio, fileName)))
            {
                newNameTries++;
                fileName = Path.GetFileNameWithoutExtension(newName)
                    + " (" + newNameTries + ")" + Path.GetExtension(newName);
            }

            var ficheiroNovoPath = Path.Combine(diretorio, fileName);

            //Move(e renomeia) o ficheiro, criando primeiro a pasta caso nao exista
            new System.IO.FileInfo(ficheiroNovoPath).Directory.Create();
            File.Move(ficheiro.FullName, ficheiroNovoPath);
        }

        /**
         * Devolve se um elemento está presente ou não
         */
        public static bool IsElementPresent(IWebDriver driver, By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        /**
         * Devolve se um elemento está presente ou não
         */
        public static bool IsElementPresentWaitAWhile(IWebDriver driver, By by)
        {
            int NUM_TRIES = 4;
            for (int i = 0; i < NUM_TRIES; i++)
            {
                //Tenta encontrar o elemento, se não encontrar repete, e espera um segundo
                if(i > 0)
                    Thread.Sleep(1000);

                try
                {
                    driver.FindElement(by);
                    return true;
                }
                catch (NoSuchElementException)
                {
                }
            }
            return false;
        }

        public static string ByteArrayToHexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        public static byte[] HexStringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
