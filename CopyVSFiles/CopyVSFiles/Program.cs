using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE100;
using EnvDTE;
using System.Threading;
using System.IO;

namespace CopyVSFiles
{
    class Program
    {
        static DTE m_dte=null;
        static void Main(string[] args)
        {
            Type visualStudioType = Type.GetTypeFromProgID("VisualStudio.DTE.9.0");
            m_dte = Activator.CreateInstance(visualStudioType) as DTE;
            m_dte.MainWindow.Visible = true;
            if(args.Count()!=2)
            {
                Console.WriteLine("Usage: {0} <Source path> <Dst Path>", System.AppDomain.CurrentDomain.FriendlyName);
                return;
            }
            DirectoryInfo rootDir = new DirectoryInfo(args[0]);
            WalkDirectoryTree(rootDir,args[1]);
        }

        static void WalkDirectoryTree(DirectoryInfo _root,string _path)
        {
            FileInfo[] files = null;
            DirectoryInfo[] subDirs = null;
            try
            {
                files = _root.GetFiles("*.*");
            }
            catch(UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
            }
            catch(DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            /*
             *  Create directory
             */
            Directory.CreateDirectory(_path);
            if(files!=null)
            {
                foreach(FileInfo fi in files)
                {
                    Console.WriteLine(fi.Name);
                    /*
                     *  Copy the file to that directory
                     */
                    if (fi.Extension == ".h" || fi.Extension == ".hpp" || fi.Extension == ".c"||fi.Extension == ".cpp")
                    {
                        CopyFiles(fi.FullName, _path + "/" + fi.Name);
                    }
                    else
                    {
                        File.Copy(fi.FullName, _path + "/" + fi.Name,true);
                    }
                }

                subDirs = _root.GetDirectories();

                foreach(DirectoryInfo dir in subDirs)
                {
                    WalkDirectoryTree(dir,_path+"/"+dir.Name);
                }
            }

        }

        static void CopyFiles(string _src,string _dst)
        {
            Window wnd=m_dte.ItemOperations.OpenFile(_src);
            TextDocument doc = m_dte.ActiveDocument.Object("TextDocument");
            var p = doc.StartPoint.CreateEditPoint();
            string content = p.GetText(doc.EndPoint);
            File.WriteAllText(_dst,content);
            wnd.Close();
        }
    }
}
