using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Interfaces;

namespace Core.Application.Services
{
    public class AlunoService : IAlunoService
    {
        private readonly IAlunoRepository _alunoRepository;

        public AlunoService(IAlunoRepository alunoRepository)
        {
            _alunoRepository = alunoRepository;
        }

        public async Task AddAsync(AlunoDto alunoDto)
        {
            Aluno aluno = new Aluno
            {
                Nome = alunoDto.Nome,
                Email = alunoDto.Email,
                Cpf = alunoDto.Cpf,
                DataNascimento = alunoDto.DataNascimento
            };

            await _alunoRepository.AddAsync(aluno);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            Aluno aluno = await _alunoRepository.GetByIdAsync(id);

            if (aluno == null)
            {
                return false;
            }

            await _alunoRepository.DeleteAsync(aluno.Id);

            return true;
        }

        public async Task<IEnumerable<AlunoDto>> GetAllAsync()
        {
            IEnumerable<Aluno> alunos = await _alunoRepository.GetAllAsync();

            return alunos.Select(a => new AlunoDto(a.Id, a.Nome, a.Email, a.Cpf, a.DataNascimento));
        }

        public async Task<AlunoDto> GetByIdAsync(int id)
        {
            Aluno aluno = await _alunoRepository.GetByIdAsync(id);

            if (aluno == null)
            {
                return null;
            }

            return new AlunoDto(aluno.Id, aluno.Nome, aluno.Email, aluno.Cpf, aluno.DataNascimento);
        }

        public async Task UpdateAsync(AlunoDto alunoDto)
        {
            Aluno aluno = await _alunoRepository.GetByIdAsync(alunoDto.Id);

            if (aluno != null)
            {
                aluno.Nome = alunoDto.Nome;
                aluno.Email = alunoDto.Email;
                aluno.Cpf = alunoDto.Cpf;
                aluno.DataNascimento = alunoDto.DataNascimento;

                await _alunoRepository.UpdateAsync(aluno);
            }
        }
    }
}
