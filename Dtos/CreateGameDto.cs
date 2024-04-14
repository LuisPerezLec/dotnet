using System.ComponentModel.DataAnnotations;

namespace dotnet.Dtos;

public record class CreateGameDto(
    [Required][StringLength(50)] string Name,
    int GenreId,
    [Range(1,100)] decimal Price, //The way to make this optional is to set it as decimal? Price
    DateOnly ReleaseDate
);
