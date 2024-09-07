using Newtonsoft.Json;

namespace Mcsg.Media.Tool.Workers;

using Common.Core.Enums;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Extensions;
using Interfaces;

internal class ConvertAudioWorker : BaseWorker, IWorker
{
    private const string TARGET = ".mp3";

    public ConvertAudioWorker(IMcsgContext context, ISetting setting, IStorageClient sc) : base(context, setting, sc) { }

    public void Execute(Job jobInfo)
    {
        if (jobInfo.JobType != JobType.ConvertAudio)
            return;

        AddToPools(jobInfo.Id, Task.Factory.StartNew(async () =>
        {
            try
            {
                // load resource
                var resourceInfo = JsonConvert.DeserializeObject<BaseResource>(jobInfo.Data);
                var url = resourceInfo.Url;
                var orgfile = await DownloadBlobAsync(url, resourceInfo.Id);
                var microService = resourceInfo.MicroService.ToEnum(MicroService.Social);

                var targetFile = Path.Combine(Path.GetDirectoryName(orgfile), Path.GetFileNameWithoutExtension(url) + TARGET);
                if (File.Exists(targetFile))
                {
                    File.Delete(targetFile);
                }
                if (Path.GetFileName(orgfile) != Path.GetFileName(targetFile))
                {
                    int width = 0, height = 0;

                    //Run conversion
                    //veryslow,slower,slow, medium, fast,faster,veryfast,superfast, ultrafast 
                    string command = "-vn -ar 44100 -ac 2 -preset faster -b:a 128k"; // optimizer
                    RunFfmpeg(orgfile, targetFile, command);

                    //upload
                    var newUrl = url.Replace(Path.GetExtension(targetFile), TARGET);
                    var length = await UploadBlobAsync(targetFile, newUrl);

                    //update job status
                    await DbService.UpdateJobStatus(jobInfo.Id, JobStatus.Success, string.Empty);

                    await DbService.UpdateResourceStatus(resourceInfo.Id, ResourceStatus.Done, newUrl, resourceInfo.BucketName, microService, width, height, length);
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
