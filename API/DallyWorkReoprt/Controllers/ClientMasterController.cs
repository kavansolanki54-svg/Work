using AutoMapper;
using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;
using DallyWorkReoprt.DTO.Models;
using DallyWorkReoprt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DallyWorkReoprt.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ClientMasterController : BaseApiController
{
    private readonly IClientMasterRepository _clientRepo;
    private readonly IMapper _mapper;

    public ClientMasterController(IClientMasterRepository clientRepo, IMapper mapper)
    {
        _clientRepo = clientRepo;
        _mapper = mapper;
    }

    [HttpGet("List/{companyId}")]
    public IActionResult GetList(int companyId)
    {
        var clients = _clientRepo.GetAll(companyId, activeStatus: 1).ToList();
        var dtos = _mapper.Map<List<ClientDTO>>(clients);
        return Ok(ApiResponse<List<ClientDTO>>.SuccessResponse(dtos, "Clients retrieved successfully"));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var client = await _clientRepo.GetByIdAsync(id);
        if (client == null) 
            return NotFound(ApiResponse<ClientDTO>.ErrorResponse("Client not found"));
        
        var dto = _mapper.Map<ClientDTO>(client);
        return Ok(ApiResponse<ClientDTO>.SuccessResponse(dto, "Client retrieved successfully"));
    }

    [HttpPost("Save")]
    public async Task<IActionResult> Save([FromBody] ClientDTO clientDTO)
    {
        if (!ModelState.IsValid) return ValidationErrorResponse();

        var client = _mapper.Map<ClientMaster>(clientDTO);
        client.Guids = Guid.NewGuid();
        client.CreateDate = DateTime.Now;
        client.ActiveStatus = 1;

        var userName = User.Identity?.Name ?? "Admin";
        client.CreatedById = userName.Length > 20 ? userName.Substring(0, 20) : userName;

        await _clientRepo.AddAsync(client);
        await _clientRepo.SaveAsync();

        return Ok(ApiResponse<ClientMaster>.SuccessResponse(client, "Client saved successfully"));
    }

    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromBody] ClientDTO clientDTO)
    {
        if (!ModelState.IsValid) return ValidationErrorResponse();

        var client = await _clientRepo.GetByIdAsync(clientDTO.ClientId);
        if (client == null) 
            return NotFound(ApiResponse<ClientMaster>.ErrorResponse("Client not found"));

        _mapper.Map(clientDTO, client);
        
        var userName = User.Identity?.Name ?? "Admin";
        client.ModifiedById = userName.Length > 20 ? userName.Substring(0, 20) : userName;
        client.ModifiedDate = DateTime.Now;

        await _clientRepo.UpdateAsync(client);
        await _clientRepo.SaveAsync();

        return Ok(ApiResponse<ClientMaster>.SuccessResponse(client, "Client updated successfully"));
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var client = await _clientRepo.GetByIdAsync(id);
        if (client == null) 
            return NotFound(ApiResponse<ClientMaster>.ErrorResponse("Client not found"));

        client.ActiveStatus = 0; // Soft delete pattern
        
        var userName = User.Identity?.Name ?? "Admin";
        client.ModifiedById = userName.Length > 20 ? userName.Substring(0, 20) : userName;
        client.ModifiedDate = DateTime.Now;

        await _clientRepo.UpdateAsync(client);
        await _clientRepo.SaveAsync();

        return Ok(ApiResponse<ClientMaster>.SuccessResponse(client, "Client deleted successfully"));
    }
}
