using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Gateways;
using System;

namespace Core.Gateways
{
    public class FuncaoColaboradorGateway(IDbConnection dbConnection) : IFuncaoColaboradorGateway
    {
        private readonly IDbConnection _dbConnection = dbConnection;
        private const string _tableName = "funcao_colaborador";

        public async Task CheckBasefuncaoColaboradorInserted()
        {
            List<FuncaoColaborador> existingFuncoes = (await _dbConnection.ListAllAsync<FuncaoColaborador>(_tableName)).ToList();

            // Se já existem funções cadastradas, não inserir as funções base
            if (existingFuncoes.Count > 0)
                return;

            List<FuncaoColaborador> funcoesColaborador = [];

            funcoesColaborador.Add(new()
            {
                IdFuncao = 1,
                Funcao = "Administrador(a)"
            });

            funcoesColaborador.Add(new()
            {
                IdFuncao = 2,
                Funcao = "Cozinheiro(a)"
            });

            funcoesColaborador.Add(new()
            {
                IdFuncao = 3,
                Funcao = "Atendente"
            });

            funcoesColaborador.AddRange(funcoesColaborador);

            foreach (FuncaoColaborador funcaoColaborador in funcoesColaborador)
            {
                await _dbConnection.InsertAsync(_tableName, new Dictionary<string, object>
                {
                    { "id_funcao", funcaoColaborador.IdFuncao },
                    { "funcao", funcaoColaborador.Funcao }
                });
            }
        }
    }
}
