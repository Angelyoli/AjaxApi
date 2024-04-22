using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Hosting;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;

namespace AjaxApi.Controllers
{
    [EnableCors("MyAllowSpecificOrigins")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FileUpController : ControllerBase
    {
        public IHostingEnvironment _env;
        public FileUpController(IHostingEnvironment env)
        {
            this._env = env;
        }

        [HttpGet]
        public string Test() => "This is test interface";

        [HttpPost]
        public async Task<OutPut> FileUp()
        {
            var ret = new OutPut();
            try
            {
                //不能用FromBody
                var dto = JsonConvert.DeserializeObject<ImagesDto>(Request.Form["ImageModelInfo"]);//文件类实体参数
                var files = Request.Form.Files;//接收上传的文件，可能多个 看前台
                if (files.Count > 0)
                {
                    var path = _env.ContentRootPath + @"/Uploads/Images/";//绝对路径
                    string dirPath = Path.Combine(path, dto.Type + "/");//绝对径路 储存文件路径的文件夹
                    if (!Directory.Exists(dirPath))//查看文件夹是否存在
                        Directory.CreateDirectory(dirPath);
                    var file = files.Where(x => true).FirstOrDefault();//只取多文件的一个
                    var fileNam = $"{Guid.NewGuid():N}_{file.FileName}";//新文件名
                    string snPath = $"{dirPath + fileNam}";//储存文件路径
                    using var stream = new FileStream(snPath, FileMode.Create);
                    await file.CopyToAsync(stream);
                    //次出还可以进行数据库操作 保存到数据库
                    ret = new OutPut { Code = 200, Msg = "上传成功", Success = true };
                }
                else//没有图片
                {
                    ret = new OutPut { Code = 400, Msg = "请上传图片", Success = false };
                }
            }
            catch (Exception ex)
            {
                ret = new OutPut { Code = 500, Msg = $"异常：{ex.Message}", Success = false };
            }
            return ret;
        }


    }
}
