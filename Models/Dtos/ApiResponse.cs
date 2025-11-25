namespace WebApplication.Models.Dtos;

public record ApiResponse<T>(int status, string message, T data);
