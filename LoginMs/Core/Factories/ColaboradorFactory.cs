using Core.Dtos;
using Core.Entities;

namespace Core.Factories
{
    public static class ColaboradorFactory
    {
        public static Colaborador GetColaboradorByDto(ColaboradorDto colaboradorDto)
        {
            return new Colaborador
            {
                IdFuncao = colaboradorDto.IdFuncao,
                Nome = colaboradorDto.Nome,
                Email = colaboradorDto.Email,
                Senha = colaboradorDto.Senha
            };
        }
    }
}
