using KevinHelper;11111
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Budsas
    {
        const string root = "https://www.budsas.org/uni/u-kinh-tieubo10/";
        const string path = @"D:\Docs\Development\C#\1ConsoleApplication\bin\Budsas\5-tieu";
        const string site = "tb15555.htm";
        static void Main1()
        {
            Budsas budsas = new Budsas();

            WebHelper.DownloadSite($"{root}{site}", path);
            //budsas.process_page($@"{path}\{site}");

            budsas.download_group();

            string[] list_files = Directory.GetFiles(path);
            List<string> errorfiles = new List<string>();
            Parallel.ForEach(list_files, file =>
            {
                int res = budsas.process_page(file);
                if (res > 0) errorfiles.Add(file);
            });

            Console.WriteLine($"finished.");
            //Console.ReadKey();
        }

        void download_group()
        {
            string content = File.ReadAllText($@"{path}\{site}");

            List<string> urls = new List<string>();
            foreach (Match m in Regex.Matches(content, @"<a\s+href=""([^>/]+?htm)"">", RegexOptions.Singleline | RegexOptions.IgnoreCase))
            {
                string group = m.Groups[1].Value;
                urls.Add(group);
            }
            Parallel.ForEach(urls, url =>
            {
                WebHelper.DownloadSite(root + url, path);
            });
        }
        int process_page(string file_path)
        {
            string html = File.ReadAllText(file_path);

            string new_content = Regex.Replace(html, @"<p .+This document is.+?<hr>", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (new_content == html) return 1;

            html = new_content;
            new_content = Regex.Replace(html, @"<font.+?>", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (new_content == html) return 2;

            html = new_content;
            new_content = Regex.Replace(html, @"</font>", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (new_content == html) return 3;

            html = new_content;
            new_content = Regex.Replace(html, @"<blockquote>|</blockquote>", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            html = new_content;
            new_content = Regex.Replace(html, @"<body .+?>", "<body>", RegexOptions.Singleline);
            if (new_content == html) return 5;

            html = new_content;
            new_content = Regex.Replace(html, @"<meta name=""GENERATOR"".+?>", "<meta name='viewport' content='width=device-width, initial-scale=1'>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (new_content == html) return 6;

            html = new_content;
            new_content = Regex.Replace(html, @"</head>", "<link rel='stylesheet' type='text/css' href='..\\css\\style.css'>\n</head>", RegexOptions.Singleline);

            html = new_content;
            new_content = Regex.Replace(html, @"<table.+?>|</table>|<tr>|</tr>|<td.+?>|</td>|<center>|</center>", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            html = new_content;
            new_content = Regex.Replace(html, @"<p ALIGN=""LEFT"">", "<p>", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            html = new_content;
            new_content = Regex.Replace(html, @"<hr.*?>", "<hr>", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            html = new_content;
            new_content = Regex.Replace(html, @"<div align=""center"">", "<div>", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            //html = new_content;
            //new_content = Regex.Replace(html, @"<p ALIGN=""center""><B>\[.+</p>", "<p><a href='trung00.htm'>Trung Bộ Kinh</a> - <a href='../index.htm'>Mục lục</a></p>", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            html = new_content;
            new_content = Regex.Replace(html, @"<p align=""center""><strong>\[<a href=""../index.htm"">Trở về\s+trang Thư Mục</a>]</strong></p>", "<p><a href='../index.htm'>Mục lục</a></p>", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            html = new_content;
            new_content = Regex.Replace(html, @"<p align=""center""><strong>\[<a href=""tieubo-00.htm"">Mục Lục.+?</p>", "<p><a href='tieubo-00.htm'>Tiểu Bộ Kinh</a> - <a href='../index.htm'>Mục lục</a></p>", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            File.WriteAllText(file_path, new_content);
            return 0;
        }
    }
}
