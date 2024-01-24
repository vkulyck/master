using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using GmWeb.Logic.Utility.Extensions.Http;

namespace GmWeb.Logic
{
    public class FileDownloader
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly Random _random = new Random();
        private List<Uri> _UriList { get; } = new List<Uri>();
        public IEnumerable<Uri> UriList
        {
            get => this._UriList;
            set
            {
                if (value == null)
                {
                    this._UriList.Clear();
                    return;
                }
                else
                {
                    this._UriList.AddRange(value);
                }
            }
        }
        public bool AllowOverwrite { get; set; }
        public string TargetDirectory { get; set; }
        public FileDownloader(IEnumerable<string> Uris = null, string TargetDirectory = null, bool AllowOverwrite = false)
            : this(Uris.Select(uriString => new Uri(uriString)), TargetDirectory, AllowOverwrite) { }
        public FileDownloader(IEnumerable<Uri> Uris = null, string TargetDirectory = null, bool AllowOverwrite = false)
        {
            this.UriList = Uris;
            this.TargetDirectory = TargetDirectory;
            this.AllowOverwrite = AllowOverwrite;
        }

        protected async Task DownloadDataAsync(Uri uri)
        {
            using (var client = new HttpClient())
            {
                string filename = Path.GetFileName(uri.GetComponents(UriComponents.Path, UriFormat.Unescaped));
                string path = Path.Combine(this.TargetDirectory, filename);
                _logger.Info($"Downloading {filename} to directory: {this.TargetDirectory}");
                uri = new Uri(uri, "?random=" + _random.Next().ToString());
                await client.DownloadFileAsync(uri, path);
                _logger.Info($"{filename} download completed.");
            }
        }

        public async Task<FileDownloader> Run()
        {
            var downloadTasks = new List<Task>();
            var block = new ActionBlock<Uri>(uri => this.DownloadDataAsync(uri), new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 2 });
            var responses = new List<Task<bool>>();
            foreach (var uri in this.UriList)
            {
                var response = block.SendAsync(uri);
                responses.Add(response);
            }
            block.Complete();
            await block.Completion;
            bool[] results = await Task.WhenAll(responses);
            if (results.Any(x => !x))
                throw new Exception($"Download failed.");
            return this;
        }
    }
}
