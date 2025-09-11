using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class Curso
    {
        public int Id { get; set; }

        public required string Nome { get; set; }

        public required string Descricao { get; set; }
    }
}
