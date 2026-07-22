using Microsoft.AspNetCore.Mvc;
using TelegramBotApp.Application.DTOs;
using TelegramBotApp.Application.Interfaces;

namespace TelegramBotApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationDto dto)
    {
        try
        {
            await _notificationService.CreateNotificationAsync(dto);
            return Ok(new { Message = "Bildirim başarıyla kuyruğa eklendi." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}