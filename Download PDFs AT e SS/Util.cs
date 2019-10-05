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
        
        public static void RenameDownloadedFiles(string downloadFolderEmpresa, ICollection<string> filesToRename)
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
        }

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

        internal static void RenameLastModifiedFileInFolder(string folder, string newName)
        {
            var directory = new DirectoryInfo(folder);
            var ficheiro = directory.GetFiles()
                .OrderByDescending(f => f.LastWriteTime).First();
            File.Move(ficheiro.FullName, Path.Combine(ficheiro.DirectoryName, newName));
        }
    }
}
