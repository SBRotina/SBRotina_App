using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SbRotina.Data;
using SbRotina.Models;
using SbRotina.Repositorio.Interfaces;
using System.Secutiry.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.IdentityModel.Tokens.Jwt;


namespace SbRotina.Controllers
{ 
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

    public class UsuarioController : ControllerBase


    {
        private readonly SbRotinaDbContext _context;

        private readonly IUsuarioReposistorio _usuarioReposistorio;
        
        private readonly IConfiguration _configuration;

        public UsuarioController(IUsuarioReposistorio usuarioReposistorio, SbRotinaDbContext context, IConfiguration _configuration)
        {
            _usuarioReposistorio = usuarioReposistorio;
            _context = context;
            _configuration = _configuration;
        }


        private async Task<bool> UsuarioExistente(string email)
        {
            if (await _context.Usuarios.AnyAsync(x => x.Email.ToLower() == email.ToLower()))
            {
                return true;
            }
            return false;
        }


        [HttpGet("buscarTodos")]
        public async Task <ActionResult<List<UsuarioModel>>>  BuscarTodosUsuarios() 
            {
            
           List<UsuarioModel> usuarios =  await _usuarioReposistorio.BuscarTodosUsuarios();
            return Ok(usuarios);
 }

        [HttpGet ("buscarPorId")]
        public async Task<ActionResult<UsuarioModel>> BuscarPorId(int id)
        {
            UsuarioModel usuario = await _usuarioReposistorio.BuscarPorId(id);
            return Ok(usuario);
        }

        [HttpPost ("Adicionar")]
        public async Task<ActionResult<UsuarioModel>> Cadastrar([FromBody] UsuarioModel usuarioModel)
        {
            UsuarioModel usuario = await _usuarioReposistorio.Adicionar(usuarioModel);
            return Ok(usuario);
        }

        [HttpPut("Atualizar")]
        public async Task<ActionResult<UsuarioModel>> Atualizar([FromBody] UsuarioModel usuarioModel, int id)
        {
            usuarioModel.Id= id;
            UsuarioModel usuario = await _usuarioReposistorio.Atualizar(usuarioModel, id);
            return Ok(usuario);
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<UsuarioModel>> Deletar(int id)
        {
           bool deletado = await _usuarioReposistorio.Apagar(id);
            return Ok(deletado);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Autenticar(UsuarioModel creds)
        {
            try
            {
                UsuarioModel usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(x => x.Email.ToLower().Equals(creds.Email.ToLower()));

                if (usuario == null)
                    throw new Exception("Usuário não encontrado!");
                if (!usuario.Senha.Equals(creds.Senha))
                    throw new Exception("Senha incorreta!");
               
                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();
                usuario.Token = CriarToken(usuario);
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private string CriarToken(Usuario usuario)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome)
            };
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(_configuration.GetSection("ConfiguracaoToken:chave").Value));
            SigningCredentials creds = new SigningCredentials (key, SecurityAlgorithms.HmacSha512Signature);
            SecurityTokenDescrpiptor tokenDescriptor = new SecurityTokenDescrpiptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);


        }



    }
}
 