using Mcsg.Lib.Common.Helpers;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Enums;
using Mcsg.Media.Tool.Features;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Web;

namespace Mcsg.Media.Tool.Workers
{
    internal class ConvertAudioWorker : BaseWorker, IWorker
    {
        private const string TARGET = ".mp3";
        public ConvertAudioWorker(IConfiguration configuration)
            : base(configuration) { }

        public void Execute(Job jobInfo)
        {
            if (jobInfo.JobType != JobType.ConvertAudio)
                return;

            AddToPools(jobInfo.Id, Task.Factory.StartNew(async () =>
            {
                try
                {
                    // create clone ffmpeg
                    var ffmpge = CloneFFmpeg();

                    // load resource
                    var resourceInfo = JsonConvert.DeserializeObject<Resource>(jobInfo.Data);
                    var url = CryptoHelper.Decrypt(HttpUtility.UrlDecode(resourceInfo.Url), EncryptKey);
                    var orgfile = await DownloadBlobAsync(url, resourceInfo.Id);

                    var targetFile = Path.Combine(Path.GetDirectoryName(orgfile), Path.GetFileNameWithoutExtension(url) + TARGET);
                    if (File.Exists(targetFile))
                    {
                        File.Delete(targetFile);
                    }
                    if (Path.GetFileName(orgfile) != Path.GetFileName(targetFile))
                    {
                        //Run conversion                   
                        //veryslow,slower,slow, medium, fast,faster,veryfast,superfast, ultrafast 
                        string command = "-vn -ar 44100 -ac 2 -preset faster -b:a 128k"; // optimizer
                        RunFFmeg(ffmpge, orgfile, targetFile, command);
                        DisposeClonedFFmpeg(ffmpge);

                        //upload
                        var newUrl = url.Replace(Path.GetExtension(targetFile), TARGET);
                        await UploadBlobAsync(targetFile, newUrl);

                        //update job status
                        await DbService.UpdateJobStatus(jobInfo.Id, JobStatus.Success, string.Empty);

                        //correct resource table
                        var endCodenewUrl = HttpUtility.UrlEncode(CryptoHelper.Encrypt(newUrl, EncryptKey));
                        var shareUrl = await BlobStorageService.CreateShareUrl(newUrl, MediaContainer);
                        await DbService.UpdateResourceStatus(resourceInfo.Id, ResourceStatus.DONE,
                            endCodenewUrl, shareUrl.GetShareUrlFromStorage(StorageAccountName));
                    }

                    //clean up resource
                    File.Delete(orgfile);
                    File.Delete(targetFile);

                    var pool = Pools.FirstOrDefault(x => x.Id == jobInfo.Id);
                    if (pool != null)
                    {
                        pool.IsRunning = false;
                    }
                }
                catch (Exception ex)
                {
                    await DbService.UpdateJobStatus(jobInfo.Id, JobStatus.Failed, ex.Message);
                }
            }));
        }
    }
}
