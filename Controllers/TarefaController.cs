using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SbRotina.Models;
using SbRotina.Repositorio.Interfaces;

namespace SbRotina.Controllers
{ 
    [Route("api/[controller]")]
    [ApiController]
    public class TarefaController : ControllerBase
    {
        private readonly ITarefaRepositorio _tarefaReposistorio;

        private readonly IConfiguration _configuration;

        public TarefaController(ITarefaRepositorio tarefaReposistorio, IConfiguration _configuration)
        {
            _configuration = _configuration;
            _tarefaReposistorio = tarefaReposistorio;
        }


        [HttpGet("buscarPorTodos")]
        public async Task <ActionResult<List<TarefaModel>>> ListarTodas() 
            {
            
           List<TarefaModel> tarefas =  await _tarefaReposistorio.BuscarTodasTarefas();
            return Ok(tarefas);
 }

        [HttpGet ("buscarPorId")]
        public async Task<ActionResult<TarefaModel>> BuscarPorId(int id)
        {
            TarefaModel tarefa = await _tarefaReposistorio.BuscarPorId(id);
            return Ok(tarefa);
        }

        [HttpPost("Cadastrar")]
        public async Task<ActionResult<TarefaModel>> Cadastrar([FromBody] TarefaModel tarefaModel)
        {
            TarefaModel tarefa = await _tarefaReposistorio.Adicionar(tarefaModel);
            return Ok(tarefa);
        }

        [HttpPut("Atualizar")]
        public async Task<ActionResult<TarefaModel>> Atualizar([FromBody] TarefaModel tarefaModel, int id)
        {
            tarefaModel.Id= id;
            TarefaModel tarefa = await _tarefaReposistorio.Atualizar(tarefaModel, id);
            return Ok(tarefa);
        }

        [HttpDelete("Deletar")]
        public async Task<ActionResult<TarefaModel>> Deletar(int id)
        {
           bool deletado = await _tarefaReposistorio.Apagar(id);
            return Ok(deletado);
        }



    }
}
 