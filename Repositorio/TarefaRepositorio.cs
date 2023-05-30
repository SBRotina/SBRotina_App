using Microsoft.EntityFrameworkCore;
using SbRotina.Data;
using SbRotina.Models;
using SbRotina.Repositorio.Interfaces;

namespace SbRotina.Repositorio
{
    public class TarefaRepositorio : ITarefaRepositorio
    {
        private readonly SbRotinaDbContext _dbContext;
        public TarefaRepositorio(SbRotinaDbContext sbRotinaDbContext)
        {
            _dbContext = sbRotinaDbContext;
        }

        public async Task<TarefaModel> BuscarPorId(int id)
        {
            return await _dbContext.Tarefas
                .Include(x => x.Usuario)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<TarefaModel>> BuscarTodasTarefas()
        {
            return await _dbContext.Tarefas
                .Include(x => x.Usuario)
                .ToListAsync();
        }

        public async Task<TarefaModel> Adicionar(TarefaModel tarefa)
        {
            await _dbContext.Tarefas.AddAsync(tarefa);
            await _dbContext.SaveChangesAsync();
            
            return tarefa;
        }

        public async Task<TarefaModel> Atualizar(TarefaModel tarefa, int id)
        {
            TarefaModel tarefaPorId = await BuscarPorId(id);
           
            if(tarefaPorId == null)
            {
                throw new Exception($"Tarefa para o Id: {id} não foi encontrado. ");
            }

            tarefaPorId.Nome = tarefa.Nome;
            tarefaPorId.TipoTarefa = tarefa.TipoTarefa;
            tarefaPorId.Descricao = tarefa.Descricao;
            tarefaPorId.Status = tarefa.Status;
            tarefaPorId.PrioriddaeTarefa= tarefa.PrioriddaeTarefa;
            tarefaPorId.UsuarioId= tarefa.UsuarioId;

            _dbContext.Tarefas.Update(tarefaPorId);
            await _dbContext.SaveChangesAsync();

            return tarefaPorId;
        }

        public async Task<bool> Apagar(int id)
        {
            TarefaModel tarefaPorId = await BuscarPorId(id);

            if (tarefaPorId == null)
            {
                throw new Exception($"Tarefa  para o Id: {id} não foi encontrado. ");
            }

            _dbContext.Tarefas.Remove(tarefaPorId);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        

    }
}
