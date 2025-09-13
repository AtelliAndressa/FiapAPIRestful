namespace Core.Application.DTOs
{
    public class CreateTurmaDto
    {
        public CreateTurmaDto()
        {
        }

        public CreateTurmaDto(string nome, string descricao)
        {
            Nome = nome;
            Descricao = descricao;
        }

        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
    }
}