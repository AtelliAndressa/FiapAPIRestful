namespace Core.Application.DTOs
{
    public class TurmaDto
    {
        public TurmaDto() { }

        public TurmaDto(int id, string nome, string descricao, int matriculas)
        {
            Id = id;
            Nome = nome;
            Descricao = descricao;
            QuantidadeAlunos = matriculas;
        }

        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public int QuantidadeAlunos { get; set; }
    }
}