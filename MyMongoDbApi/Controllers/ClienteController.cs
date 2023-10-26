using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MyMongoDbApi.Models;
using MyMongoDbApi.Context;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using System.ComponentModel.DataAnnotations;

namespace MyMongoDbApi.Controllers;

[Controller]
[Route("api/[controller]")]
public class ClienteController : Controller
{
    private readonly MongoDBContext _mongoDBContext;

    public ClienteController(MongoDBContext mongoDBContext)
    {
        _mongoDBContext = mongoDBContext;
    }

    [HttpGet]
    public async Task<List<Cliente>> Get()
    {
        return await _mongoDBContext.GetAsync();
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Cliente cliente)
    {
        await _mongoDBContext.CreateAsync(cliente);
        return CreatedAtAction(nameof(Get), new { id = cliente.Id }, cliente);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AddToComision(string id, [FromBody] string comisionId)
    {
        await _mongoDBContext.AddToComisionAsync(id, comisionId);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _mongoDBContext.DeleteAsync(id);
        return Ok();
    }

    [HttpPost("UploadFileV1")]
    public async Task<IActionResult> UploadFileV1(string id, List<IFormFile> files)
    {
        long size = files.Sum(f => f.Length);

        foreach (var formFile in files)
        {
            if (formFile.Length > 0)
            {
                string byteString;
                using (var target = new MemoryStream())
                {
                    formFile.CopyTo(target);
                    var array = target.ToArray();
                    byteString = Encoding.UTF8.GetString(array);
                }
                await _mongoDBContext.AddToComisionAsync(id, byteString);
            }
        }

        return Ok();
    }

    [HttpGet("DownloadFileV1")]
    public async Task<IActionResult> DownloadFileV1(string id)
    {
        Cliente cliente = _mongoDBContext.GetById(id);

        foreach (string item in cliente.ComisionesIds)
        {
            if (item.Length > 0)
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(item);
                string mimeType = "application/pdf";
                return new FileContentResult(byteArray, mimeType);
            }
        }

        return Ok();
    }

    [HttpPost("UploadFileV2")]
    public async Task<IActionResult> UploadFileV2(List<IFormFile> files)
    {
        long size = files.Sum(f => f.Length);

        foreach (var formFile in files)
        {
            if (formFile.Length > 0)
            {
                MemoryStream stream = new MemoryStream();
                formFile.CopyTo(stream);
                await _mongoDBContext.UploadFromBytesAsync(
                    new FileContent()
                    {
                        Name = formFile.FileName,
                        Content = stream.ToArray()
                    });
            }
        }

        return Ok();
    }

    [HttpGet("DownloadFileV2")]
    public async Task<IActionResult> DownloadFileV2(string fileName)
    {
        var byteArray = await _mongoDBContext.DownloadFromStream(fileName);
        string mimeType = "application/pdf";
        return new FileContentResult(byteArray, mimeType);
    }

    private byte[] ToByteArray(Stream input)
    {
        byte[] buffer = new byte[16 * 1024];
        using (var ms = new MemoryStream())
        {
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }
    }
}