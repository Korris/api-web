using Newtonsoft.Json;

namespace Mcsg.Media.Tool.Workers;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Core.Requests;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Extensions;
using Interfaces;

internal class ConvertVideoWorker : BaseWorker, IWorker
{
    private const string TARGET = ".mp4";

    public ConvertVideoWorker(IMcsgContext context, ISetting setting, IStorageClient sc) : base(context, setting, sc) { }

    public void Execute(Job jobInfo)
    {
        if (jobInfo.JobType != JobType.ConvertVideo)
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
                    /*Breakdown of the command:
                     -c:v libx264                               : Use H.264 video codec
                     -vf "scale=trunc(iw/3)*2:trunc(ih/3)*2"    : Scale video to 1 / 3 of original size, ensuring even dimensions
                     -preset faster                             : Encoding speed preset(options: ultrafast, superfast, veryfast, faster, fast, medium, slow, slower, veryslow)
                     -crf 28                                    : Constant Rate Factor(0 - 51, lower is better quality but larger file size)
                     -maxrate 2M                                : Maximum bitrate cap at 2 Mbps
                     -bufsize 4M                                : Video buffer size for rate control
                     -c:a aac                                   : Use AAC audio codec
                     -b:a 96k                                   : Set audio bitrate to 96 kbps

                     Note: This command balances compression and quality.Adjust parameters as needed:
                     - Lower CRF for better quality (e.g., 23 - 25 for high quality, 28 - 32 for smaller file size)
                     - Change preset for different encoding speed / efficiency trade - offs
                     - Modify scale factor to adjust output resolution
                     - Alter maxrate and bufsize to control bitrate
                     - Adjust audio bitrate(-b:a) for different audio quality levels
                    */

                    /*
                    ffmpeg -i {input-file} -c:v libx264 -vf \"scale=trunc(iw/6)*2:trunc(ih/6)*2\" -b:v 1000k -preset faster -crf 32 -c:a aac -b:a 64k {output-file} #compress video with low quality and small size
                    ffmpeg -i {input-file} -c:v libx264 -vf "scale=trunc(iw/3)*2:trunc(ih/3)*2" -preset faster -crf 28 -maxrate 2M -bufsize 4M -c:a aac -b:a 96k {output-file} #compress video with good quality and normal size
                    */

                    //var command = "-c:v libx264 -vf \"scale=trunc(iw/6)*2:trunc(ih/6)*2\" -b:v 1000k -preset faster -crf 32 -c:a aac -b:a 64k"; // optimizer
                    var command = "-c:v libx264 -vf \"scale=trunc(iw/3)*2:trunc(ih/3)*2\" -preset faster -crf 28 -maxrate 2M -bufsize 4M -c:a aac -b:a 96k"; // optimizer
                    orgfile.RunFfmpeg(targetFile, command);

                    //upload
                    var newUrl = url.Replace(Path.GetExtension(targetFile), TARGET);
                    var length = await UploadBlobAsync(targetFile, newUrl);

                    //update job status
                    await DbService.UpdateJobStatus(jobInfo.Id, JobStatus.Success, string.Empty);

                    await DbService.UpdateResourceStatus(resourceInfo.Id, ResourceStatus.Done, newUrl, resourceInfo.BucketName, microService, length);
                }

                //clean up resource
                File.Delete(orgfile);
                File.Delete(targetFile);

                var pool = Pools.FirstOrDefault(x => x.Id == jobInfo.Id);
                if (pool != null)
                {
                    pool.IsRunning = false;
                }

                //Send notification when video process completed
                var video = await DbService.LoadResource(resourceInfo.HashId, microService);

                Console.WriteLine("ConvertVideo Job Id: {0} - Video HashId : {1} - Video Id {2}", jobInfo.Id, resourceInfo.HashId, video.Id);

                if (video != null && !string.IsNullOrWhiteSpace(video.HashId))
                {
                    var notiReq = new VideoNotificationR
                    {
                        Id = video.Id,
                        AuthorId = video.AuthorId,
                        AuthorName = video.AuthorName,
                        Action = NotificationAction.Completed,
                        HashId = resourceInfo.HashId,
                        PostId = video.PostId,
                        PostHashId = video.PostHashId,
                        TargetType = Common.Core.Constants.Setting.NotificationTargetType.Social
                    };

                    await NotiService.AddVideoNotificationAsync(notiReq, _setting.Api.Web.Realtime);
                }
            }
            catch (Exception ex)
            {
                await DbService.UpdateJobStatus(jobInfo.Id, JobStatus.Failed, ex.Message);
            }
        }));
    }
}
