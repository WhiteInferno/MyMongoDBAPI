using System;
using Microsoft.AspNetCore.Mvc;
using MyMongoDbApi.Models;
using MyMongoDbApi.Context;

namespace MyMongoDbApi.Controllers;

[Controller]
[Route("api/[controller]")]
public class ClienteController : Controller
{
    private readonly MongoDBContext _mongoDBContext;

    public ClienteController(MongoDBContext mongoDBContext){
        _mongoDBContext = mongoDBContext;
    }

    [HttpGet]
    public async Task<List<Cliente>> Get() {
        return await _mongoDBContext.GetAsync();
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Cliente cliente) {
        await _mongoDBContext.CreateAsync(cliente);
        return CreatedAtAction(nameof(Get), new { id = cliente.Id}, cliente);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AddToComision(string id, [FromBody] string comisionId) {
        await _mongoDBContext.AddToComisionAsync(id, comisionId);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id) {
        await _mongoDBContext.DeleteAsync(id);
        return NoContent();
    }
}