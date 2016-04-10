﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;



using System.Windows.Forms;
using System.IO;

namespace DirectoryReporter
{
    public class FolderAnalyzer
    {
        private static readonly FolderAnalyzer instance = new FolderAnalyzer();

        public DirectoryStorage Storage;

        public Thread work;

        private FolderAnalyzer() { }

        public delegate void PathPostingSatus(Object s, EventArgs e);
        public event PathPostingSatus OnPathPostingStart;
        public event PathPostingSatus OnPathPostingFinish;

        private string CurrentAnalizedPath;

        public string TargetDirectoryPath;

        public static FolderAnalyzer Instance
        {
            get
            {
                return instance;
            }
        }

        public void StartAnalyze(string path)
        {
            CurrentAnalizedPath = path;
            Thread DirectoryScan = new Thread(() => DirectorySearch(path));
            DirectoryScan.Start();
            OnPathPostingStart(this, EventArgs.Empty);
        }

        private void DirectorySearch(string directoryPath)
        {
            try
            {
                foreach (var f in Directory.GetFiles(directoryPath))
                {
                    Storage.PushDirectoryPath(f);
                    // Console.WriteLine(f);
                }
                foreach (string d in Directory.GetDirectories(directoryPath))
                {
                    Storage.PushDirectoryPath(d);
                    DirectorySearch(d);
                }
                if (directoryPath == CurrentAnalizedPath)
                {
                    if (OnPathPostingFinish != null)
                    {
                        OnPathPostingFinish(this, EventArgs.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}